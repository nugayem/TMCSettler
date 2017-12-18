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

namespace TmcOperationService
{
   public  class EtranzactChannelTransaction 
    {

        private Logger logger;
        public void Run()
        {
                logger = new Logger();
                EtzbkDataContext db = new EtzbkDataContext();


                logger.LogInfoMessage(nameof(EtranzactChannelTransaction) + " Starting  EtranzactChannelTransaction ");

                Task<IList<E_TRANSACTION>> t1 = Task.Factory.StartNew(Channel1);
                Task<IList<E_TRANSACTION>> t2 = Task.Factory.StartNew(Channel2);
                Task<IList<E_TRANSACTION>> t3 = Task.Factory.StartNew(Channel3);
                Task<IList<E_TRANSACTION>> t4 = Task.Factory.StartNew(Channel4);
                Task<IList<E_TRANSACTION>> t5 = Task.Factory.StartNew(Channel5);
                Task<IList<E_TRANSACTION>> t6 = Task.Factory.StartNew(Channel6);
                Task<IList<E_TRANSACTION>> t7 = Task.Factory.StartNew(Channel7);
                Task<IList<E_TRANSACTION>> t8 = Task.Factory.StartNew(Channel8);


                logger.LogInfoMessage(nameof(EtranzactChannelTransaction) + "  EtranzactChannelTransaction waiting for Merging ");

            try
            {
                List<Task> taskList = new List<Task> { t1, t2, t3, t4, t5, t6, t7, t8 };
                Task.WaitAll(taskList.ToArray());
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception" + ex.Message);
            }
                logger.LogInfoMessage(nameof(EtranzactChannelTransaction) + " Merged ");


            //Merge All Data Spooled
            var allTmcData = DataManupulation.MergeEntityList(new List<List<E_TRANSACTION>>() { t1.Result.ToList(), t2.Result.ToList(), t3.Result.ToList(), t4.Result.ToList(), t5.Result.ToList(),t6.Result.ToList(),t7.Result.ToList(),t8.Result.ToList() });
//            var allTmcData = new List<E_TRANSACTION>(t1.Result.Count +t2.Result.Count + t3.Result.Count+ t4.Result.Count + t5.Result.Count+ t6.Result.Count+ t7.Result.Count+ t8.Result.Count);

            //allTmcData.AddRange(t1.Result);
            //allTmcData.AddRange(t2.Result);
            //allTmcData.AddRange(t3.Result);
            //allTmcData.AddRange(t4.Result);
            //allTmcData.AddRange(t5.Result);
            //allTmcData.AddRange(t6.Result);
            //allTmcData.AddRange(t7.Result);
            //allTmcData.AddRange(t8.Result);


            //Check if Data already exist on eTransactions
            
            var uniqueIDs = allTmcData.Select(u => u.UNIQUE_TRANSID).Distinct().ToArray();
            var uniqueIDsOnDB = db.E_TRANSACTION.Where(u => uniqueIDs.Contains(u.UNIQUE_TRANSID)).Select(u=>u.UNIQUE_TRANSID).ToArray();
            var etrxData = allTmcData.Where(u => !uniqueIDsOnDB.Contains(u.UNIQUE_TRANSID));

            db.E_TRANSACTION.AddRange(etrxData);
            db.SaveChanges();


            /*
            db.E_TRANSACTION.AddRange(t1.Result);
                db.E_TRANSACTION.AddRange(t2.Result);
                db.E_TRANSACTION.AddRange(t3.Result);
                db.E_TRANSACTION.AddRange(t4.Result);
                db.E_TRANSACTION.AddRange(t5.Result);
                db.E_TRANSACTION.AddRange(t6.Result);
                db.E_TRANSACTION.AddRange(t7.Result);
                db.E_TRANSACTION.AddRange(t8.Result);

            */


            Console.WriteLine("ETZ Channel Transaction Completed");

            logger.LogInfoMessage(nameof(EtranzactChannelTransaction) + " ETZ Channel  Transaction Completed ");

        }

