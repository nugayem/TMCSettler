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
    public class TransferProducer
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
           // Task.WaitAll(taskList.ToArray());

            Console.WriteLine("Transfer Proccessing Batch Complete");
        }

        private void Producer()
        {
            using (EtzbkDataContext db = new EtzbkDataContext())
            {
                try
                {
                    List<E_TRANSACTION> etzTrx = db.E_TRANSACTION.Where(a => a.TRANS_CODE == "T" && (a.PROCESS_STATUS == "0" || a.PROCESS_STATUS == null)).Take(Settings.number_of_record_perround).ToList();


                    Parallel.ForEach(etzTrx, item =>
                    {

                        bool successful = DataManupulation.CheckTransactionStatusOnTMC(item.UNIQUE_TRANSID, item.TRANS_CODE);

                        if (successful)
                        {
                            enqueData.Add(item);
                            Console.WriteLine("Equeued Data" + item.UNIQUE_TRANSID);
                        }
                        else
                        {
                            itemsToRemove.Add(item);
                            logger.LogInfoMessage(nameof(TransferProducer) + " round saved to database ");
                        }
                    });


                    enqueData.CompleteAdding();
                    DataManupulation.RemoveTransactionFromSettlement(itemsToRemove);
                    DataManupulation.UpdateTransactionAsProcccessed(etzTrx);
                }
                catch (Exception ex)
                {
                    Logger logger = new Logger();
                    Console.WriteLine("Exception from EtranzactChannelTransaction Run " + ExceptionExtensions.GetFullMessage(ex));
                    logger.LogInfoMessage("Exception from Run " + nameof(TransferProducer) + " " + ExceptionExtensions.GetFullMessage(ex));
                }
            }

        }


        private void Consumer()

        {
            List<E_TRANSFER_COMMISSION_SPLIT> splitFormular = CachingProvider.GetCachedData<List<E_TRANSFER_COMMISSION_SPLIT>>("Transfer");
            List<E_FUNDGATE_COMMISSION_SPLIT> fundGatesplitFormular = CachingProvider.GetCachedData<List<E_FUNDGATE_COMMISSION_SPLIT>>("FundGate");

            List<CommissionMapViewModel> commission = AutoMapper.Mapper.Map<List<CommissionMapViewModel>>(splitFormular);
            List<CommissionMapViewModel> fundGatecommission = AutoMapper.Mapper.Map<List<CommissionMapViewModel>>(fundGatesplitFormular);

            using (EtzbkDataContext etzbk = new EtzbkDataContext())
            {
                try
                {
                    etzbk.Configuration.AutoDetectChangesEnabled = false;

                    int i = 0;
                    foreach (var item in enqueData.GetConsumingEnumerable())
                    {

                        List<E_FEE_DETAIL_BK> feeDetailList = new List<E_FEE_DETAIL_BK>();
                        if (item.CHANNELID == "09")
                            feeDetailList = FeeProcessing.ProcessCardloadSplit(item, fundGatecommission);
                        else
                            feeDetailList = FeeProcessing.ProcessCardloadSplit(item, commission);

                        etzbk.E_FEE_DETAIL_BK.AddRange(feeDetailList);

                        E_SETTLEMENT_DOWNLOAD_BK e_settlement = AutoMapper.Mapper.Map<E_SETTLEMENT_DOWNLOAD_BK>(item);
                        decimal bankFee = feeDetailList.Where(fee => fee.MERCHANT_CODE.EndsWith("9999")).Select(FEE => FEE.FEE).FirstOrDefault();
                        e_settlement.BANK_FEE = bankFee;
                        etzbk.E_SETTLEMENT_DOWNLOAD_BK.Add(e_settlement);


                        if (i % 50 == 0)
                        {
                            etzbk.SaveChanges();
                        }
                        i++;


                    }
                    etzbk.SaveChanges();
                }
                catch (Exception ex)
                {
                    Logger logger = new Logger();
                    Console.WriteLine("Exception from EtranzactChannelTransaction Run " + ExceptionExtensions.GetFullMessage(ex));
                    logger.LogInfoMessage("Exception from Run " + nameof(EtranzactChannelTransaction) + " " + ExceptionExtensions.GetFullMessage(ex));
                }
            }
        }

    }
}
