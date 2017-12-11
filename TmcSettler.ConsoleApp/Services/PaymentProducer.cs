﻿using DALContext;
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
            


                List<E_TRANSFER_COMMISSION_SPLIT> splitFormular = CachingProvider.GetCachedData<List<E_TRANSFER_COMMISSION_SPLIT>>("CardLoad");
            List<E_COMMISSION_MAP> commission = AutoMapper.Mapper.Map<List<E_COMMISSION_MAP>>(splitFormular);

            EtzbkDataContext etzbk = new EtzbkDataContext();
                etzbk.Configuration.AutoDetectChangesEnabled = false;

                int i = 0;
                foreach (var item in enqueData.GetConsumingEnumerable())
                {
                string Merchant_Code = item.MERCHANT_CODE;

                var spltConfir = from A in etzbk.E_MERCHANT
                                 join B in etzbk.E_CATSCALE
                                 on new { X = A.CAT_ID } equals new { X = B.CAT_ID } into jointData
                                 from joinRecord in jointData.DefaultIfEmpty()
                                 where (A.MERCHANT_CODE == Merchant_Code)
                                 select new { A.};
                                 
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



            private static bool CheckTransactionStatusOnTMC(string UNIQUE_TRANSID, string TRANS_CODE)
            {
                bool value = true;
                Console.WriteLine("Checking Transaction" + UNIQUE_TRANSID);
                return value;

            }
        }

}