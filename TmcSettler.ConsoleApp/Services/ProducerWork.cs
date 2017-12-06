using DALContext;
using DALContext.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TmcSettler.ConsoleApp.Services
{
    public class ProducerWork
    {
        private static BlockingCollection<E_TRANSACTION> enqueData;//= new BlockingCollection<E_Transaction>();
        private static List<E_TRANSACTION> itemsToRemove = new List<E_TRANSACTION>();

        private static void Producer()

        {

            EtzbkDataContext db = new EtzbkDataContext();

            var etzTrx = db.E_TRANSACTION.ToList();

            Parallel.ForEach(etzTrx, item =>
            {
                // Check TMC if transaction has been reversed.    

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

            db.E_TRANSACTION.RemoveRange(itemsToRemove);


        }

        private static void Consumer()

        {

            foreach (var item in enqueData.GetConsumingEnumerable())

            {

                Console.WriteLine(item);

            }

        }


        private static bool  CheckTransactionStatusOnTMC(string UNIQUE_TRANSID, string TRANS_CODE)
        {
            bool value = false;

            return value;

        }
        
        public void SplitCardLoad_D_Transaction(E_TRANSACTION entity)
        {

            //var 
        }

      
            
    }
}
