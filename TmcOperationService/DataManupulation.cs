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
        private readonly static Semaphore checkSemaphore = new Semaphore(30, 30);
        private readonly static Semaphore updateSemaphore = new Semaphore(2, 2);

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

        public static SettleOption CheckTransactionStatusOnTMC(string UNIQUE_TRANSID, string TRANS_CODE)
        {
            Logger logger = new Logger();

            try
            {
                checkSemaphore.WaitOne();

                Console.WriteLine("Checking Transaction" + UNIQUE_TRANSID);

                using (TmcDataContext tmcData = new TmcDataContext())
                {
                    var reversed = tmcData.E_TMCREQUEST.Where(a => a.TRANS_DATA == UNIQUE_TRANSID && a.MTI == "0420").FirstOrDefault();
                    if (reversed != null)
                        return SettleOption.Invalid;

                    var requestResp = tmcData.E_REQUESTLOG.Where(a => a.transid == UNIQUE_TRANSID).FirstOrDefault();
                    if (requestResp != null)
                        if (requestResp.response_code != "00" && requestResp.response_code != "0")
                            return SettleOption.Invalid;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(nameof(DataManupulation) + " " + ExceptionExtensions.GetFullMessage(ex));
                logger.LogInfoMessage(nameof(DataManupulation) + " " + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ExceptionExtensions.GetFullMessage(ex));
                return SettleOption.Valid;
            }

            finally
            {
                checkSemaphore.Release();
            }


            return SettleOption.Valid;
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

                        Console.WriteLine(nameof(DataManupulation) + " " + ExceptionExtensions.GetFullMessage(ex));
                        logger.LogInfoMessage(nameof(DataManupulation) + " " + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ExceptionExtensions.GetFullMessage(ex));

                    }
                } while (saveFailed);
            }
        }

        public static void UpdateTMCProcccessedTransaction(string[] uniqueIDs)
        {
            updateSemaphore.WaitOne();

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
                    Console.WriteLine(nameof(DataManupulation) + " " + ExceptionExtensions.GetFullMessage(ex));
                    logger.LogInfoMessage(nameof(DataManupulation) + " " + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ExceptionExtensions.GetFullMessage(ex));
                }
                finally
                {
                    updateSemaphore.Release();
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
