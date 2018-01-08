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
    public class CardloadProducer
    {


        private static BlockingCollection<E_TRANSACTION> enqueData;//= new BlockingCollection<E_Transaction>();

        List<E_TRANSACTION> itemsToRemove = new List<E_TRANSACTION>();

        Logger logger = new Logger();
        public void Run()
        {
            try { 
            enqueData = new BlockingCollection<E_TRANSACTION>();
            Task t1 = Task.Factory.StartNew(Producer);
            Task t2 = Task.Factory.StartNew(Consumer);

            List<Task> taskList = new List<Task> { t1, t2 };
           // Task.WaitAll(taskList.ToArray());

            Console.WriteLine("CardLoad Proccessing Batch Complete");
        }
            catch(ArgumentException ae)
            {
                logger.LogDebugMessage("ArgumentException" + ae.InnerException.Message);
            }
            catch(Exception ex) { logger.LogDebugMessage("Outer Exception" + ex.InnerException.Message); }
        }

        private void Producer()
        {
            List<E_TRANSACTION> etzTrx = new List<E_TRANSACTION>();
            using (EtzbkDataContext db = new EtzbkDataContext())
            {
                try
                {
                    etzTrx = db.E_TRANSACTION.Where(a => a.TRANS_CODE == "D" && (a.PROCESS_STATUS == "0" || a.PROCESS_STATUS == null)).Take(Settings.number_of_record_perround).ToList();

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
                        }
                    });
                }
                catch (Exception ex)
                {
                    Logger logger = new Logger();
                    Console.WriteLine("Exception from " + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ExceptionExtensions.GetFullMessage(ex));
                    logger.LogInfoMessage(nameof(CardloadProducer) + " " + ExceptionExtensions.GetFullMessage(ex));
                }
            }
            enqueData.CompleteAdding();

            DataManupulation.RemoveTransactionFromSettlement(itemsToRemove);
            DataManupulation.UpdateTransactionAsProcccessed(etzTrx);
        }


        private void Consumer()

        {
            List<E_CARDLOAD_COMMISSION_SPLIT> splitFormular = CachingProvider.GetCachedData<List<E_CARDLOAD_COMMISSION_SPLIT>>("CardLoad");

            List<CommissionMapViewModel> commission = AutoMapper.Mapper.Map<List<CommissionMapViewModel>>(splitFormular);


            using (EtzbkDataContext etzbk = new EtzbkDataContext())
            {
                etzbk.Configuration.AutoDetectChangesEnabled = false;

                try
                {
                    int i = 0;
                    foreach (var item in enqueData.GetConsumingEnumerable())
                    {

                        List<E_FEE_DETAIL_BK> feeDetailList = new List<E_FEE_DETAIL_BK>();
                        feeDetailList = FeeProcessing.ProcessCardloadSplit(item, commission);
                        etzbk.E_FEE_DETAIL_BK.AddRange(feeDetailList);

                        E_SETTLEMENT_DOWNLOAD_BK e_settlement = AutoMapper.Mapper.Map<E_SETTLEMENT_DOWNLOAD_BK>(item);
                        decimal bankFee = feeDetailList.Where(fee => fee.MERCHANT_CODE.EndsWith("9999")).Select(FEE => FEE.FEE).FirstOrDefault();
                        e_settlement.BANK_FEE = bankFee;
                        etzbk.E_SETTLEMENT_DOWNLOAD_BK.Add(e_settlement);


                        if (i % 50 == 0)
                        {
                            etzbk.SaveChanges();
                            logger.LogInfoMessage( nameof(CardloadProducer) + " round saved to database " );

                        }
                        i++;

                    }
                    etzbk.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception from EtranzactChannelTransaction Run " + ExceptionExtensions.GetFullMessage(ex));
                    logger.LogInfoMessage("Exception from Run " + nameof(CardloadProducer) + " " + ExceptionExtensions.GetFullMessage(ex));
                }

            }
        }



        
    }
}
