using DALContext;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using AutoMapper;
using DALContext.Model;
using LoggerHelper.Services;
using System.Data.SqlClient;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;

namespace TmcOperationService
{
    public class EtranzactChannelTransaction
    {

        private Logger logger;
        public void Run()
        {
            logger = new Logger();

            logger.LogInfoMessage(nameof(EtranzactChannelTransaction) + " Starting  EtranzactChannelTransaction ");

            Task<List<E_TRANSACTION>> t1 = Task.Factory.StartNew(Channel1);
            Task<List<E_TRANSACTION>> t2 = Task.Factory.StartNew(Channel2);
            Task<List<E_TRANSACTION>> t3 = Task.Factory.StartNew(Channel3);
            Task<List<E_TRANSACTION>> t4 = Task.Factory.StartNew(Channel4);
            Task<List<E_TRANSACTION>> t5 = Task.Factory.StartNew(Channel5);
            Task<List<E_TRANSACTION>> t6 = Task.Factory.StartNew(Channel6);
            Task<List<E_TRANSACTION>> t7 = Task.Factory.StartNew(Channel7);
            Task<List<E_TRANSACTION>> t8 = Task.Factory.StartNew(Channel8);


            logger.LogInfoMessage(nameof(EtranzactChannelTransaction) + "  EtranzactChannelTransaction waiting for Merging ");
            Console.WriteLine("  EtranzactChannelTransaction waiting for Merging ");
            using (EtzbkDataContext db = new EtzbkDataContext())
            {

                try
                {
                    List<Task> taskList = new List<Task> { t1, t2, t3, t4, t5, t6, t7,t8 };
                    Task.WaitAll(taskList.ToArray());
                    Console.WriteLine("  EtranzactChannelTransaction Merged");

                    //Merge All Data Spooled
                    var allTmcData = DataManupulation.MergeEntityList(new List<List<E_TRANSACTION>>() { t1.Result.ToList(), t2.Result.ToList(), t3.Result.ToList(), t4.Result.ToList(), t5.Result.ToList(), t6.Result.ToList(), t7.Result.ToList(), t8.Result.ToList() });

                    Console.WriteLine(" Merge All Data Spooled... Removing Duplicate record");

                    //Remove duplicate value
                    var uniqueIDs = allTmcData.Select(u => u.UNIQUE_TRANSID).Distinct().ToArray();
                    var uniqueIDsOnDB = db.E_TRANSACTION.Where(u => uniqueIDs.Contains(u.UNIQUE_TRANSID)).Select(u => u.UNIQUE_TRANSID).ToArray();
                    var etrxData = allTmcData.Where(u => !uniqueIDsOnDB.Contains(u.UNIQUE_TRANSID));

                    Console.WriteLine(uniqueIDsOnDB.Count() + " Duplicate record removed--NonEtzCardTransaction");

                    logger.LogInfoMessage(nameof(EtranzactChannelTransaction) + " " + etrxData.Count() + " Record ready to be Inserted");

                    db.E_TRANSACTION.AddRange(etrxData);
                    db.SaveChanges();

                    logger.LogInfoMessage(nameof(EtranzactChannelTransaction) + " " + etrxData.Count() + " Record Inserted for Settlement");

                    Console.WriteLine(etrxData.Count() + " Record Inserted for Settlement");
                    Console.WriteLine("Marking Transaction as spooled transaction");
                    if (uniqueIDs.Count() > 0)
                        DataManupulation.UpdateTMCProcccessedTransaction(uniqueIDs);
                    Console.WriteLine("Spooled transactions Marked");

                }
                catch (SqlException ex)
                {
                    Console.WriteLine("SQLException from EtranzactChannelTransaction Run " + ExceptionExtensions.GetFullMessage(ex));
                    logger.LogDebugMessage("SQLException from Run " + nameof(EtranzactChannelTransaction) + " " + ExceptionExtensions.GetFullMessage(ex));

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception from EtranzactChannelTransaction Run " + ExceptionExtensions.GetFullMessage(ex));
                    logger.LogDebugMessage("Exception from Run " + nameof(EtranzactChannelTransaction) + " " + ExceptionExtensions.GetFullMessage(ex));
                }

                logger.LogInfoMessage(nameof(EtranzactChannelTransaction) + " Merged ");


                Console.WriteLine("ETZ Channel Transaction Completed");

                logger.LogInfoMessage(nameof(EtranzactChannelTransaction) + " ETZ Channel  Transaction Completed ");
            }
        }
        #region Spool Payment Transactions from TMC
        public List<E_TRANSACTION> Channel1()
        {

            List<E_TRANSACTION> e_Transaction = new List<E_TRANSACTION>();
            using (TmcDataContext db = new TmcDataContext())
            {
                try
                {
                    var query = from A in db.E_TMCREQUEST
                                join B in db.E_TMCHOST_RESP
                                on new { X = A.TRANS_DATA, Y = A.TRANS_SEQ } equals new { X = B.TRANS_DATA, Y = B.SWITCH_REF } into jointData
                                from joinRecord in jointData.DefaultIfEmpty()
                                where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && A.PRO_CODE.StartsWith("13") && Settings.successKeys.Contains(A.RESPONSE_CODE) && A.MTI == "0200" && Settings.targets.Contains(A.TARGET))
                                select new EtransactionViewModel
                                {
                                    TRANS_CODE = "P",
                                    CARD_NUM = A.PAN,
                                    TRANSID = A.STAN,
                                    MERCHANT_CODE = A.CARD_ACC_ID,
                                    TRANS_DESCR = A.CARD_ACC_NAME,
                                    RESPONSE_CODE = A.RESPONSE_CODE,
                                    TRANS_AMOUNT = A.AMOUNT,
                                    TRANS_DATE = A.TRANS_DATE,
                                    CHANNELID = A.TRANS_DATA.Substring(0, 2),
                                    TRANS_TYPE = "1",
                                    EXTERNAL_TRANSID = A.TRANS_DATA,
                                    UNIQUE_TRANSID = A.TRANS_DATA,
                                    FEE = A.FEE,
                                    CURRENCY = A.CURRENCY,
                                    REVERSAL_KEY = A.TRANS_KEY,
                                    TRANS_NO = A.TRANS_SEQ,
                                    TERMINAL_ID = A.TERMINAL_ID,
                                    CARD_SCHEME = A.CARD_SCHEME,
                                    REFERENCE = joinRecord.REFERENCE,
                                    RESP_RESPONSE_CODE = joinRecord.RESPONSE_CODE

                                };

                    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                    stopwatch.Start();

                    var tmcreq = query.ToList().Take(Settings.number_of_record_perround);

                    stopwatch.Stop();
                    Console.WriteLine("Inspectingtime for channel 1 completed in " + stopwatch.Elapsed);

                    e_Transaction = Mapper.Map<List<E_TRANSACTION>>(tmcreq);
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Exception from " + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ExceptionExtensions.GetFullMessage(ex));
                    logger.LogDebugMessage(nameof(EtranzactChannelTransaction) + " " + ExceptionExtensions.GetFullMessage(ex));

                }
            }
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + e_Transaction.Count());
            Console.WriteLine("Channel1 Completed");
            logger.LogInfoMessage(nameof(EtranzactChannelTransaction) + " Channel1 Completed ");

