using DALContext;
using DALContext.Model;
using LoggerHelper.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TmcOperationService
{
    public static class DataManupulation
    {

        public static List<T> MergeEntityList<T>( List<List<T>> list)
        {
            int mergedSize = 0;
            foreach(var item   in list )
            {
                mergedSize += item.Count;
            }
            var allTmcData = new List<T>(mergedSize);

            foreach (var item in list)
            {
                allTmcData.AddRange(item);
            }

            return allTmcData;
        }
        
        //   private static TmcDataContext db = new TmcDataContext();

        public  static bool CheckTransactionStatusOnTMC(string UNIQUE_TRANSID, string TRANS_CODE)
        {
            Logger logger = new Logger();
            bool value = false;
            try
            {
                Console.WriteLine("Checking Transaction" + UNIQUE_TRANSID);

                TmcDataContext tmcData = new TmcDataContext();
                var reversed = tmcData.E_TMCREQUEST.Where(a => a.TRANS_DATA == UNIQUE_TRANSID && a.MTI == "0420").FirstOrDefault();
                if (reversed != null)
                    value = false;

                var requestResp = tmcData.E_REQUESTLOG.Where(a => a.transid == UNIQUE_TRANSID).FirstOrDefault();
                if (requestResp == null)
                    value = false;
            }
            catch(Exception ex)
            {
                Console.WriteLine(nameof(DataManupulation) + " " + ExceptionExtensions.GetFullMessage(ex));
                logger.LogInfoMessage(nameof(DataManupulation) + " " + ExceptionExtensions.GetFullMessage(ex));
            }

            return value;

        }

        public static void RemoveTransactionFromSettlement(List<E_TRANSACTION> itemsToRemove)
        {
            EtzbkDataContext etz = new EtzbkDataContext();
            etz.E_TRANSACTION.AddRange(itemsToRemove);
            //etz.Entry(itemsToRemove).State = System.Data.Entity.EntityState.Deleted;
            itemsToRemove.ForEach(p => etz.Entry(p).State = EntityState.Deleted);
            etz.E_TRANSACTION.RemoveRange(itemsToRemove);

            bool saveFailed;
            do
            {
                saveFailed = false;
                try
                {
                    etz.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;
                    ex.Entries.Single().Reload();
                }
            } while (saveFailed);
        }

        public  static void UpdateTransactionAsProcccessed(List<E_TRANSACTION> itemsToUpdate)
        {
            itemsToUpdate.ToList().ForEach(a => a.PROCESS_STATUS = "1");
            EtzbkDataContext etz = new EtzbkDataContext();   
            etz.E_TRANSACTION.AddRange(itemsToUpdate);
            //etz.Entry(itemsToRemove).State = System.Data.Entity.EntityState.Deleted;
            itemsToUpdate.ForEach(p => etz.Entry(p).State = EntityState.Modified);

            bool saveFailed;
            do
            {
                saveFailed = false;
                try
                {
                    etz.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;
                    ex.Entries.Single().Reload();
                }
            } while (saveFailed);
        }

        public static void UpdateTMCProcccessedTransaction(string[] uniqueIDs)
        {

            using (var db = new TmcDataContext())
            {
                try
                {
                    var paramVal = string.Join("','", uniqueIDs);
                    string sql = "UPDATE E_TMCREQUEST SET STATUS ='1' WHERE TRANS_DATA IN ('" + paramVal + "')";

                    Console.WriteLine(sql);

                    db.Database.ExecuteSqlCommand(sql);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Datamanupulation " + ExceptionExtensions.GetFullMessage(ex));
                }
                finally
                {

                }
            }
           

            
            
                //db.E_TMCREQUEST.AddRange(itemsToUpdate);
                //etz.Entry(itemsToRemove).State = System.Data.Entity.EntityState.Deleted;
           
            //bool saveFailed;
            //do
            //{
            //    saveFailed = false;
            //    try
            //    {
            //        db.SaveChanges();
            //    }
            //    catch (DbUpdateConcurrencyException ex)
            //    {
            //        saveFailed = true;
            //        ex.Entries.Single().Reload();
            //    }
            //} while (saveFailed);
        }
    }

    public class Settlement
    {
        public static String GenerateBatch()
        {

            EtzbkDataContext etzbk = new EtzbkDataContext();


            string currentTime = DateTime.Now.ToString("yyMMddHHmm");
            string[] month = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J",
                "K", "L" };
            string[] day = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K",
                "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W",
                "X", "Y", "Z", "1", "2", "3", "4", "5" };
            string[] hour = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J",
                "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V",
                "W", "X" };
            string finalkey = "";
            try
            {
                int keyID = 0;
                var batchnumber = etzbk.E_FEEBATCH.FirstOrDefault();
                if (batchnumber != null)
                {
                    keyID = batchnumber.KEYID + 1;

                    if (keyID == 999998)
                        keyID = 1;
                    batchnumber = new E_FEEBATCH() { KEYID = keyID };
                    etzbk.E_FEEBATCH.Add(batchnumber);

                }
                else
                {
                    keyID = 1;
                    batchnumber = new E_FEEBATCH() { KEYID = 1 };
                    etzbk.E_FEEBATCH.Add(batchnumber);
                }

                String key = keyID.ToString("D6");
                int mm = int.Parse(currentTime.Substring(2, 2));
                int dd = int.Parse(currentTime.Substring(4, 2));
                int hh = int.Parse(currentTime.Substring(6, 2));
                if (mm == 0)
                    mm = 1;
                if (dd == 0)
                    dd = 1;
                if (hh == 0)
                    hh = 1;
                finalkey = currentTime.Substring(0, 2) + month[mm - 1]
                      + day[dd - 1] + hour[hh - 1] + currentTime.Substring(8)
                      + key;
                etzbk.SaveChanges();
            }
            catch (Exception _e)
            {
                Console.WriteLine(_e.Message);
            }
            return finalkey;
        }


        public static string SetSettleBatch()
        {
            string settleBatch = GenerateBatch();

            try
            {
                EtzbkDataContext etzbk = new EtzbkDataContext();


                E_SETTLE_BATCH batch = new E_SETTLE_BATCH() { BATCH_DATE = DateTime.Now, BATCH_ID = settleBatch, CLOSED = "0", START_DATE = DateTime.Today, END_DATE = DateTime.Today.AddDays(1).AddTicks(-1) };

                etzbk.E_SETTLE_BATCH.Add(batch);

                etzbk.SaveChanges();
            }
            catch {
            }
            return settleBatch;

        }
        public static E_SETTLE_BATCH GetSettleBatch()
        {
            E_SETTLE_BATCH settle_batch = new E_SETTLE_BATCH();
            try
            {
                EtzbkDataContext etzbk = new EtzbkDataContext();

                settle_batch = etzbk.E_SETTLE_BATCH.Where(c => c.CLOSED == "0").OrderBy(c=>c.BATCH_DATE).LastOrDefault();
                
            }
            catch { }

                return settle_batch;
        }
    }

    public static class ExceptionExtensions
    {
        public static string GetFullMessage(this Exception ex)
        {

            return ex.InnerException == null
                 ? ex.Message
                 : ex.Message + " --> " + ex.InnerException.GetFullMessage();
        }
    }
}