        #region Spool Payment Transactions from TMC
        public IList<E_TRANSACTION> Channel1()
        {

            TmcDataContext db = new TmcDataContext();
            var query = from A in db.E_TMCREQUEST
                        join B in db.E_TMCHOST_RESP
                        on new { X = A.TRANS_DATA, Y = A.TRANS_SEQ } equals new { X = B.TRANS_DATA, Y = B.SWITCH_REF } into jointData
                        from joinRecord in jointData.DefaultIfEmpty()
                        where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && A.PRO_CODE.StartsWith("13") && Settings.successKeys.Contains(A.RESPONSE_CODE) && A.MTI == "0200 ")
                        select new EtransactionViewModel
                        {
                            TRANS_CODE = "P",
                            CARD_NUM = A.PAN,
                            TRANSID = A.STAN,
                            MERCHANT_CODE = A.ACCT_ID2,
                            TRANS_DESCR = A.CARD_ACC_NAME,
                            RESPONSE_CODE =A.RESPONSE_CODE,
                            TRANS_AMOUNT = A.AMOUNT,
                            TRANS_DATE= A.TRANS_DATE,
                            CHANNELID = A.TRANS_DATA.Substring(0,2),
                            TRANS_TYPE= "1",
                            EXTERNAL_TRANSID = A.TRANS_DATA,
                            UNIQUE_TRANSID=A.TRANS_DATA,
                            FEE= A.FEE,
                            REVERSAL_KEY = A.TRANS_KEY,
                            TRANS_NO = A.TRANS_SEQ,
                            TERMINAL_ID=A.TERMINAL_ID,
                            CARD_SCHEME=A.CARD_SCHEME,
                            REFERENCE=joinRecord.REFERENCE,
                            RESP_RESPONSE_CODE = joinRecord.RESPONSE_CODE

                        };
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            var tmcreq = query.ToList().Take(Settings.number_of_record_perround);

            stopwatch.Stop();
            Console.WriteLine("Inspectingtime for channel 1 completed in " + stopwatch.Elapsed);
            
            var e_Transaction = Mapper.Map<IList<E_TRANSACTION>>(tmcreq);

            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + e_Transaction.Count());
            logger.LogInfoMessage(nameof(EtranzactChannelTransaction) + "Instantiating CardloadProducer  Threads ");
            Console.WriteLine("Channel1 Completed");
            logger.LogInfoMessage(nameof(EtranzactChannelTransaction) + " Channel1 Completed ");

