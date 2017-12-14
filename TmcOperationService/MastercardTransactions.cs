using DALContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TmcOperationService
{
    public class MastercardTransactions
    {


        public void Run()
        {
            var t1 = Task.Factory.StartNew(MastercardTrx1);
            var t2 = Task.Factory.StartNew(MastercardTrx2);
            var t3 = Task.Factory.StartNew(MastercardTrx3);

            List<Task> taskList = new List<Task> { t1, t2, t3 };
            Task.WaitAll(taskList.ToArray());

            Console.WriteLine("ETZ Master Card  Transaction Completed");
        }

        #region eTz Mastercard trans
        public void MastercardTrx1()
        {
            var src_node_value = new int[] { 210000059, 510000059 };

            TmcDataContext db = new TmcDataContext();
            var query = from A in db.E_TMCREQUEST
                        join B in db.E_TMCHOST_RESP
                        on new { X = A.TRANS_DATA, Y = A.TRANS_SEQ } equals new { X = B.TRANS_DATA, Y = B.SWITCH_REF } into jointData
                        from joinRecord in jointData.DefaultIfEmpty()
                        join C in db.E_TMCNODE
                        on A.TARGET_NODE equals C.INCON_ID into jointData2
                        from joinRecord2 in jointData2.DefaultIfEmpty()
                        where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && src_node_value.Contains(A.SRC_NODE) && A.TARGET_NODE == 210000049 && Settings.successKeys.Contains(joinRecord.RESPONSE_CODE) && A.MTI == "0200" && A.AMOUNT > 0 && A.REVERSAL_KEY == "" && A.TRANS_DATA != "")
                        //from A in db.E_TMCREQUEST
                        //from B in db.E_TMCHOST_RESP
                        //    .Where(details => A.TRANS_DATA == details.TRANS_DATA & A.TRANS_SEQ == details.SWITCH_REF)
                        //    .DefaultIfEmpty()
                        //from C in db.E_TMCNODE
                        //.Where(detailed => A.TARGET_NODE == detailed.INCON_ID).DefaultIfEmpty()
                        //.Where(opt => A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && src_node_value.Contains(A.SRC_NODE) && A.TARGET_NODE == 210000049 && Settings.successKeys.Contains(B.RESPONSE_CODE) && A.MTI == "0200" && A.AMOUNT > 0 && A.REVERSAL_KEY == "" && A.TRANS_DATA != "")
                        select new
                        {
                            TRANS_CODE = "F",
                            CARD_NUM = joinRecord2.ISSUER_CODE + A.PAN,
                            TRANS_NO = joinRecord.STAN,
                            MERCHANT_CODE = A.CARD_ACC_ID,
                            TRANS_DESCR = A.CARD_ACC_NAME,
                            A.RESPONSE_CODE,
                            TRANS_AMOUNT = A.AMOUNT,
                            A.TRANS_DATE,
                            EXTERNAL_TRANSID = A.SWITCH_KEY,
                            A.TRANS_DATA,
                            joinRecord.FEE,
                            A.REVERSAL_KEY,
                            TERMINAL_ID = A.TERMINAL_ID,
                            CARD_SCHEME = A.PRO_CODE,
                            TRANSID = joinRecord.REFERENCE,
                            RESP_RESPONSE_CODE = joinRecord.RESPONSE_CODE,
                            joinRecord.SOURCE_ACCT
                        };


            var tmcreq = query.ToList().Take(Settings.number_of_record_perround);

            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + tmcreq.Count());
        }


        public void MastercardTrx2()
        {

            TmcDataContext db = new TmcDataContext();
            var query =
            //from A in db.E_TMCREQUEST
            //            from B in db.E_TMCHOST_RESP
            //                .Where(details => A.TRANS_DATA == details.TRANS_DATA & A.TRANS_SEQ == details.SWITCH_REF)
            //                .DefaultIfEmpty()
            //            from C in db.E_TMCNODE
            //            .Where(detailed => A.TARGET_NODE == detailed.INCON_ID).DefaultIfEmpty()
                        from A in db.E_TMCREQUEST
                        join B in db.E_TMCHOST_RESP
                        on new { X = A.TRANS_DATA, Y = A.TRANS_SEQ } equals new { X = B.TRANS_DATA, Y = B.SWITCH_REF } into jointData
                        from joinRecord in jointData.DefaultIfEmpty()
                        join C in db.E_TMCNODE
                        on A.TARGET_NODE equals C.INCON_ID into jointData2
                        from joinRecord2 in jointData2.DefaultIfEmpty()
                        where( A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && A.SRC_NODE == 210000059 && A.TARGET_NODE == 510000063 && Settings.successKeys.Contains(A.RESPONSE_CODE) && A.MTI == "0200" && A.AMOUNT > 0 && A.REVERSAL_KEY == "" && A.TRANS_DATA != "")
                        select new
                        {
                            TRANS_CODE = "G",
                            CARD_NUM = joinRecord2.ISSUER_CODE + A.ACCT_ID1.Substring(1, 3) + A.PAN.Substring(1, 6) + "XXX" + A.PAN.Substring(A.PAN.Length - 4, 4),
                            TRANS_NO = A.STAN,
                            MERCHANT_CODE = A.CARD_ACC_ID,
                            TRANS_DESCR = A.CARD_ACC_NAME,
                            A.RESPONSE_CODE,
                            TRANS_AMOUNT = A.AMOUNT,
                            A.TRANS_DATE,
                            EXTERNAL_TRANSID = A.SWITCH_KEY,
                            A.TRANS_DATA,
                            FEE = joinRecord2.INT_FEE,
                            A.REVERSAL_KEY,
                            TERMINAL_ID = A.TERMINAL_ID,
                            CARD_SCHEME = A.PRO_CODE,
                            TRANSID = A.TRANS_SEQ,
                            RESP_RESPONSE_CODE = A.RESPONSE_CODE,
                            SOURCE_ACCT = A.ACCT_ID1

                        };




            var tmcreq = query.ToList().Take(Settings.number_of_record_perround);

            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + tmcreq.Count());
        }

        public void MastercardTrx3()
        {

            TmcDataContext db = new TmcDataContext();
            var query =
                        //from A in db.E_TMCREQUEST
                        //from B in db.E_TMCHOST_RESP
                        //    .Where(details => A.TRANS_DATA == details.TRANS_DATA & A.TRANS_SEQ == details.SWITCH_REF)
                        //    .DefaultIfEmpty()
                        //from C in db.E_TMCNODE
                        //.Where(detailed => A.TARGET_NODE == detailed.INCON_ID).DefaultIfEmpty()
                        from A in db.E_TMCREQUEST
                        join B in db.E_TMCHOST_RESP
                        on new { X = A.TRANS_DATA, Y = A.TRANS_SEQ } equals new { X = B.TRANS_DATA, Y = B.SWITCH_REF } into jointData
                        from joinRecord in jointData.DefaultIfEmpty()
                        join C in db.E_TMCNODE
                        on A.TARGET_NODE equals C.INCON_ID into jointData2
                        from joinRecord2 in jointData2.DefaultIfEmpty()
                        where(A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && A.SRC_NODE == 510000064 && Settings.successKeys.Contains(A.RESPONSE_CODE) && A.MTI == "0200" && A.AMOUNT > 0 && A.REVERSAL_KEY == "" && A.TRANS_DATA != "")
                        select new
                        {
                            TRANS_CODE = "H",
                            CARD_NUM = joinRecord2.AQISSUER_CODE + "SMTATMW",
                            TRANS_NO = A.STAN,
                            MERCHANT_CODE = "700602SMTW",
                            TRANS_DESCR = A.CARD_ACC_NAME,
                            A.RESPONSE_CODE,
                            TRANS_AMOUNT = A.AMOUNT,
                            A.TRANS_DATE,
                            EXTERNAL_TRANSID = A.TRANS_KEY,
                            A.TRANS_DATA,
                            FEE = A.FEE,
                            A.REVERSAL_KEY,
                            TERMINAL_ID = A.TERMINAL_ID,
                            CARD_SCHEME = A.PRO_CODE,
                            TRANSID = A.TRANS_SEQ,
                            RESP_RESPONSE_CODE = A.RESPONSE_CODE,
                            SOURCE_ACCT = A.ACCT_ID1

                        };


            var tmcreq = query.ToList().Take(Settings.number_of_record_perround);

            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + tmcreq.Count());
        }


        #endregion

    }
}
