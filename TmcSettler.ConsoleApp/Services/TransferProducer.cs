using DALContext;
using DALContext.Model;
using LoggerHelper.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TmcSettler.ConsoleApp.Services
{
    public class TransferProducer
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

            Console.WriteLine("Transfer Proccessing Batch Complete");
        }

        private void Producer()
        {
            EtzbkDataContext db = new EtzbkDataContext();
            var etzTrx = db.E_TRANSACTION.Where(a => a.TRANS_CODE == "T").ToList();


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
            List<E_TRANSFER_COMMISSION_SPLIT> splitFormular = CachingProvider.GetCachedData<List<E_TRANSFER_COMMISSION_SPLIT>>("Transfer");
            List<E_FUNDGATE_COMMISSION_SPLIT> fundGatesplitFormular = CachingProvider.GetCachedData<List<E_FUNDGATE_COMMISSION_SPLIT>>("FundGate");

            List<E_COMMISSION_MAP> commission = AutoMapper.Mapper.Map<List<E_COMMISSION_MAP>>(splitFormular);
            List<E_COMMISSION_MAP> fundGatecommission = AutoMapper.Mapper.Map<List<E_COMMISSION_MAP>>(fundGatesplitFormular);

            EtzbkDataContext etzbk = new EtzbkDataContext();
            etzbk.Configuration.AutoDetectChangesEnabled = false;

            int i = 0;
            foreach (var item in enqueData.GetConsumingEnumerable())
            {

                List<E_FEE_DETAIL_BK> feeDetailList = new List<E_FEE_DETAIL_BK>();
                if(item.CHANNELID=="09")
                    feeDetailList = FeeProcessing.ProcessCardloadSplit(item, fundGatecommission);
                else
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



        private static bool CheckTransactionStatusOnTMC(string UNIQUE_TRANSID, string TRANS_CODE)
        {
            bool value = true;
            Console.WriteLine("Checking Transaction" + UNIQUE_TRANSID);
            return value;

        }
    }
}