            return e_Transaction;
        }
        public IList<E_TRANSACTION> Channel2()
        {

            TmcDataContext db = new TmcDataContext();
            var query = from A in db.E_TMCREQUEST
                        join B in db.E_TMCHOST_RESP
                        on new { X = A.TRANS_DATA, Y = A.TRANS_SEQ } equals new { X = B.TRANS_DATA, Y = B.SWITCH_REF } into jointData
                        from joinRecord in jointData.DefaultIfEmpty()
                        where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && A.PAN.Contains("SWI") && A.PRO_CODE.StartsWith("4") && Settings.successKeys.Contains(joinRecord.RESPONSE_CODE) && A.MTI == "0900")
                        select new EtransactionViewModel
                        {
                            TRANS_CODE = "P",
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
                            UNIQUE_TRANSID=A.TRANS_DATA,
                            FEE = A.FEE,
                            REVERSAL_KEY = A.TRANS_KEY,
                            TRANS_NO = A.TRANS_SEQ,
                            TERMINAL_ID = A.TERMINAL_ID,
                            CARD_SCHEME = A.CARD_SCHEME,
                            REFERENCE = joinRecord.REFERENCE,
                            RESP_RESPONSE_CODE = joinRecord.RESPONSE_CODE

                        };
            var tmcreq = query.ToList().Take(Settings.number_of_record_perround);

            var e_Transaction = Mapper.Map<IList<E_TRANSACTION>>(tmcreq);
            //EtzbkDataContext etzbkData = new EtzbkDataContext();
            //etzbkData.E_TRANSACTION.AddRange(e_Transaction);
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + tmcreq.Count());


            logger.LogInfoMessage(nameof(EtranzactChannelTransaction) + " Channel2 Completed ");
            return e_Transaction;
        }
        public IList<E_TRANSACTION> Channel3()
        {

            TmcDataContext db = new TmcDataContext();
            var query = from A in db.E_TMCREQUEST
                        join B in db.E_TMCHOST_RESP
                        on new { X = A.TRANS_DATA, Y = A.TRANS_SEQ } equals new { X = B.TRANS_DATA, Y = B.SWITCH_REF } into jointData
                        from joinRecord in jointData.DefaultIfEmpty()
                        where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && /* A.card_acc_id LIKE: Bank_Code AND */ A.PRO_CODE.StartsWith("4") && Settings.successKeys.Contains(joinRecord.RESPONSE_CODE) && A.MTI == "0900" && A.TRANS_DATA.EndsWith("XP"))
                        select new EtransactionViewModel
                        {
                            TRANS_CODE = "P",
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
                            UNIQUE_TRANSID=A.TRANS_DATA,
                            FEE = A.FEE,
                            REVERSAL_KEY = A.TRANS_KEY,
                            TRANS_NO = A.TRANS_SEQ,
                            TERMINAL_ID = A.TERMINAL_ID,
                            CARD_SCHEME = A.CARD_SCHEME,
                            REFERENCE = joinRecord.REFERENCE,
                            RESP_RESPONSE_CODE = joinRecord.RESPONSE_CODE

                        };
            var tmcreq = query.ToList().Take(Settings.number_of_record_perround);

            var e_Transaction = Mapper.Map<IList<E_TRANSACTION>>(tmcreq);
            //EtzbkDataContext etzbkData = new EtzbkDataContext();
            //etzbkData.E_TRANSACTION.AddRange(e_Transaction);
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + tmcreq.Count());



            logger.LogInfoMessage(nameof(EtranzactChannelTransaction) + " Channel3 Completed ");

            return e_Transaction;
        }
        public IList<E_TRANSACTION> Channel4()
        {

            TmcDataContext db = new TmcDataContext();
            
            var query = from A in db.E_TMCREQUEST
                        join B in db.E_TMCHOST_RESP
                        on new { X = A.TRANS_DATA, Y = A.TRANS_SEQ } equals new { X = B.TRANS_DATA, Y = B.SWITCH_REF } into jointData
                        from joinRecord in jointData.DefaultIfEmpty()
                        where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && /* A.card_acc_id LIKE: Bank_Code AND */ A.PRO_CODE.StartsWith("4") && Settings.successKeys.Contains(A.RESPONSE_CODE) && A.MTI == "0200")
                        select new EtransactionViewModel
                        {
                            TRANS_CODE = "P",
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
                            UNIQUE_TRANSID=A.TRANS_DATA,
                            FEE = A.FEE,
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
            Console.WriteLine("Inspectingtime completed in " + stopwatch.Elapsed);
            
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + tmcreq.Count());


            var e_Transaction = Mapper.Map<IList<E_TRANSACTION>>(tmcreq);
           // EtzbkDataContext etzbkData = new EtzbkDataContext();
            //etzbkData.E_TRANSACTION.AddRange(e_Transaction);
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + tmcreq.Count());


            logger.LogInfoMessage(nameof(EtranzactChannelTransaction) + " Channel4 Completed ");
            Console.WriteLine("Channel4 Completed");

            return e_Transaction;

        }
        public IList<E_TRANSACTION> Channel5()
        {

            TmcDataContext db = new TmcDataContext();
            var query = from A in db.E_TMCREQUEST
                        join B in db.E_TMCHOST_RESP
                        on new { X = A.TRANS_DATA, Y = A.TRANS_SEQ } equals new { X = B.TRANS_DATA, Y = B.SWITCH_REF } into jointData
                        from joinRecord in jointData.DefaultIfEmpty()
                        where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && /* A.card_acc_id LIKE: Bank_Code AND */ A.PRO_CODE.StartsWith("01") && Settings.successKeys.Contains(A.RESPONSE_CODE) && A.MTI == "0200")
                        select new EtransactionViewModel
                        {
                            TRANS_CODE = "P",
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
                            UNIQUE_TRANSID=A.TRANS_DATA,
                            FEE = A.FEE,
                            REVERSAL_KEY = A.TRANS_KEY,
                            TRANS_NO = A.TRANS_SEQ,
                            TERMINAL_ID = A.TERMINAL_ID,
                            CARD_SCHEME = A.CARD_SCHEME,
                            REFERENCE = joinRecord.REFERENCE,
                            RESP_RESPONSE_CODE = joinRecord.RESPONSE_CODE

                        };
            var tmcreq = query.ToList().Take(Settings.number_of_record_perround);

            var e_Transaction = Mapper.Map<IList<E_TRANSACTION>>(tmcreq);
            //EtzbkDataContext etzbkData = new EtzbkDataContext();
            //etzbkData.E_TRANSACTION.AddRange(e_Transaction);
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + tmcreq.Count());


            Console.WriteLine("Channel5 Completed");

            return e_Transaction;

        }
        public IList<E_TRANSACTION> Channel6()
        {

            TmcDataContext db = new TmcDataContext();
            var query = from A in db.E_TMCREQUEST
                        join B in db.E_TMCHOST_RESP
                        on new { X = A.TRANS_DATA, Y = A.TRANS_SEQ } equals new { X = B.TRANS_DATA, Y = B.SWITCH_REF } into jointData
                        from joinRecord in jointData.DefaultIfEmpty()
                        where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && /* A.card_acc_id LIKE: Bank_Code AND */ A.PRO_CODE.StartsWith("40") && Settings.successKeys.Contains(A.RESPONSE_CODE) && A.MTI == "0220" && A.CARD_ACC_ID.Trim().Length < 11)
                        select new EtransactionViewModel
                        {
                            TRANS_CODE = "P",
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
                            UNIQUE_TRANSID=A.TRANS_DATA,
                            FEE = A.FEE,
                            REVERSAL_KEY = A.TRANS_KEY,
                            TRANS_NO = A.TRANS_SEQ,
                            TERMINAL_ID = A.TERMINAL_ID,
                            CARD_SCHEME = A.CARD_SCHEME,
                            REFERENCE = joinRecord.REFERENCE,
                            RESP_RESPONSE_CODE = joinRecord.RESPONSE_CODE

                        };
            var tmcreq = query.ToList().Take(Settings.number_of_record_perround);


            var e_Transaction = Mapper.Map<IList<E_TRANSACTION>>(tmcreq);
            //EtzbkDataContext etzbkData = new EtzbkDataContext();
            //etzbkData.E_TRANSACTION.AddRange(e_Transaction);
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + tmcreq.Count());


            Console.WriteLine("Channel6 Completed");

            return e_Transaction;

        }
        public IList<E_TRANSACTION> Channel7()
        {

            TmcDataContext db = new TmcDataContext();
            var query = from A in db.E_TMCREQUEST
                        join B in db.E_TMCHOST_RESP
                        on new { X = A.TRANS_DATA, Y = A.TRANS_SEQ } equals new { X = B.TRANS_DATA, Y = B.SWITCH_REF } into jointData
                        from joinRecord in jointData.DefaultIfEmpty()
                        where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && /* A.card_acc_id LIKE: Bank_Code AND */ A.PRO_CODE.StartsWith("40") && Settings.successKeys.Contains(joinRecord.RESPONSE_CODE) && A.MTI == "0220" && A.CARD_ACC_ID.Trim().Length < 11)
                        select new EtransactionViewModel
                        {
                            TRANS_CODE = "P",
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
                            UNIQUE_TRANSID=A.TRANS_DATA,
                            FEE = A.FEE,
                            REVERSAL_KEY = A.TRANS_KEY,
                            TRANS_NO = A.TRANS_SEQ,
                            TERMINAL_ID = A.TERMINAL_ID,
                            CARD_SCHEME = A.CARD_SCHEME,
                            REFERENCE = joinRecord.REFERENCE,
                            RESP_RESPONSE_CODE = joinRecord.RESPONSE_CODE

                        };
            var tmcreq = query.ToList().Take(Settings.number_of_record_perround);


            var e_Transaction = Mapper.Map<IList<E_TRANSACTION>>(tmcreq);
            //EtzbkDataContext etzbkData = new EtzbkDataContext();
            //etzbkData.E_TRANSACTION.AddRange(e_Transaction);
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + tmcreq.Count());


            Console.WriteLine("Channel7 Completed");

            return e_Transaction;

        }
        public IList<E_TRANSACTION> Channel8()
        {

            TmcDataContext db = new TmcDataContext();
            var query = from A in db.E_TMCREQUEST
                        join B in db.E_TMCHOST_RESP
                        on new { X = A.TRANS_DATA, Y = A.TRANS_SEQ } equals new { X = B.TRANS_DATA, Y = B.SWITCH_REF } into jointData
                        from joinRecord in jointData.DefaultIfEmpty()
                        where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && (A.PAN.StartsWith("700001HOM") || A.PAN.StartsWith("700001INS")) && A.PRO_CODE.StartsWith("4") && Settings.successKeys.Contains(A.RESPONSE_CODE) && A.MTI == "0900")
                        select new EtransactionViewModel
                        {
                            TRANS_CODE = "P",
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
                            UNIQUE_TRANSID=A.TRANS_DATA,
                            FEE = A.FEE,
                            REVERSAL_KEY = A.TRANS_KEY,
                            TRANS_NO = A.TRANS_SEQ,
                            TERMINAL_ID = A.TERMINAL_ID,
                            CARD_SCHEME = A.CARD_SCHEME,
                            REFERENCE = joinRecord.REFERENCE,
                            RESP_RESPONSE_CODE = joinRecord.RESPONSE_CODE

                        };
            var tmcreq = query.ToList().Take(Settings.number_of_record_perround);

            var e_Transaction = Mapper.Map<IList<E_TRANSACTION>>(tmcreq);
            //EtzbkDataContext etzbkData = new EtzbkDataContext();
            //etzbkData.E_TRANSACTION.AddRange(e_Transaction);
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + tmcreq.Count());


            Console.WriteLine("Channel8 Completed");

            return e_Transaction;


        }
        #endregion
    }
}
