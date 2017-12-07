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
    public class CardloadProducer
    {


        private static BlockingCollection<E_TRANSACTION> enqueData;//= new BlockingCollection<E_Transaction>();

        List<E_TRANSACTION> itemsToRemove = new List<E_TRANSACTION>();

        public void Run()
        {
            Task t1 = Task.Factory.StartNew(Producer);
        }

        private void Producer()
        {
            EtzbkDataContext db = new EtzbkDataContext();
            var etzTrx = db.E_TRANSACTION.Where(a => a.TRANS_CODE == "D").ToList();


            Parallel.ForEach(etzTrx, item =>
            {

                bool successful = CheckTransactionStatusOnTMC(item.UNIQUE_TRANSID, item.TRANS_CODE);

                if (successful)
                {
                    enqueData.Add(item);
                }
                else
                {
                    itemsToRemove.Add(item);
                }
            });

            enqueData.CompleteAdding();

        }


        private static void Consumer()

        {
            List<E_CARDLOAD_COMMISSION_SPLIT> splitFormular =CachingProvider.GetCachedData<List<E_CARDLOAD_COMMISSION_SPLIT>>("CardLoad");


            foreach (var item in enqueData.GetConsumingEnumerable())
            {

                List<E_FEE_DETAIL_BK> feeDetailList = new List<E_FEE_DETAIL_BK>();
                FeeProcessing.ProcessCardloadSplit(item, splitFormular);

            }

        }



        private static bool CheckTransactionStatusOnTMC(string UNIQUE_TRANSID, string TRANS_CODE)
        {
            bool value = true;

            return value;

        }
    }
}
