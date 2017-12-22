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

        public void Run()
        {
            enqueData = new BlockingCollection<E_TRANSACTION>();
            Task t1 = Task.Factory.StartNew(Producer);
            Task t2 = Task.Factory.StartNew(Consumer);

            List<Task> taskList = new List<Task> { t1, t2};
            Task.WaitAll(taskList.ToArray());

            Console.WriteLine("CardLoad Proccessing Batch Complete");
        }

        private void Producer()
        {
            EtzbkDataContext db = new EtzbkDataContext();            
            List<E_TRANSACTION> etzTrx = db.E_TRANSACTION.Where(a => a.TRANS_CODE == "D" && (a.PROCESS_STATUS == "0" || a.PROCESS_STATUS == null)).Take(Settings.number_of_record_perround).ToList();


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

            enqueData.CompleteAdding();

            DataManupulation.RemoveTransactionFromSettlement(itemsToRemove);
            DataManupulation.UpdateTransactionAsProcccessed(etzTrx);
        }


        private void Consumer()

        {
            List<E_CARDLOAD_COMMISSION_SPLIT> splitFormular = CachingProvider.GetCachedData<List<E_CARDLOAD_COMMISSION_SPLIT>>("CardLoad");

            List<CommissionMapViewModel> commission = AutoMapper.Mapper.Map<List<CommissionMapViewModel>>(splitFormular);


            EtzbkDataContext etzbk = new EtzbkDataContext();
            etzbk.Configuration.AutoDetectChangesEnabled = false;

            int i = 0;
            foreach (var item in enqueData.GetConsumingEnumerable())
            {

                List<E_FEE_DETAIL_BK> feeDetailList = new List<E_FEE_DETAIL_BK>();
                feeDetailList = FeeProcessing.ProcessCardloadSplit(item, commission);
                etzbk.E_FEE_DETAIL_BK.AddRange(feeDetailList);
                if (i % 50 == 0)
                {
                    etzbk.SaveChanges();
                }
                i++;


            }
            etzbk.SaveChanges();
        }



        
    }
}
