using DALContext;
using DALContext.Model;
using LoggerHelper.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TmcOperationService
{
    public class PaymentProducer
    {
        private static BlockingCollection<E_TRANSACTION> enqueData;//= new BlockingCollection<E_Transaction>();

        List<E_TRANSACTION> itemsToRemove = new List<E_TRANSACTION>();
        Logger logger = new Logger();

        public void Run()
        {
            enqueData = new BlockingCollection<E_TRANSACTION>();
            Task t1 = Task.Factory.StartNew(Producer);
            Task t2 = Task.Factory.StartNew(Consumer);

            List<Task> taskList = new List<Task> { t1, t2 };
          //  Task.WaitAll(taskList.ToArray());

            Console.WriteLine("Payment Proccessing Batch Complete");
        }

        private void Producer()
        {
            EtzbkDataContext db = new EtzbkDataContext();
            List<E_TRANSACTION> etzTrx = db.E_TRANSACTION.Where(a => a.TRANS_CODE == "P" && (a.PROCESS_STATUS == "0" || a.PROCESS_STATUS == null)).Take(Settings.number_of_record_perround).ToList();

            try
            {
                Parallel.ForEach(etzTrx, item =>
                {

                    bool successful = DataManupulation.CheckTransactionStatusOnTMC(item.UNIQUE_TRANSID, item.TRANS_CODE);

                    if (successful)
                    {
                        enqueData.Add(item);
                        Console.WriteLine("Equeued Payment Data " + item.UNIQUE_TRANSID);
                    }
                    else
                    {
                        itemsToRemove.Add(item);
                    }
                });
            }
            catch (Exception ex)
            {
                Logger logger = new Logger();
                Console.WriteLine("Exception from " + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ExceptionExtensions.GetFullMessage(ex));
                logger.LogInfoMessage(nameof(PaymentProducer) + " " + ExceptionExtensions.GetFullMessage(ex));
            }
            enqueData.CompleteAdding();
            DataManupulation.RemoveTransactionFromSettlement(itemsToRemove);
            DataManupulation.UpdateTransactionAsProcccessed(etzTrx);

        }


        private void Consumer()
        {
            List<string> McNoSplit = new List<string>();

            using (EtzbkDataContext etzbk = new EtzbkDataContext())
            {
                try
                {
                    etzbk.Configuration.AutoDetectChangesEnabled = false;
                    int i = 0;
                    foreach (var item in enqueData.GetConsumingEnumerable())
                    {
                        string Merchant_Code = item.MERCHANT_CODE;
                        List<E_FEE_DETAIL_BK> feeDetailList = new List<E_FEE_DETAIL_BK>();

                        var merchantScaleQuery = from A in etzbk.E_MERCHANT
                                                 join B in etzbk.E_CATSCALE
                                                 on new { X = A.CAT_ID } equals new { X = B.CAT_ID } into jointData
                                                 from joinRecord in jointData.DefaultIfEmpty()
                                                 where (A.MERCHANT_CODE == Merchant_Code)
                                                 select new
                                                 {
                                                     A.FEE_STATUS,
                                                     A.SPECIAL_SPLIT,
                                                     joinRecord.SCALE_VALUE,
                                                     joinRecord.SCALE_TYPE,
                                                     joinRecord.CAT_ID
                                                 };

                        var merchantScale = merchantScaleQuery.FirstOrDefault();
                        if (merchantScale == null)
                        {
                            if (!McNoSplit.Contains(Merchant_Code))
                                McNoSplit.Add(Merchant_Code);
                            continue;
                            ///Write Code to handle No Merchant Code or Split category configured
                        }
                        if (merchantScale.SPECIAL_SPLIT == "0")
                        {
                            // Check If Fee is Charged if not, ignore and comparee value
                            if (merchantScale.SCALE_TYPE == "1" & item.FEE == 0)
                                item.FEE = FeeProcessing.CalculateFeeBeneficiary(merchantScale.SCALE_VALUE, item.TRANS_AMOUNT);
                            var query = from A in etzbk.E_MERCHANT_COMMISSION_SPLIT
                                        where (A.MERCHANT_CODE == item.MERCHANT_CODE)
                                        select new CommissionMapViewModel
                                        {
                                            AGENT = "",
                                            MAIN_FLAG = A.MAIN_FLAG,
                                            SPLIT_CARD = A.SPLIT_CARD,
                                            RATIO = A.RATIO,
                                            SPLIT_DESCR = A.SPLIT_DESCR,
                                            COMM_SUSPENCE = item.MERCHANT_CODE

                                        };
                            List<CommissionMapViewModel> commission = AutoMapper.Mapper.Map<List<CommissionMapViewModel>>(query.ToList());

                            var e_settlement = AutoMapper.Mapper.Map<E_SETTLEMENT_DOWNLOAD_BK>(item);

                            feeDetailList = FeeProcessing.ProcessRatioPaymentSplit(item, commission);
                            decimal bankFee = feeDetailList.Where(fee => fee.MERCHANT_CODE.EndsWith("9999")).Select(FEE => FEE.TRANS_AMOUNT).FirstOrDefault();
                            e_settlement.BANK_FEE = bankFee;
                            etzbk.E_SETTLEMENT_DOWNLOAD_BK.Add(e_settlement);

                        }
                        else
                        {
                            var query = from A in etzbk.E_MERCHANT_SPECIAL_SPLIT
                                        where (A.MERCHANT_CODE == item.MERCHANT_CODE)
                                        select new CommissionMapViewModel
                                        {
                                            AGENT = "",
                                            MAIN_FLAG = A.MAIN_FLAG,
                                            SPLIT_CARD = A.SPLIT_CARD,
                                            RATIO = A.SVALUE,
                                            SPLIT_DESCR = A.SPLIT_DESCR,
                                            COMM_SUSPENCE = item.MERCHANT_CODE

                                        };

                            List<CommissionMapViewModel> commission = AutoMapper.Mapper.Map<List<CommissionMapViewModel>>(query.ToList());

                            E_SETTLEMENT_DOWNLOAD_BK e_settlement = AutoMapper.Mapper.Map<E_SETTLEMENT_DOWNLOAD_BK>(item);

                            feeDetailList = FeeProcessing.ProcessRatioPaymentSplit(item, commission);

                            decimal bankFee = feeDetailList.Where(fee => fee.MERCHANT_CODE.EndsWith("9999")).Select(FEE => FEE.FEE).FirstOrDefault();
                            e_settlement.BANK_FEE = bankFee;
                            etzbk.E_SETTLEMENT_DOWNLOAD_BK.Add(e_settlement);

                        }



                        // feeDetailList = FeeProcessing.ProcessCardloadSplit(item, commission);
                        etzbk.E_FEE_DETAIL_BK.AddRange(feeDetailList);
                        if (i % 50 == 0)
                        {

                            //var uniqueIDs = feeDetailList.Select(u => u.UNIQUE_TRANSID).Distinct().ToArray();
                            //var uniqueIDsOnDB = etzbk.E_TRANSACTION.Where(u => uniqueIDs.Contains(u.UNIQUE_TRANSID)).Select(u => u.UNIQUE_TRANSID).ToArray();
                            //var feeForDBata = feeDetailList.Where(u => !uniqueIDsOnDB.Contains(u.UNIQUE_TRANSID));

                            etzbk.SaveChanges();
                            logger.LogInfoMessage(nameof(PaymentProducer) + " round saved to database ");
                        }
                        i++;



                    }


                    etzbk.SaveChanges();
                    logger.LogInfoMessage(nameof(PaymentProducer) + " FInal round saved to database ");
                }
                catch (Exception ex)
                {
                    Logger logger = new Logger();
                    Console.WriteLine("Exception from EtranzactChannelTransaction Run " + ExceptionExtensions.GetFullMessage(ex));
                    logger.LogInfoMessage("Exception from Run " + nameof(PaymentProducer) + " " + ExceptionExtensions.GetFullMessage(ex));
                }
                //Act on MC without Split

                Console.WriteLine("List of MC without split");
                foreach (string item in McNoSplit)
                {
                    Console.WriteLine(item);
                }

            }



        }

    }
}
