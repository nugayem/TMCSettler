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

namespace TmcSettler.ConsoleApp.Services
{
   public  class EtranzactChannelTransaction 
    {
      
        public   List<IQueryable> GetListOfEtzChannelQuery()
        {
            List<IQueryable> eTzQueryTrxList = new List<IQueryable>();

            TmcDataContext db = new TmcDataContext(); 
            var query1 = from A in db.E_TMCREQUEST
                        join B in db.E_TMCHOST_RESP
                        on new { X = A.TRANS_DATA, Y = A.TRANS_SEQ } equals new { X = B.TRANS_DATA, Y = B.SWITCH_REF } into jointData
                        from joinRecord in jointData.DefaultIfEmpty()
                        where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && A.PRO_CODE.StartsWith("13") && Settings.successKeys.Contains(A.RESPONSE_CODE) && A.MTI == "0200")
                        select new
                        {
                            TRANS_CODE = "P",
                            CARD_NUM = A.PAN,
                            TRANSID = A.STAN,
                            MERCHANT_CODE = A.ACCT_ID2,
                            TRANS_DESCR = A.CARD_ACC_NAME,
                            A.RESPONSE_CODE,
                            TRANS_AMOUNT = A.AMOUNT,
                            A.TRANS_DATE,
                            EXTERNAL_TRANSID = A.TRANS_DATA,
                            A.FEE,
                            REVERSAL_KEY = A.TRANS_KEY,
                            TRANS_NO = A.TRANS_SEQ,
                            A.TERMINAL_ID,
                            A.CARD_SCHEME,
                            joinRecord.REFERENCE,
                            RESP_RESPONSE_CODE = joinRecord.RESPONSE_CODE


                        };
            
            var query2 = from A in db.E_TMCREQUEST
                        join B in db.E_TMCHOST_RESP
                        on new { X = A.TRANS_DATA, Y = A.TRANS_SEQ } equals new { X = B.TRANS_DATA, Y = B.SWITCH_REF } into jointData
                        from joinRecord in jointData.DefaultIfEmpty()
                        where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && A.PAN.Contains("SWI") && A.PRO_CODE.StartsWith("4") && Settings.successKeys.Contains(joinRecord.RESPONSE_CODE) && A.MTI == "0900")
                        select new
                        {
                            TRANS_CODE = "N",
                            CARD_NUM = A.PAN,
                            TRANSID = A.STAN,
                            MERCHANT_CODE = A.ACCT_ID2,
                            TRANS_DESCR = A.CARD_ACC_NAME,
                            A.RESPONSE_CODE,
                            TRANS_AMOUNT = A.AMOUNT,
                            A.TRANS_DATE,
                            EXTERNAL_TRANSID = A.TRANS_DATA,
                            A.FEE,
                            REVERSAL_KEY = A.TRANS_KEY,
                            TRANS_NO = A.TRANS_SEQ,
                            A.TERMINAL_ID,
                            A.CARD_SCHEME,
                            joinRecord.REFERENCE,
                            RESP_RESPONSE_CODE = joinRecord.RESPONSE_CODE


                        };

