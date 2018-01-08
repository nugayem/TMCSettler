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
        private readonly static Logger logger = new Logger();
        private readonly static Semaphore semaphore = new Semaphore(2, 2);

        public static List<T> MergeEntityList<T>(List<List<T>> list)
        {
            int mergedSize = 0;
            foreach (var item in list)
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

        public static bool CheckTransactionStatusOnTMC(string UNIQUE_TRANSID, string TRANS_CODE)
        {
            Logger logger = new Logger();
            bool value = true;
            try
            {
                Console.WriteLine("Checking Transaction" + UNIQUE_TRANSID);

                using (TmcDataContext tmcData = new TmcDataContext())
                {
                    var reversed = tmcData.E_TMCREQUEST.Where(a => a.TRANS_DATA == UNIQUE_TRANSID && a.MTI == "0420").FirstOrDefault();
                    if (reversed != null)
                        return false;

                    var requestResp = tmcData.E_REQUESTLOG.Where(a => a.transid == UNIQUE_TRANSID).FirstOrDefault();
                    if (requestResp != null)
                        if (requestResp.response_code != "00" && requestResp.response_code != "0")
                            return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(nameof(DataManupulation) + " " + ExceptionExtensions.GetFullMessage(ex));
                logger.LogInfoMessage(nameof(DataManupulation) + " " + ExceptionExtensions.GetFullMessage(ex));
                return false;
            }

            return value;

        }

        public static void RemoveTransactionFromSettlement(List<E_TRANSACTION> itemsToRemove)
        {
            using (EtzbkDataContext etz = new EtzbkDataContext())
            {
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
        }

        public static void UpdateTransactionAsProcccessed(List<E_TRANSACTION> itemsToUpdate)
        {
            itemsToUpdate.ToList().ForEach(a => a.PROCESS_STATUS = "1");
            using (EtzbkDataContext etz = new EtzbkDataContext())
            {
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
        }

        public static void UpdateTMCProcccessedTransaction(string[] uniqueIDs)
        {
            semaphore.WaitOne();

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
                    logger.LogInfoMessage("SQLException from Run " + nameof(EtranzactChannelTransaction) + " " + ExceptionExtensions.GetFullMessage(ex));
                }
                finally
                {
                    semaphore.Release();
                }
            }


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