            return e_Transaction;
        }
        public List<E_TRANSACTION> Channel2()
        {
            List<E_TRANSACTION> e_Transaction = new List<E_TRANSACTION>();
            using (TmcDataContext db = new TmcDataContext())
            {
                try

                {
                    var query = from A in db.E_TMCREQUEST
                                join B in db.E_TMCHOST_RESP
                                on new { X = A.TRANS_DATA, Y = A.TRANS_SEQ } equals new { X = B.TRANS_DATA, Y = B.SWITCH_REF } into jointData
                                from joinRecord in jointData.DefaultIfEmpty()
                                where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && A.PAN.Contains("SWI") && A.PRO_CODE.StartsWith("4") && Settings.successKeys.Contains(joinRecord.RESPONSE_CODE) && A.MTI == "0900" && Settings.targets.Contains(A.TARGET))
                                select new EtransactionViewModel
                                {
                                    TRANS_CODE = "T",
                                    CARD_NUM = A.PAN,
                                    TRANSID = joinRecord.REFERENCE == null || joinRecord.REFERENCE ==""? A.STAN : joinRecord.REFERENCE,
                                    MERCHANT_CODE = A.ACCT_ID2,
                                    TRANS_DESCR = A.CARD_ACC_NAME,
                                    RESPONSE_CODE = A.RESPONSE_CODE,
                                    TRANS_AMOUNT = A.AMOUNT,
                                    TRANS_DATE = A.TRANS_DATE,
                                    CHANNELID = A.TRANS_DATA.Substring(0, 2),
                                    TRANS_TYPE = "1",
                                    EXTERNAL_TRANSID = A.TRANS_DATA +"-"+ joinRecord.REFERENCE,
                                    UNIQUE_TRANSID = A.TRANS_DATA,
                                    FEE = A.FEE,
                                    CURRENCY = A.CURRENCY,
                                    REVERSAL_KEY = A.TRANS_KEY,
                                    TRANS_NO = A.TRANS_SEQ,
                                    TERMINAL_ID = A.TERMINAL_ID,
                                    CARD_SCHEME = A.CARD_SCHEME,
                                    REFERENCE = joinRecord.REFERENCE,
                                    RESP_RESPONSE_CODE = joinRecord.RESPONSE_CODE

                                };
                    var tmcreq = query.ToList().Take(Settings.number_of_record_perround);

                    e_Transaction = Mapper.Map<List<E_TRANSACTION>>(tmcreq);
                }

                catch (Exception ex)
                {

                    Console.WriteLine("Exception from " + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ExceptionExtensions.GetFullMessage(ex));
                    logger.LogDebugMessage(nameof(EtranzactChannelTransaction) + " " + ExceptionExtensions.GetFullMessage(ex));

                }
            }
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + e_Transaction.Count());
            Console.WriteLine("Channel2 Completed");
            logger.LogInfoMessage(nameof(EtranzactChannelTransaction) + " Channel2 Completed ");

            return e_Transaction;
        }
        public List<E_TRANSACTION> Channel3()
        {

            List<E_TRANSACTION> e_Transaction = new List<E_TRANSACTION>();
            using (TmcDataContext db = new TmcDataContext())
            {
                try
                {
                    var query = from A in db.E_TMCREQUEST
                                join B in db.E_TMCHOST_RESP
                                on new { X = A.TRANS_DATA, Y = A.TRANS_SEQ } equals new { X = B.TRANS_DATA, Y = B.SWITCH_REF } into jointData
                                from joinRecord in jointData.DefaultIfEmpty()
                                where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && /* A.card_acc_id LIKE: Bank_Code AND */ A.PRO_CODE.StartsWith("4") && Settings.successKeys.Contains(joinRecord.RESPONSE_CODE) && A.MTI == "0900" && A.TRANS_DATA.EndsWith("XP") && Settings.targets.Contains(A.TARGET))
                                select new EtransactionViewModel
                                {
                                    TRANS_CODE = "T",
                                    CARD_NUM = A.PAN,
                                    TRANSID = joinRecord.REFERENCE == null || joinRecord.REFERENCE == "" ? A.STAN : joinRecord.REFERENCE,
                                    MERCHANT_CODE = A.ACCT_ID2,
                                    TRANS_DESCR = A.CARD_ACC_NAME,
                                    RESPONSE_CODE = A.RESPONSE_CODE,
                                    TRANS_AMOUNT = A.AMOUNT,
                                    TRANS_DATE = A.TRANS_DATE,
                                    CHANNELID = A.TRANS_DATA.Substring(0, 2),
                                    TRANS_TYPE = "1",
                                    EXTERNAL_TRANSID = A.TRANS_DATA,
                                    UNIQUE_TRANSID = A.TRANS_DATA,
                                    FEE = A.FEE,
                                    CURRENCY = A.CURRENCY,
                                    REVERSAL_KEY = A.TRANS_KEY,
                                    TRANS_NO = A.TRANS_SEQ,
                                    TERMINAL_ID = A.TERMINAL_ID,
                                    CARD_SCHEME = A.CARD_SCHEME,
                                    REFERENCE = joinRecord.REFERENCE,
                                    RESP_RESPONSE_CODE = joinRecord.RESPONSE_CODE

                                };
                    var tmcreq = query.ToList().Take(Settings.number_of_record_perround);

                    e_Transaction = Mapper.Map<List<E_TRANSACTION>>(tmcreq);
                    //EtzbkDataContext etzbkData = new EtzbkDataContext();
                    //etzbkData.E_TRANSACTION.AddRange(e_Transaction);

                }
                catch (Exception ex)
                {

                    Console.WriteLine("Exception from " + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ExceptionExtensions.GetFullMessage(ex));
                    logger.LogDebugMessage(nameof(EtranzactChannelTransaction) + " " + ExceptionExtensions.GetFullMessage(ex));

                }
            }
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + e_Transaction.Count());
            Console.WriteLine("Channel3 Completed");
            logger.LogInfoMessage(nameof(EtranzactChannelTransaction) + " Channel3 Completed ");

            return e_Transaction;
        }
        public List<E_TRANSACTION> Channel4()
        {
            List<E_TRANSACTION> e_Transaction = new List<E_TRANSACTION>();
            using (TmcDataContext db = new TmcDataContext())
            {
                try
                {

                    var query = from A in db.E_TMCREQUEST
                                join B in db.E_TMCHOST_RESP
                                on new { X = A.TRANS_DATA, Y = A.TRANS_SEQ } equals new { X = B.TRANS_DATA, Y = B.SWITCH_REF } into jointData
                                from joinRecord in jointData.DefaultIfEmpty()
                                where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && /* A.card_acc_id LIKE: Bank_Code AND */ A.PRO_CODE.StartsWith("4") && Settings.successKeys.Contains(A.RESPONSE_CODE) && A.MTI == "0200" && Settings.targets.Contains(A.TARGET))
                                select new EtransactionViewModel
                                {
                                    TRANS_CODE = "T",
                                    CARD_NUM = A.PAN,
                                    TRANSID = A.STAN,
                                    MERCHANT_CODE = A.ACCT_ID2,
                                    TRANS_DESCR = A.CARD_ACC_NAME,
                                    RESPONSE_CODE = A.RESPONSE_CODE,
                                    TRANS_AMOUNT = A.AMOUNT,
                                    TRANS_DATE = A.TRANS_DATE,
                                    CHANNELID = A.TRANS_DATA.Substring(0, 2),
                                    TRANS_TYPE = "1",
                                    EXTERNAL_TRANSID = A.TRANS_DATA,
                                    UNIQUE_TRANSID = A.TRANS_DATA,
                                    FEE = A.FEE,
                                    CURRENCY = A.CURRENCY,
                                    REVERSAL_KEY = A.TRANS_KEY,
                                    TRANS_NO = A.TRANS_SEQ,
                                    TERMINAL_ID = A.TERMINAL_ID,
                                    CARD_SCHEME = A.CARD_SCHEME,
                                    REFERENCE = joinRecord.REFERENCE,
                                    RESP_RESPONSE_CODE = joinRecord.RESPONSE_CODE

                                };


                    var tmcreq = query.ToList().Take(Settings.number_of_record_perround);

                    Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + tmcreq.Count());


                    e_Transaction = Mapper.Map<List<E_TRANSACTION>>(tmcreq);
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Exception from " + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ExceptionExtensions.GetFullMessage(ex));
                    logger.LogDebugMessage(nameof(EtranzactChannelTransaction) + " " + ExceptionExtensions.GetFullMessage(ex));

                }
            }
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + e_Transaction.Count());
            Console.WriteLine("Channel4 Completed");
            logger.LogInfoMessage(nameof(EtranzactChannelTransaction) + " Channel4 Completed ");

            return e_Transaction;

        }
        public List<E_TRANSACTION> Channel5()
        {

            List<E_TRANSACTION> e_Transaction = new List<E_TRANSACTION>();
            using (TmcDataContext db = new TmcDataContext())
            {
                try
                {
                    var query = from A in db.E_TMCREQUEST
                                join B in db.E_TMCNODE
                                on A.SRC_NODE equals B.INCON_ID into jointData
                                from joinRecord in jointData.DefaultIfEmpty()
                                where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && /* A.card_acc_id LIKE: Bank_Code AND */ A.PRO_CODE.StartsWith("01") && Settings.successKeys.Contains(A.RESPONSE_CODE) && A.MTI == "0200" && Settings.targets.Contains(A.TARGET))
                                select new EtransactionViewModel
                                {
                                    TRANS_CODE = "W",
                                    CARD_NUM = A.CARD_SCHEME+"XXXXXX"+ A.PAN.Substring(A.PAN.Length-4),
                                    TRANSID = A.STAN,
                                    MERCHANT_CODE = joinRecord.AQISSUER_CODE + "ATMWHDR",
                                    TRANS_DESCR = "WITHDRAWAL: " + A.STAN+ "000000 :" + A.TERMINAL_ID + ":" + A.CARD_ACC_NAME,
                                    RESPONSE_CODE = A.RESPONSE_CODE,
                                    TRANS_AMOUNT = A.AMOUNT,
                                    TRANS_DATE = A.TRANS_DATE,
                                    CHANNELID = A.TRANS_DATA.Substring(0, 2),
                                    TRANS_TYPE = "1",
                                    EXTERNAL_TRANSID = A.REVERSAL_KEY,
                                    UNIQUE_TRANSID = A.TRANS_DATA,
                                    FEE = A.FEE,
                                    CURRENCY = A.CURRENCY,
                                    REVERSAL_KEY = A.TRANS_KEY,
                                    TRANS_NO = A.TRANS_SEQ,
                                    TERMINAL_ID = A.TERMINAL_ID,
                                    CARD_SCHEME = A.CARD_SCHEME,
                                    REFERENCE = A.REFERENCE,
                                    RESP_RESPONSE_CODE = ""

                                };
                    var tmcreq = query.ToList().Take(Settings.number_of_record_perround);

                    e_Transaction = Mapper.Map<List<E_TRANSACTION>>(tmcreq);
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Exception from " + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ExceptionExtensions.GetFullMessage(ex));
                    logger.LogDebugMessage(nameof(EtranzactChannelTransaction) + " " + ExceptionExtensions.GetFullMessage(ex));

                }
            }
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + e_Transaction.Count());
            Console.WriteLine("Channel5 Completed");
            logger.LogInfoMessage(nameof(EtranzactChannelTransaction) + " Channel5 Completed ");

            return e_Transaction;

        }
        public List<E_TRANSACTION> Channel6()
        {
            List<E_TRANSACTION> e_Transaction = new List<E_TRANSACTION>();
            using (TmcDataContext db = new TmcDataContext())
            {
                try
                {
                    var query = from A in db.E_TMCREQUEST
                                join B in db.E_TMCHOST_RESP
                                on new { X = A.TRANS_DATA, Y = A.TRANS_SEQ } equals new { X = B.TRANS_DATA, Y = B.SWITCH_REF } into jointData
                                from joinRecord in jointData.DefaultIfEmpty()
                                where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && /* A.card_acc_id LIKE: Bank_Code AND */ A.PRO_CODE.StartsWith("40") && Settings.successKeys.Contains(A.RESPONSE_CODE) && A.MTI == "0220" && A.CARD_ACC_ID.Trim().Length < 11 && Settings.targets.Contains(A.TARGET))
                                select new EtransactionViewModel
                                {
                                    TRANS_CODE = "P",
                                    CARD_NUM = A.PAN,
                                    TRANSID = A.REFERENCE,
                                    MERCHANT_CODE = A.CARD_ACC_ID,
                                    TRANS_DESCR = A.CARD_ACC_NAME,
                                    RESPONSE_CODE = A.RESPONSE_CODE,
                                    TRANS_AMOUNT = A.AMOUNT,
                                    TRANS_DATE = A.TRANS_DATE,
                                    CHANNELID = A.TRANS_DATA.Substring(0, 2),
                                    TRANS_TYPE = "1",
                                    EXTERNAL_TRANSID = A.TRANS_DATA,
                                    UNIQUE_TRANSID = A.TRANS_DATA,
                                    FEE = A.FEE,
                                    CURRENCY = A.CURRENCY,
                                    REVERSAL_KEY = A.TRANS_KEY,
                                    TRANS_NO = A.BILLER_ID.Substring(A.BILLER_ID.IndexOf("#")+1),
                                    TERMINAL_ID = A.BILL_ID,
                                    CARD_SCHEME = A.CARD_SCHEME,
                                    REFERENCE = joinRecord.REFERENCE,
                                    RESP_RESPONSE_CODE = joinRecord.RESPONSE_CODE
                                    
                                };
                    var tmcreq = query.ToList().Take(Settings.number_of_record_perround);


                    e_Transaction = Mapper.Map<List<E_TRANSACTION>>(tmcreq);
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Exception from " + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ExceptionExtensions.GetFullMessage(ex));
                    logger.LogDebugMessage(nameof(EtranzactChannelTransaction) + " " + ExceptionExtensions.GetFullMessage(ex));

                }
            }
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + e_Transaction.Count());
            Console.WriteLine("Channel6 Completed");
            logger.LogInfoMessage(nameof(EtranzactChannelTransaction) + " Channel6 Completed ");

            return e_Transaction;

        }
        public List<E_TRANSACTION> Channel7()
        {
            List<E_TRANSACTION> e_Transaction = new List<E_TRANSACTION>();
            using (TmcDataContext db = new TmcDataContext())
            {
                try
                {
                    var query = from A in db.E_TMCREQUEST
                                join B in db.E_TMCHOST_RESP
                                on new { X = A.TRANS_DATA, Y = A.TRANS_SEQ } equals new { X = B.TRANS_DATA, Y = B.SWITCH_REF } into jointData
                                from joinRecord in jointData.DefaultIfEmpty()
                                where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && /* A.card_acc_id LIKE: Bank_Code AND */ A.PRO_CODE.StartsWith("40") && Settings.successKeys.Contains(joinRecord.RESPONSE_CODE) && A.MTI == "0220" && A.CARD_ACC_ID.Trim().Length > 11 && Settings.targets.Contains(A.TARGET))
                                select new EtransactionViewModel
                                {
                                    TRANS_CODE = A.CARD_ACC_ID.EndsWith("D") ? "D" : "U",
                                    CARD_NUM = A.CARD_ACC_ID.EndsWith("D") ? A.PAN : A.BILLER_ID.Substring(A.BILLER_ID.Length - DbFunctions.Reverse( A.BILLER_ID).IndexOf("#") ),
                                    TRANSID = A.REFERENCE,
                                    MERCHANT_CODE = A.CARD_ACC_ID.EndsWith("D") ? A.BILLER_ID.Substring(A.BILLER_ID.Length - DbFunctions.Reverse(A.BILLER_ID).IndexOf("#")) : A.PAN,
                                    TRANS_DESCR = A.CARD_ACC_NAME,
                                    RESPONSE_CODE = A.RESPONSE_CODE,
                                    TRANS_AMOUNT = A.AMOUNT,
                                    TRANS_DATE = A.TRANS_DATE,
                                    CHANNELID = A.TRANS_DATA.Substring(0, 2),
                                    TRANS_TYPE = "1",
                                    EXTERNAL_TRANSID = A.TRANS_DATA.Substring(3, A.TRANS_DATA.Length/2),
                                    UNIQUE_TRANSID = A.TRANS_DATA,
                                    FEE = A.FEE,
                                    CURRENCY = A.CURRENCY,
                                    REVERSAL_KEY = A.TRANS_KEY,
                                    TRANS_NO = A.BILLER_ID.Substring(A.BILLER_ID.IndexOf("#"), A.BILLER_ID.Length - DbFunctions.Reverse(A.BILLER_ID).IndexOf("#")),
                                    TERMINAL_ID = A.TERMINAL_ID,
                                    CARD_SCHEME = A.CARD_SCHEME,
                                    REFERENCE = joinRecord.REFERENCE,
                                    RESP_RESPONSE_CODE = joinRecord.RESPONSE_CODE

                                };
                    var tmcreq = query.ToList().Take(Settings.number_of_record_perround);
                    e_Transaction = Mapper.Map<List<E_TRANSACTION>>(tmcreq);
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Exception from " + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ExceptionExtensions.GetFullMessage(ex));
                    logger.LogDebugMessage(nameof(EtranzactChannelTransaction) + " " + ExceptionExtensions.GetFullMessage(ex));

                }
            }
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + e_Transaction.Count());
            logger.LogInfoMessage(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + e_Transaction.Count());
            Console.WriteLine("Channel7 Completed");
            logger.LogInfoMessage(nameof(EtranzactChannelTransaction) + " Channel7 Completed ");

            return e_Transaction;

        }
        public List<E_TRANSACTION> Channel8()
        {
            List<E_TRANSACTION> e_Transaction = new List<E_TRANSACTION>();
            using (TmcDataContext db = new TmcDataContext())
            {
                try
                {
                    var query = from A in db.E_TMCREQUEST
                                join B in db.E_TMCHOST_RESP
                                on new { X = A.TRANS_DATA, Y = A.TRANS_SEQ } equals new { X = B.TRANS_DATA, Y = B.SWITCH_REF } into jointData
                                from joinRecord in jointData.DefaultIfEmpty()
                                where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && (A.PAN.StartsWith("700001HOM") || A.PAN.StartsWith("700001INS")) && A.PRO_CODE.StartsWith("4") && Settings.successKeys.Contains(A.RESPONSE_CODE) && A.MTI == "0900" && Settings.targets.Contains(A.TARGET))
                                select new EtransactionViewModel
                                {
                                    TRANS_CODE = "T",
                                    CARD_NUM = "700001HOMESEND00000",
                                    TRANSID = A.STAN,
                                    MERCHANT_CODE = A.ACCT_ID2,
                                    TRANS_DESCR = A.CARD_ACC_NAME,
                                    RESPONSE_CODE = A.RESPONSE_CODE,
                                    TRANS_AMOUNT = A.AMOUNT,
                                    TRANS_DATE = A.TRANS_DATE,
                                    CHANNELID = A.TRANS_DATA.Substring(0, 2),
                                    TRANS_TYPE = "1",
                                    EXTERNAL_TRANSID = A.TRANS_DATA,
                                    UNIQUE_TRANSID = A.TRANS_DATA,
                                    FEE = A.FEE,
                                    CURRENCY = A.CURRENCY,
                                    REVERSAL_KEY = A.TRANS_KEY,
                                    TRANS_NO = A.TRANS_SEQ,
                                    TERMINAL_ID = A.TERMINAL_ID,
                                    CARD_SCHEME = A.CARD_SCHEME,
                                    REFERENCE = joinRecord.REFERENCE,
                                    RESP_RESPONSE_CODE = joinRecord.RESPONSE_CODE

                                };
                    var tmcreq = query.ToList().Take(Settings.number_of_record_perround);

                    e_Transaction = Mapper.Map<List<E_TRANSACTION>>(tmcreq);
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Exception from " + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ExceptionExtensions.GetFullMessage(ex));
                    logger.LogDebugMessage(nameof(EtranzactChannelTransaction) + " " + ExceptionExtensions.GetFullMessage(ex));

                }
            }
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + e_Transaction.Count());
            logger.LogInfoMessage(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + e_Transaction.Count());
            Console.WriteLine("Channel8 Completed");
            logger.LogInfoMessage(nameof(EtranzactChannelTransaction) + " Channel8 Completed ");

            return e_Transaction;


        }
        #endregion
    }
}