            var query3 = from A in db.E_TMCREQUEST
                        join B in db.E_TMCHOST_RESP
                        on new { X = A.TRANS_DATA, Y = A.TRANS_SEQ } equals new { X = B.TRANS_DATA, Y = B.SWITCH_REF } into jointData
                        from joinRecord in jointData.DefaultIfEmpty()
                        where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && /* A.card_acc_id LIKE: Bank_Code AND */ A.PRO_CODE.StartsWith("4") && Settings.successKeys.Contains(joinRecord.RESPONSE_CODE) && A.MTI == "0900" && A.TRANS_DATA.EndsWith("XP"))
                        select new
                        {
                            TRANS_CODE = "N",
                            CARD_NUM = A.PAN,
                            TRANSID = A.STAN,
                            MERCHANT_CODE = A.ACCT_ID2,
                            TRANS_DESCR = A.CARD_ACC_NAME,
                            A.RESPONSE_CODE,
                            TRANS_AMOUNT = A.AMOUNT,
                            A.TRANS_DATE,
                            EXTERNAL_TRANSID = A.TRANS_DATA,
                            A.FEE,
                            REVERSAL_KEY = A.TRANS_KEY,
                            TRANS_NO = A.TRANS_SEQ,
                            A.TERMINAL_ID,
                            A.CARD_SCHEME,
                            joinRecord.REFERENCE,
                            RESP_RESPONSE_CODE = joinRecord.RESPONSE_CODE
                        };

            var query4 = from A in db.E_TMCREQUEST
                        join B in db.E_TMCHOST_RESP
                        on new { X = A.TRANS_DATA, Y = A.TRANS_SEQ } equals new { X = B.TRANS_DATA, Y = B.SWITCH_REF } into jointData
                        from joinRecord in jointData.DefaultIfEmpty()
                        where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && /* A.card_acc_id LIKE: Bank_Code AND */ A.PRO_CODE.StartsWith("4") && Settings.successKeys.Contains(A.RESPONSE_CODE) && A.MTI == "0200")
                        select new
                        {
                            TRANS_CODE = "T",
                            CARD_NUM = A.PAN,
                            TRANSID = A.STAN,
                            MERCHANT_CODE = A.ACCT_ID2,
                            TRANS_DESCR = A.CARD_ACC_NAME,
                            A.RESPONSE_CODE,
                            TRANS_AMOUNT = A.AMOUNT,
                            A.TRANS_DATE,
                            EXTERNAL_TRANSID = A.TRANS_DATA,
                            A.FEE,
                            REVERSAL_KEY = A.TRANS_KEY,
                            TRANS_NO = A.TRANS_SEQ,
                            A.TERMINAL_ID,
                            A.CARD_SCHEME,
                            joinRecord.REFERENCE,
                            RESP_RESPONSE_CODE = joinRecord.RESPONSE_CODE
                        };

            var query5 = from A in db.E_TMCREQUEST
                         join B in db.E_TMCHOST_RESP
                         on new { X = A.TRANS_DATA, Y = A.TRANS_SEQ } equals new { X = B.TRANS_DATA, Y = B.SWITCH_REF } into jointData
                         from joinRecord in jointData.DefaultIfEmpty()
                         where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && /* A.card_acc_id LIKE: Bank_Code AND */ A.PRO_CODE.StartsWith("01") && Settings.successKeys.Contains(A.RESPONSE_CODE) && A.MTI == "0200")
                         select new
                         {
                             TRANS_CODE = "W",
                             CARD_NUM = A.PAN,
                             TRANSID = A.STAN,
                             MERCHANT_CODE = A.ACCT_ID2,
                             TRANS_DESCR = A.CARD_ACC_NAME,
                             A.RESPONSE_CODE,
                             TRANS_AMOUNT = A.AMOUNT,
                             A.TRANS_DATE,
                             EXTERNAL_TRANSID = A.TRANS_DATA,
                             A.FEE,
                             REVERSAL_KEY = A.TRANS_KEY,
                             TRANS_NO = A.TRANS_SEQ,
                             A.TERMINAL_ID,
                             A.CARD_SCHEME,
                             joinRecord.REFERENCE,
                             RESP_RESPONSE_CODE = ""
                         };


            var query6 = from A in db.E_TMCREQUEST
                        join B in db.E_TMCHOST_RESP
                        on new { X = A.TRANS_DATA, Y = A.TRANS_SEQ } equals new { X = B.TRANS_DATA, Y = B.SWITCH_REF } into jointData
                        from joinRecord in jointData.DefaultIfEmpty()
                        where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && /* A.card_acc_id LIKE: Bank_Code AND */ A.PRO_CODE.StartsWith("40") && Settings.successKeys.Contains(A.RESPONSE_CODE) && A.MTI == "0220" && A.CARD_ACC_ID.Trim().Length < 11)
                        select new
                        {
                            TRANS_CODE = "M",
                            CARD_NUM = A.PAN,
                            TRANSID = A.STAN,
                            MERCHANT_CODE = A.ACCT_ID2,
                            TRANS_DESCR = A.CARD_ACC_NAME,
                            A.RESPONSE_CODE,
                            TRANS_AMOUNT = A.AMOUNT,
                            A.TRANS_DATE,
                            EXTERNAL_TRANSID = A.TRANS_DATA,
                            A.FEE,
                            REVERSAL_KEY = A.TRANS_KEY,
                            TRANS_NO = A.TRANS_SEQ,
                            A.TERMINAL_ID,
                            A.CARD_SCHEME,
                            joinRecord.REFERENCE,
                            RESP_RESPONSE_CODE = joinRecord.RESPONSE_CODE
                        };



            var query7 = from A in db.E_TMCREQUEST
                        join B in db.E_TMCHOST_RESP
                        on new { X = A.TRANS_DATA, Y = A.TRANS_SEQ } equals new { X = B.TRANS_DATA, Y = B.SWITCH_REF } into jointData
                        from joinRecord in jointData.DefaultIfEmpty()
                        where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && /* A.card_acc_id LIKE: Bank_Code AND */ A.PRO_CODE.StartsWith("40") && Settings.successKeys.Contains(joinRecord.RESPONSE_CODE) && A.MTI == "0220" && A.CARD_ACC_ID.Trim().Length < 11)
                        select new
                        {
                            TRANS_CODE = "D",
                            CARD_NUM = A.PAN,
                            TRANSID = A.STAN,
                            MERCHANT_CODE = A.ACCT_ID2,
                            TRANS_DESCR = A.CARD_ACC_NAME,
                            A.RESPONSE_CODE,
                            TRANS_AMOUNT = A.AMOUNT,
                            A.TRANS_DATE,
                            EXTERNAL_TRANSID = A.TRANS_DATA,
                            A.FEE,
                            REVERSAL_KEY = A.TRANS_KEY,
                            TRANS_NO = A.TRANS_SEQ,
                            A.TERMINAL_ID,
                            A.CARD_SCHEME,
                            joinRecord.REFERENCE,
                            RESP_RESPONSE_CODE = joinRecord.RESPONSE_CODE
                        };


            var query8 = from A in db.E_TMCREQUEST
                        join B in db.E_TMCHOST_RESP
                        on new { X = A.TRANS_DATA, Y = A.TRANS_SEQ } equals new { X = B.TRANS_DATA, Y = B.SWITCH_REF } into jointData
                        from joinRecord in jointData.DefaultIfEmpty()
                        where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && (A.PAN.StartsWith("700001HOM") || A.PAN.StartsWith("700001INS")) && A.PRO_CODE.StartsWith("4") && Settings.successKeys.Contains(A.RESPONSE_CODE) && A.MTI == "0900")
                        select new
                        {
                            TRANS_CODE = "H",
                            CARD_NUM = A.PAN,
                            TRANSID = A.STAN,
                            MERCHANT_CODE = A.ACCT_ID2,
                            TRANS_DESCR = A.CARD_ACC_NAME,
                            A.RESPONSE_CODE,
                            TRANS_AMOUNT = A.AMOUNT,
                            A.TRANS_DATE,
                            EXTERNAL_TRANSID = A.TRANS_DATA,
                            A.FEE,
                            REVERSAL_KEY = A.TRANS_KEY,
                            TRANS_NO = A.TRANS_SEQ,
                            A.TERMINAL_ID,
                            A.CARD_SCHEME,
                            joinRecord.REFERENCE,
                            RESP_RESPONSE_CODE = joinRecord.RESPONSE_CODE
                        };


            eTzQueryTrxList.Add(query1);
            eTzQueryTrxList.Add(query2);
            eTzQueryTrxList.Add(query3);
            eTzQueryTrxList.Add(query4);
            eTzQueryTrxList.Add(query5);
            eTzQueryTrxList.Add(query6);
            eTzQueryTrxList.Add(query7);
            eTzQueryTrxList.Add(query8);

            return eTzQueryTrxList;

        }


        public void Run()
        {
            EtzbkDataContext  db = new EtzbkDataContext();

            Task <IList<E_TRANSACTION>> t1 = Task.Factory.StartNew(Channel1);
            Task<IList<E_TRANSACTION>> t2 = Task.Factory.StartNew(Channel2);
            Task<IList<E_TRANSACTION>> t3 = Task.Factory.StartNew(Channel3);
            Task<IList<E_TRANSACTION>> t4 = Task.Factory.StartNew(Channel4);
            Task<IList<E_TRANSACTION>> t5 = Task.Factory.StartNew(Channel5);
            Task<IList<E_TRANSACTION>> t6 = Task.Factory.StartNew(Channel6);
            Task<IList<E_TRANSACTION>> t7 = Task.Factory.StartNew(Channel7);
            Task<IList<E_TRANSACTION>> t8 = Task.Factory.StartNew(Channel8);

            List<Task> taskList = new List<Task> { t1, t2, t3, t4, t5, t6, t7, t8 };
            Task.WaitAll(taskList.ToArray());

            db.E_TRANSACTION.AddRange(t1.Result);
            db.E_TRANSACTION.AddRange(t2.Result);
            db.E_TRANSACTION.AddRange(t3.Result);
            db.E_TRANSACTION.AddRange(t4.Result);
            db.E_TRANSACTION.AddRange(t5.Result);
            db.E_TRANSACTION.AddRange(t6.Result);
            db.E_TRANSACTION.AddRange(t7.Result);
            db.E_TRANSACTION.AddRange(t8.Result);


            db.SaveChanges();

            Console.WriteLine("ETZ Channel  Transaction Completed");
        }      

        #region Spool Payment Transactions from TMC
        public IList<E_TRANSACTION> Channel1()
        {

            TmcDataContext db = new TmcDataContext();
            var query = from A in db.E_TMCREQUEST
                        join B in db.E_TMCHOST_RESP
                        on new { X = A.TRANS_DATA, Y = A.TRANS_SEQ } equals new { X = B.TRANS_DATA, Y = B.SWITCH_REF } into jointData
                        from joinRecord in jointData.DefaultIfEmpty()
                        where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && A.PRO_CODE.StartsWith("13") && Settings.successKeys.Contains(A.RESPONSE_CODE) && A.MTI == "0200")
                        select new EtzCardTranx
                        {
                            TRANS_CODE = "P",
                            CARD_NUM = A.PAN,
                            TRANSID = A.STAN,
                            MERCHANT_CODE = A.ACCT_ID2,
                            TRANS_DESCR = A.CARD_ACC_NAME,
                            RESPONSE_CODE =A.RESPONSE_CODE,
                            TRANS_AMOUNT = A.AMOUNT,
                             TRANS_DATE= A.TRANS_DATE,
                            EXTERNAL_TRANSID = A.TRANS_DATA,
                            FEE= A.FEE,
                            REVERSAL_KEY = A.TRANS_KEY,
                            TRANS_NO = A.TRANS_SEQ,
                            TERMINAL_ID=A.TERMINAL_ID,
                            CARD_SCHEME=A.CARD_SCHEME,
                            REFERENCE=joinRecord.REFERENCE,
                            RESP_RESPONSE_CODE = joinRecord.RESPONSE_CODE

                        };
            var tmcreq = query.ToList().Take(Settings.number_of_record_perround);

            var e_Transaction = Mapper.Map<IList<E_TRANSACTION>>(tmcreq);
            EtzbkDataContext etzbkData = new EtzbkDataContext();
            etzbkData.E_TRANSACTION.AddRange(e_Transaction);
                    Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + tmcreq.Count());
 

            Console.WriteLine("Channel1 Completed");

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
                        select new EtzCardTranx
                        {
                            TRANS_CODE = "P",
                            CARD_NUM = A.PAN,
                            TRANSID = A.STAN,
                            MERCHANT_CODE = A.ACCT_ID2,
                            TRANS_DESCR = A.CARD_ACC_NAME,
                            RESPONSE_CODE = A.RESPONSE_CODE,
                            TRANS_AMOUNT = A.AMOUNT,
                            TRANS_DATE = A.TRANS_DATE,
                            EXTERNAL_TRANSID = A.TRANS_DATA,
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
            EtzbkDataContext etzbkData = new EtzbkDataContext();
            etzbkData.E_TRANSACTION.AddRange(e_Transaction);
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + tmcreq.Count());


            Console.WriteLine("Channel2 Completed");

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
                        select new EtzCardTranx
                        {
                            TRANS_CODE = "P",
                            CARD_NUM = A.PAN,
                            TRANSID = A.STAN,
                            MERCHANT_CODE = A.ACCT_ID2,
                            TRANS_DESCR = A.CARD_ACC_NAME,
                            RESPONSE_CODE = A.RESPONSE_CODE,
                            TRANS_AMOUNT = A.AMOUNT,
                            TRANS_DATE = A.TRANS_DATE,
                            EXTERNAL_TRANSID = A.TRANS_DATA,
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
            EtzbkDataContext etzbkData = new EtzbkDataContext();
            etzbkData.E_TRANSACTION.AddRange(e_Transaction);
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + tmcreq.Count());


            Console.WriteLine("Channel3 Completed");

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
                        select new EtzCardTranx
                        {
                            TRANS_CODE = "P",
                            CARD_NUM = A.PAN,
                            TRANSID = A.STAN,
                            MERCHANT_CODE = A.ACCT_ID2,
                            TRANS_DESCR = A.CARD_ACC_NAME,
                            RESPONSE_CODE = A.RESPONSE_CODE,
                            TRANS_AMOUNT = A.AMOUNT,
                            TRANS_DATE = A.TRANS_DATE,
                            EXTERNAL_TRANSID = A.TRANS_DATA,
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
            EtzbkDataContext etzbkData = new EtzbkDataContext();
            etzbkData.E_TRANSACTION.AddRange(e_Transaction);
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + tmcreq.Count());


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
                        select new EtzCardTranx
                        {
                            TRANS_CODE = "P",
                            CARD_NUM = A.PAN,
                            TRANSID = A.STAN,
                            MERCHANT_CODE = A.ACCT_ID2,
                            TRANS_DESCR = A.CARD_ACC_NAME,
                            RESPONSE_CODE = A.RESPONSE_CODE,
                            TRANS_AMOUNT = A.AMOUNT,
                            TRANS_DATE = A.TRANS_DATE,
                            EXTERNAL_TRANSID = A.TRANS_DATA,
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
            EtzbkDataContext etzbkData = new EtzbkDataContext();
            etzbkData.E_TRANSACTION.AddRange(e_Transaction);
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
                        select new EtzCardTranx
                        {
                            TRANS_CODE = "P",
                            CARD_NUM = A.PAN,
                            TRANSID = A.STAN,
                            MERCHANT_CODE = A.ACCT_ID2,
                            TRANS_DESCR = A.CARD_ACC_NAME,
                            RESPONSE_CODE = A.RESPONSE_CODE,
                            TRANS_AMOUNT = A.AMOUNT,
                            TRANS_DATE = A.TRANS_DATE,
                            EXTERNAL_TRANSID = A.TRANS_DATA,
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
            EtzbkDataContext etzbkData = new EtzbkDataContext();
            etzbkData.E_TRANSACTION.AddRange(e_Transaction);
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
                        select new EtzCardTranx
                        {
                            TRANS_CODE = "P",
                            CARD_NUM = A.PAN,
                            TRANSID = A.STAN,
                            MERCHANT_CODE = A.ACCT_ID2,
                            TRANS_DESCR = A.CARD_ACC_NAME,
                            RESPONSE_CODE = A.RESPONSE_CODE,
                            TRANS_AMOUNT = A.AMOUNT,
                            TRANS_DATE = A.TRANS_DATE,
                            EXTERNAL_TRANSID = A.TRANS_DATA,
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
            EtzbkDataContext etzbkData = new EtzbkDataContext();
            etzbkData.E_TRANSACTION.AddRange(e_Transaction);
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
                        select new EtzCardTranx
                        {
                            TRANS_CODE = "P",
                            CARD_NUM = A.PAN,
                            TRANSID = A.STAN,
                            MERCHANT_CODE = A.ACCT_ID2,
                            TRANS_DESCR = A.CARD_ACC_NAME,
                            RESPONSE_CODE = A.RESPONSE_CODE,
                            TRANS_AMOUNT = A.AMOUNT,
                            TRANS_DATE = A.TRANS_DATE,
                            EXTERNAL_TRANSID = A.TRANS_DATA,
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
            EtzbkDataContext etzbkData = new EtzbkDataContext();
            etzbkData.E_TRANSACTION.AddRange(e_Transaction);
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + tmcreq.Count());


            Console.WriteLine("Channel8 Completed");

            return e_Transaction;


        }
        #endregion
    }
}
