﻿using AutoMapper;
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
            EtzbkDataContext db = new EtzbkDataContext();

            Task<IList<E_TRANSACTION>> t1 = Task.Factory.StartNew(NonEtzCard1);
            Task<IList<E_TRANSACTION>> t2 = Task.Factory.StartNew(NonEtzCard2);


            Console.WriteLine("  NonEtzCardTransaction waiting for Merging ");


            List<Task> taskList = new List<Task> { t1, t2 };
            Task.WaitAll(taskList.ToArray());
            Console.WriteLine("  NonEtzCardTransaction Merged");
            try
            {
                var allTmcData = new List<E_TRANSACTION>(t1.Result.Count + t2.Result.Count);


                Console.WriteLine(" Merge All Data Spooled... Removing Duplicate record");

                //Check if Data already exist on eTransactions
                var uniqueIDs = allTmcData.Select(u => u.UNIQUE_TRANSID).Distinct().ToArray();
                var uniqueIDsOnDB = db.E_TRANSACTION.Where(u => uniqueIDs.Contains(u.UNIQUE_TRANSID)).Select(u => u.UNIQUE_TRANSID).ToArray();
                var etrxData = allTmcData.Where(u => !uniqueIDsOnDB.Contains(u.UNIQUE_TRANSID));
                Console.WriteLine(etrxData.Count() + " Duplicate record removed--NonEtzCardTransaction");


                db.E_TRANSACTION.AddRange(etrxData);
                db.SaveChanges();


                Console.WriteLine(etrxData.Count() + " Record Inserted for Settlement");
                Console.WriteLine("Marking Transaction as spooled transaction");
                DataManupulation.UpdateTMCProcccessedTransaction(uniqueIDs);
                Console.WriteLine("Spooled transactions Marked");

            }
            catch (SqlException ex)
            {
                Console.WriteLine("Exception from Channel 1 " + ExceptionExtensions.GetFullMessage(ex));
                logger.LogInfoMessage(nameof(NonEtzCardTransaction) + " " + ExceptionExtensions.GetFullMessage(ex));

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception from Channel 1 " + ExceptionExtensions.GetFullMessage(ex));
                logger.LogInfoMessage(nameof(NonEtzCardTransaction) + " " + ExceptionExtensions.GetFullMessage(ex));
            }

            logger.LogInfoMessage(nameof(NonEtzCardTransaction) + " Merged ");


            Console.WriteLine("NonEtzCardTransaction Transaction spool Completed");

        }



        #region  Non etz cards
        public IList<E_TRANSACTION> NonEtzCard1()
        {
            TmcDataContext db = new TmcDataContext();
            var query = from A in db.E_TMCREQUEST
                        join B in db.E_TMCNODE
                        on A.TRANS_SEQ equals B.INCON_NAME into jointData
                        from joinRecord in jointData.DefaultIfEmpty()
                        where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && (A.TERMINAL_ID.StartsWith("4505") || A.TERMINAL_ID.StartsWith("2030")) && A.RESPONSE_CODE == "00" && A.MTI == "0200" && A.PRO_CODE.StartsWith("00"))
                        select new EtransactionViewModel
                        {
                            TRANS_CODE = "K",
                            CARD_NUM = A.PAN,
                            TRANSID = A.STAN,                            
                            MERCHANT_CODE = A.CARD_ACC_ID,
                            TRANS_DESCR = A.CARD_ACC_NAME,
                            RESPONSE_CODE=A.RESPONSE_CODE,
                            TRANS_AMOUNT = A.AMOUNT,
                            TRANS_DATE=A.TRANS_DATE,
                            CHANNELID = A.TRANS_DATA.Substring(0, 2),
                            TRANS_TYPE = "1",
                            EXTERNAL_TRANSID = A.SWITCH_KEY,
                            FEE=A.FEE,
                            REVERSAL_KEY = A.TRANS_KEY,
                            TRANS_NO = A.TRANS_SEQ,
                           UNIQUE_TRANSID =A.TRANS_DATA,
                            BANK_CODE = A.TARGET == "NIBBS_TMS" && A.PAN.Substring(1, 1) != "4" ? "032" : joinRecord.AQISSUER_CODE
                    
                        };


            var tmcreq = query.ToList().Take(Settings.number_of_record_perround);

            var e_Transaction = Mapper.Map<IList<E_TRANSACTION>>(tmcreq);
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + e_Transaction.Count());

            logger.LogInfoMessage(nameof(EtranzactChannelTransaction) + " NonEtzCardTransaction1 Completed ");
            return e_Transaction;

        }

        public IList<E_TRANSACTION> NonEtzCard2()
        {

            TmcDataContext db = new TmcDataContext();
            var query = from A in db.E_TMCREQUEST
                        join B in db.E_TMCNODE
                        on A.TRANS_SEQ equals B.INCON_NAME into jointData
                        from joinRecord in jointData.DefaultIfEmpty()
                        where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && (A.TERMINAL_ID.StartsWith("2") && !A.TERMINAL_ID.StartsWith("270") && !A.TERMINAL_ID.StartsWith("4505")) && A.RESPONSE_CODE == "00" && A.MTI == "0200" && A.PRO_CODE.StartsWith("00") && A.TARGET == "NIBBS_TMS")
                        select new EtransactionViewModel
                        {
                            TRANS_CODE = "K",
                            CARD_NUM = A.PAN,
                            TRANSID = A.STAN,
                            MERCHANT_CODE = A.ACCT_ID2,
                            TRANS_DESCR = A.CARD_ACC_NAME,
                            RESPONSE_CODE=A.RESPONSE_CODE,
                            TRANS_AMOUNT = A.AMOUNT,
                            TRANS_DATE=A.TRANS_DATE,
                            CHANNELID = A.TRANS_DATA.Substring(0, 2),
                            TRANS_TYPE = "1",
                            EXTERNAL_TRANSID = A.SWITCH_KEY,
                            UNIQUE_TRANSID = A.TRANS_DATA,
                            FEE=A.FEE,
                            REVERSAL_KEY = A.TRANS_KEY,
                            TRANS_NO = A.TRANS_SEQ,  
                            BANK_CODE = A.TARGET == "NIBBS_TMS" && A.PAN.Substring(1, 1) == "4" ? A.TERMINAL_ID.Substring(2, 3) : joinRecord.AQISSUER_CODE
                            
                        };


            var tmcreq = query.ToList().Take(Settings.number_of_record_perround);

            var e_Transaction = Mapper.Map<IList<E_TRANSACTION>>(tmcreq);
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + e_Transaction.Count());

            logger.LogInfoMessage(nameof(EtranzactChannelTransaction) + " NonEtzCardTransaction2 Completed ");
            return e_Transaction;
        }

        #endregion

    }
}
