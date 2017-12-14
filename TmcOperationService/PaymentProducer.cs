using DALContext;
using DALContext.Model;
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

            public void Run()
            {
                enqueData = new BlockingCollection<E_TRANSACTION>();
                Task t1 = Task.Factory.StartNew(Producer);
                Task t2 = Task.Factory.StartNew(Consumer);

                List<Task> taskList = new List<Task> { t1, t2 };
                Task.WaitAll(taskList.ToArray());

            Console.WriteLine("Payment Proccessing Batch Complete");
            }

            private void Producer()
            {
                EtzbkDataContext db = new EtzbkDataContext();
                var etzTrx = db.E_TRANSACTION.Where(a => a.TRANS_CODE == "P").ToList();


                Parallel.ForEach(etzTrx, item =>
                {

                    bool successful = CheckTransactionStatusOnTMC(item.UNIQUE_TRANSID, item.TRANS_CODE);

                    if (successful)
                    {
                        enqueData.Add(item);
                        Console.WriteLine("Equeued Data" + item.UNIQUE_TRANSID);
                    }
                    else
                    {
                        itemsToRemove.Add(item);
                    }
                });

                enqueData.CompleteAdding();

            }


        private void Consumer()
        {
            EtzbkDataContext etzbk = new EtzbkDataContext();
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
                    ///Write Code to handle No Merchant Code or Split category configured
                }
                if (merchantScale.SPECIAL_SPLIT == "0")
                {
                    // Check If Fee is Charged if not, ignore and comparee value
                    if (merchantScale.SCALE_TYPE == "1" & item.FEE == 0)
                        item.FEE = FeeProcessing.CalculateFeeBeneficiary(merchantScale.SCALE_VALUE, item.TRANS_AMOUNT);
                    var query = from A in etzbk.E_MERCHANT_COMMISSION_SPLIT
                                where (A.MERCHANT_CODE == item.MERCHANT_CODE)
                                select new E_COMMISSION_MAP
                                {
                                    AGENT = "",
                                    MAIN_FLAG = A.MAIN_FLAG,
                                    SPLIT_CARD = A.SPLIT_CARD,
                                    RATIO = A.RATIO,
                                    SPLIT_DESCR = A.SPLIT_DESCR,
                                    COMM_SUSPENCE = item.MERCHANT_CODE

                                };
                    List<E_COMMISSION_MAP> commission = AutoMapper.Mapper.Map<List<E_COMMISSION_MAP>>(query.ToList());

                    E_SETTLEMENT_DOWNLOAD_BK e_settlement = AutoMapper.Mapper.Map<E_SETTLEMENT_DOWNLOAD_BK>(item);

                    etzbk.E_SETTLEMENT_DOWNLOAD_BK.Add(e_settlement);
                    feeDetailList = FeeProcessing.ProcessRatioPaymentSplit(item, commission);

                }
                else
                {
                    var query = from A in etzbk.E_MERCHANT_SPECIAL_SPLIT
                                where (A.MERCHANT_CODE == item.MERCHANT_CODE)
                                select new E_COMMISSION_MAP
                                {
                                    AGENT = "",
                                    MAIN_FLAG = A.MAIN_FLAG,
                                    SPLIT_CARD = A.SPLIT_CARD,
                                    RATIO = A.SVALUE,
                                    SPLIT_DESCR = A.SPLIT_DESCR,
                                    COMM_SUSPENCE = item.MERCHANT_CODE

                                };
                    List<E_COMMISSION_MAP> commission = AutoMapper.Mapper.Map<List<E_COMMISSION_MAP>>(query.ToList());

                    //  feeDetailList = FeeProcessing.ProcessAbsolutePaymentSplit(item, commission);

                }



                // feeDetailList = FeeProcessing.ProcessCardloadSplit(item, commission);
                etzbk.E_FEE_DETAIL_BK.AddRange(feeDetailList);
                if (i % 50 == 0)
                {
                    etzbk.SaveChanges();
                }
                i++;


            }
            etzbk.SaveChanges();
        }



            private static bool CheckTransactionStatusOnTMC(string UNIQUE_TRANSID, string TRANS_CODE)
            {
                bool value = true;
                Console.WriteLine("Checking Transaction" + UNIQUE_TRANSID);
                return value;

            }
        }

}
