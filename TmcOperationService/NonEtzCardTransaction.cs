using AutoMapper;
using DALContext;
using DALContext.Model;
using LoggerHelper.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TmcOperationService
{
    public class NonEtzCardTransaction
    {


        private Logger logger;
        public void Run()
        {
            logger = new Logger();
           

            Task<List<E_TRANSACTION>> t1 = Task.Factory.StartNew(NonEtzCard1);
            Task<List<E_TRANSACTION>> t2 = Task.Factory.StartNew(NonEtzCard2);


            Console.WriteLine("  NonEtzCardTransaction waiting for Merging ");


            List<Task> taskList = new List<Task> { t1, t2 };
            Task.WaitAll(taskList.ToArray());
            Console.WriteLine("  NonEtzCardTransaction Merged");
            using (EtzbkDataContext db = new EtzbkDataContext())
            {
                try
                {
                    var allTmcData = DataManupulation.MergeEntityList(new List<List<E_TRANSACTION>>() { t1.Result.ToList(), t2.Result.ToList() });


                    Console.WriteLine(" Merge All Data Spooled... Removing Duplicate record");

                    //Check if Data already exist on eTransactions
                    var uniqueIDs = allTmcData.Select(u => u.UNIQUE_TRANSID).Distinct().ToArray();
                    var uniqueIDsOnDB = db.E_TRANSACTION.Where(u => uniqueIDs.Contains(u.UNIQUE_TRANSID)).Select(u => u.UNIQUE_TRANSID).ToArray();
                    var etrxData = allTmcData.Where(u => !uniqueIDsOnDB.Contains(u.UNIQUE_TRANSID));
                    Console.WriteLine(uniqueIDsOnDB.Count() + " Duplicate record removed--NonEtzCardTransaction");

                    logger.LogInfoMessage(nameof(NonEtzCardTransaction) + " " + etrxData.Count() + " Record ready to be Inserted");

                    db.E_TRANSACTION.AddRange(etrxData);
                    db.SaveChanges();

                    logger.LogInfoMessage(nameof(NonEtzCardTransaction) + " " + etrxData.Count() + " Record Inserted for Settlement");

                    Console.WriteLine(etrxData.Count() + " Record Inserted for Settlement");
                    Console.WriteLine("Marking Transaction as spooled transaction");
                    if (uniqueIDs.Count() > 0)
                        DataManupulation.UpdateTMCProcccessedTransaction(uniqueIDs);
                    Console.WriteLine("Spooled transactions Marked");

                }
                catch (SqlException ex)
                {
                    Console.WriteLine("SQLException from NonEtzCardTransaction Run " + ExceptionExtensions.GetFullMessage(ex));
                    logger.LogInfoMessage("SQLException from Run " + nameof(NonEtzCardTransaction) + " " + ExceptionExtensions.GetFullMessage(ex));

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception from NonEtzCardTransaction run " + ExceptionExtensions.GetFullMessage(ex));
                    logger.LogInfoMessage("Exception from Run " + nameof(NonEtzCardTransaction) + " " + ExceptionExtensions.GetFullMessage(ex));
                }
            }
            logger.LogInfoMessage(nameof(NonEtzCardTransaction) + " Merged ");


            Console.WriteLine("NonEtzCardTransaction Transaction spool Completed");

        }



        #region  Non etz cards
        public List<E_TRANSACTION> NonEtzCard1()
        {
            List<E_TRANSACTION> e_Transaction = new List<E_TRANSACTION>();
            using (TmcDataContext db = new TmcDataContext())
            {
                try
                {
                    var query = from A in db.E_TMCREQUEST
                                join B in db.E_TMCNODE
                                on A.TRANS_SEQ equals B.INCON_NAME into jointData
                                from joinRecord in jointData.DefaultIfEmpty()
                                where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && (A.TERMINAL_ID.StartsWith("4505") || A.TERMINAL_ID.StartsWith("2030")) && A.RESPONSE_CODE == "00" && A.MTI == "0200" && A.PRO_CODE.StartsWith("00"))
                                select new EtransactionViewModel
                                {
                                    TRANS_CODE = "P",
                                    CARD_NUM = A.PAN,
                                    TRANSID = A.STAN,
                                    MERCHANT_CODE = A.CARD_ACC_ID,
                                    TRANS_DESCR = "Payment to " + A.CARD_ACC_ID +" - "+ A.TRANS_DATA.Substring(A.TRANS_DATA.IndexOf("#")+1),
                                    RESPONSE_CODE = A.RESPONSE_CODE,
                                    TRANS_AMOUNT = A.AMOUNT,
                                    TRANS_DATE = A.TRANS_DATE,
                                    CHANNELID = A.TRANS_DATA.Substring(0, 2),
                                    TRANS_TYPE = "1",
                                    EXTERNAL_TRANSID = A.SWITCH_KEY,
                                    FEE = A.FEE,
                                    CURRENCY = A.CURRENCY,
                                    REVERSAL_KEY = A.TRANS_KEY,
                                    TRANS_NO = A.TRANS_SEQ,
                                    UNIQUE_TRANSID = A.TRANS_DATA,//.Substring(0, A.TRANS_DATA.IndexOf("#")),
                                    BANK_CODE = A.TARGET == "NIBBS_TMS" && A.PAN.Substring(1, 1) != "4" ? "032" : joinRecord.AQISSUER_CODE

                                };


                    var tmcreq = query.ToList().Take(Settings.number_of_record_perround);


                    e_Transaction = Mapper.Map<List<E_TRANSACTION>>(tmcreq);
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Exception from " + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ExceptionExtensions.GetFullMessage(ex));
                    logger.LogInfoMessage(nameof(NonEtzCardTransaction) + " " + ExceptionExtensions.GetFullMessage(ex));

                }
            }
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + e_Transaction.Count());
            Console.WriteLine("NonEtzCard1 Completed");
            logger.LogInfoMessage(nameof(NonEtzCardTransaction) + " NonEtzCard1 Completed ");

            return e_Transaction;

        }

        public List<E_TRANSACTION> NonEtzCard2()
        {

            List<E_TRANSACTION> e_Transaction = new List<E_TRANSACTION>();
            using (TmcDataContext db = new TmcDataContext())
            {
                try
                {
                    var query = from A in db.E_TMCREQUEST
                                join B in db.E_TMCNODE
                                on A.TRANS_SEQ equals B.INCON_NAME into jointData
                                from joinRecord in jointData.DefaultIfEmpty()
                                where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && (A.TERMINAL_ID.StartsWith("2") && !A.TERMINAL_ID.StartsWith("270") && !A.TERMINAL_ID.StartsWith("4505")) && A.RESPONSE_CODE == "00" && A.MTI == "0200" && A.PRO_CODE.StartsWith("00") && A.TARGET == "NIBBS_TMS")
                                select new EtransactionViewModel
                                {
                                    TRANS_CODE = "P",
                                    CARD_NUM = A.PAN,
                                    TRANSID = A.STAN,
                                    MERCHANT_CODE = A.ACCT_ID2,
                                    TRANS_DESCR = "Payment to " + A.CARD_ACC_ID + " - " + A.TRANS_DATA.Substring(A.TRANS_DATA.IndexOf("#") + 1),
                                    RESPONSE_CODE = A.RESPONSE_CODE,
                                    TRANS_AMOUNT = A.AMOUNT,
                                    TRANS_DATE = A.TRANS_DATE,
                                    CHANNELID = A.TRANS_DATA.Substring(0, 2),
                                    TRANS_TYPE = "1",
                                    EXTERNAL_TRANSID = A.SWITCH_KEY,
                                    UNIQUE_TRANSID = A.TRANS_DATA,
                                    FEE = A.FEE,
                                    CURRENCY = A.CURRENCY,
                                    REVERSAL_KEY = A.TRANS_KEY,
                                    TRANS_NO = A.TRANS_SEQ,
                                    BANK_CODE = A.TARGET == "NIBBS_TMS" && A.PAN.Substring(1, 1) == "4" ? A.TERMINAL_ID.Substring(2, 3) : joinRecord.AQISSUER_CODE

                                };


                    var tmcreq = query.ToList().Take(Settings.number_of_record_perround);


                    e_Transaction = Mapper.Map<List<E_TRANSACTION>>(tmcreq);
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Exception from " + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ExceptionExtensions.GetFullMessage(ex));
                    logger.LogInfoMessage(nameof(NonEtzCardTransaction) + " " + ExceptionExtensions.GetFullMessage(ex));

                }
            }
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + e_Transaction.Count());
            Console.WriteLine("NonEtzCard2 Completed");
            logger.LogInfoMessage(nameof(NonEtzCardTransaction) + " NonEtzCard2 Completed ");

            return e_Transaction;
        }

        #endregion

    }
}