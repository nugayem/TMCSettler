using AutoMapper;
using DALContext;
using DALContext.Model;
using LoggerHelper.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TmcOperationService
{
    public class MastercardTransactions
    {

        private Logger logger;
        public void Run()
        {
            logger = new Logger();
            

            Task<List<E_TRANSACTION>> t1 = Task.Factory.StartNew(MastercardTrx1);
            Task<List<E_TRANSACTION>> t2 = Task.Factory.StartNew(MastercardTrx2);
            Task<List<E_TRANSACTION>> t3 = Task.Factory.StartNew(MastercardTrx3);

            Console.WriteLine("  MastercardTransactions waiting for Merging ");

            List<Task> taskList = new List<Task> { t1, t2, t3 };
            Task.WaitAll(taskList.ToArray());
            Console.WriteLine("  MastercardTransactions Merged");

            using (EtzbkDataContext db = new EtzbkDataContext())
            {
                try
                {
                    var allTmcData = DataManupulation.MergeEntityList(new List<List<E_TRANSACTION>>() { t1.Result.ToList(), t2.Result.ToList(), t3.Result.ToList() });

                    Console.WriteLine(" Merge All Data Spooled... Removing Duplicate record");


                    var uniqueIDs = allTmcData.Select(u => u.UNIQUE_TRANSID).Distinct().ToArray();
                    var uniqueIDsOnDB = db.E_TRANSACTION.Where(u => uniqueIDs.Contains(u.UNIQUE_TRANSID)).Select(u => u.UNIQUE_TRANSID).ToArray();
                    var etrxData = allTmcData.Where(u => !uniqueIDsOnDB.Contains(u.UNIQUE_TRANSID));
                    Console.WriteLine(uniqueIDsOnDB.Count() + " Duplicate record removed--NonEtzCardTransaction");

                    logger.LogInfoMessage(nameof(MastercardTransactions) + " " + etrxData.Count() + " Record ready to be Inserted");
                    db.E_TRANSACTION.AddRange(etrxData);
                    db.SaveChanges();

                    logger.LogInfoMessage(nameof(MastercardTransactions) + " " + etrxData.Count() + " Record Inserted for Settlement");

                    Console.WriteLine(etrxData.Count() + " Record Inserted for Settlement");
                    Console.WriteLine("Marking Transaction as spooled transaction");
                    if (uniqueIDs.Count() > 0)
                        DataManupulation.UpdateTMCProcccessedTransaction(uniqueIDs);
                    Console.WriteLine("Spooled transactions Marked");
                    Console.WriteLine("Spooled transactions Marked");
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("SQLException from MastercardTransactions Run " + ExceptionExtensions.GetFullMessage(ex));
                    logger.LogDebugMessage("SQLException from Run " + nameof(MastercardTransactions) + " " + ExceptionExtensions.GetFullMessage(ex));

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception from Run " + ExceptionExtensions.GetFullMessage(ex));
                    logger.LogDebugMessage("Exception from Run " + nameof(MastercardTransactions) + " " + ExceptionExtensions.GetFullMessage(ex));
                }
            }
            logger.LogInfoMessage(nameof(MastercardTransactions) + " Merged ");



            Console.WriteLine("MastercardTransactions Transaction spool Completed");

        }

        #region eTz Mastercard trans
        public List<E_TRANSACTION> MastercardTrx1()
        {
            var src_node_value = new int[] { 210000059, 510000059 };

            List<E_TRANSACTION> e_Transaction = new List<E_TRANSACTION>();
            using (TmcDataContext db = new TmcDataContext())
            {
                try
                {
                    var query = from A in db.E_TMCREQUEST
                                join B in db.E_TMCHOST_RESP
                                on new { X = A.TRANS_DATA, Y = A.TRANS_SEQ } equals new { X = B.TRANS_DATA, Y = B.SWITCH_REF } into jointData
                                from joinRecord in jointData.DefaultIfEmpty()
                                join C in db.E_TMCNODE
                                on A.TARGET_NODE equals C.INCON_ID into jointData2
                                from joinRecord2 in jointData2.DefaultIfEmpty()
                                where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && src_node_value.Contains(A.SRC_NODE) && A.TARGET_NODE == 210000049 && Settings.successKeys.Contains(joinRecord.RESPONSE_CODE) && A.MTI == "0200" && A.AMOUNT > 0 && A.REVERSAL_KEY == "" && A.TRANS_DATA != "")
                                
                                select new EtransactionViewModel
                                {
                                    TRANS_CODE = "F",
                                    CARD_NUM = joinRecord2.ISSUER_CODE + A.PAN,
                                    TRANSID = joinRecord.REFERENCE,
                                    TRANS_NO = joinRecord.STAN,
                                    MERCHANT_CODE = A.CARD_ACC_ID,
                                    TRANS_DESCR = A.CARD_ACC_NAME,
                                    RESPONSE_CODE = A.RESPONSE_CODE,
                                    TRANS_AMOUNT = A.AMOUNT,
                                    TRANS_DATE = A.TRANS_DATE,
                                    CHANNELID = A.TRANS_DATA.Substring(0, 2),
                                    TRANS_TYPE = "1",
                                    EXTERNAL_TRANSID = A.SWITCH_KEY,
                                    UNIQUE_TRANSID = A.TRANS_DATA,
                                    FEE = int.Parse(joinRecord.FEE.Substring(1))/100,
                                    CURRENCY = A.CURRENCY,
                                    REVERSAL_KEY = A.REVERSAL_KEY,
                                    TERMINAL_ID = A.TERMINAL_ID,
                                    CARD_SCHEME = A.PRO_CODE,
                                    RESP_RESPONSE_CODE = joinRecord.RESPONSE_CODE,

                                };


                    var tmcreq = query.ToList().Take(Settings.number_of_record_perround);


                    e_Transaction = Mapper.Map<List<E_TRANSACTION>>(tmcreq);
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Exception from " + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ExceptionExtensions.GetFullMessage(ex));
                    logger.LogDebugMessage(nameof(MastercardTransactions) + " " + ExceptionExtensions.GetFullMessage(ex));

                }
            }
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + e_Transaction.Count());
            Console.WriteLine("Channel4 Completed");
            logger.LogInfoMessage(nameof(MastercardTransactions) + " Channel4 Completed ");

            return e_Transaction;
        }


        public List<E_TRANSACTION> MastercardTrx2()
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
                                join C in db.E_TMCNODE
                                on A.TARGET_NODE equals C.INCON_ID into jointData2
                                from joinRecord2 in jointData2.DefaultIfEmpty()
                                where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && A.SRC_NODE == 210000059 && A.TARGET_NODE == 510000063 && Settings.successKeys.Contains(A.RESPONSE_CODE) && A.MTI == "0200" && A.AMOUNT > 0 && A.REVERSAL_KEY == "" && A.TRANS_DATA != "")
                                select new EtransactionViewModel
                                {
                                    TRANS_CODE = "G",
                                    CARD_NUM = joinRecord2.ISSUER_CODE + A.ACCT_ID1.Substring(1, 3) + A.PAN.Substring(1, 6) + "XXX" + A.PAN.Substring(A.PAN.Length - 4, 4),
                                    TRANSID = A.TRANS_SEQ,
                                    TRANS_NO = A.STAN,
                                    MERCHANT_CODE = A.CARD_ACC_ID,
                                    TRANS_DESCR = A.CARD_ACC_NAME,
                                    RESPONSE_CODE = A.RESPONSE_CODE,
                                    TRANS_AMOUNT = A.AMOUNT,
                                    TRANS_DATE = A.TRANS_DATE,
                                    CHANNELID = A.TRANS_DATA.Substring(0, 2),
                                    TRANS_TYPE = "1",
                                    EXTERNAL_TRANSID = A.SWITCH_KEY,
                                    UNIQUE_TRANSID = A.TRANS_DATA,
                                    FEE = joinRecord2.INT_FEE,
                                    CURRENCY = A.CURRENCY,
                                    REVERSAL_KEY = A.REVERSAL_KEY,
                                    TERMINAL_ID = A.TERMINAL_ID,
                                    CARD_SCHEME = A.PRO_CODE,
                                    RESP_RESPONSE_CODE = A.RESPONSE_CODE
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
            Console.WriteLine("Channel4 Completed");
            logger.LogInfoMessage(nameof(EtranzactChannelTransaction) + " Channel4 Completed ");

            return e_Transaction;
        }

        public List<E_TRANSACTION> MastercardTrx3()
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
                                join C in db.E_TMCNODE
                                on A.TARGET_NODE equals C.INCON_ID into jointData2
                                from joinRecord2 in jointData2.DefaultIfEmpty()
                                where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && A.SRC_NODE == 510000064 && Settings.successKeys.Contains(A.RESPONSE_CODE) && A.MTI == "0200" && A.AMOUNT > 0 && A.REVERSAL_KEY == "" && A.TRANS_DATA != "")
                                select new EtransactionViewModel
                                {
                                    TRANS_CODE = "H",
                                    CARD_NUM = joinRecord2.AQISSUER_CODE + "SMTATMW",
                                    TRANSID = A.TRANS_SEQ,
                                    TRANS_NO = A.STAN,
                                    MERCHANT_CODE = "700602SMTW",
                                    TRANS_DESCR = A.CARD_ACC_NAME,
                                    RESPONSE_CODE = A.RESPONSE_CODE,
                                    TRANS_AMOUNT = A.AMOUNT,
                                    TRANS_DATE = A.TRANS_DATE,
                                    CHANNELID = A.TRANS_DATA.Substring(0, 2),
                                    TRANS_TYPE = "1",
                                    EXTERNAL_TRANSID = A.TRANS_KEY,
                                    FEE = A.FEE,
                                    CURRENCY = A.CURRENCY,
                                    TERMINAL_ID = A.TERMINAL_ID,
                                    CARD_SCHEME = A.PRO_CODE,
                                    RESP_RESPONSE_CODE = A.RESPONSE_CODE,


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
            Console.WriteLine("Channel4 Completed");
            logger.LogInfoMessage(nameof(EtranzactChannelTransaction) + " Channel4 Completed ");

            return e_Transaction;

        }
       #endregion
    }
}
