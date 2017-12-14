using DALContext;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TmcOperationService
{
    public class NonEtzCardTransaction
    {



        public void Run()
        {
            var t1 = Task.Factory.StartNew(NonEtzCard1);
            var t2 = Task.Factory.StartNew(NonEtzCard2); 

            List<Task> taskList = new List<Task> { t1, t2 };
            Task.WaitAll(taskList.ToArray());

            Console.WriteLine("ETZ Not Etz Transaction Completed");
        }



        #region  Non etz cards
        public void NonEtzCard1()
        {
            TmcDataContext db = new TmcDataContext();
            var query = from A in db.E_TMCREQUEST
                        join B in db.E_TMCNODE
                        on A.TRANS_SEQ equals B.INCON_NAME into jointData
                        from joinRecord in jointData.DefaultIfEmpty()
                        where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && (A.TERMINAL_ID.StartsWith("4505") || A.TERMINAL_ID.StartsWith("2030")) && A.RESPONSE_CODE == "00" && A.MTI == "0200" && A.PRO_CODE.StartsWith("00"))
                        select new
                        {
                            TRANS_CODE = "K",
                            CARD_NUM = A.PAN,
                            TRANSID = A.STAN,
                            //A.ACCT_ID2 MERCHANT_CODE,
                            MERCHANT_CODE = A.CARD_ACC_ID,
                            TRANS_DESCR = A.CARD_ACC_NAME,
                            A.RESPONSE_CODE,
                            TRANS_AMOUNT = A.AMOUNT,
                            A.TRANS_DATE,
                            EXTERNAL_TRANSID = A.SWITCH_KEY,
                            A.FEE,
                            REVERSAL_KEY = A.TRANS_KEY,
                            TRANS_NO = A.TRANS_SEQ,
                            A.TRANS_DATA,
                            A.TARGET,
                            CHANNELID = "03",
                            BANK_CODE = A.TARGET == "NIBBS_TMS" && A.PAN.Substring(1, 1) != "4" ? "032" : joinRecord.AQISSUER_CODE

                        };
            var tmcreq = query.ToList().Take(Settings.number_of_record_perround);

            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + tmcreq.Count());

        }

        public void NonEtzCard2()
        {

            TmcDataContext db = new TmcDataContext();
            var query = from A in db.E_TMCREQUEST
                        join B in db.E_TMCNODE
                        on A.TRANS_SEQ equals B.INCON_NAME into jointData
                        from joinRecord in jointData.DefaultIfEmpty()
                        where (A.TRANS_DATE > Settings.startdate && A.STATUS == "0" && (A.TERMINAL_ID.StartsWith("2") && !A.TERMINAL_ID.StartsWith("270") && !A.TERMINAL_ID.StartsWith("4505")) && A.RESPONSE_CODE == "00" && A.MTI == "0200" && A.PRO_CODE.StartsWith("00") && A.TARGET == "NIBBS_TMS")
                        select new
                        {
                            TRANS_CODE = "K",
                            CARD_NUM = A.PAN,
                            TRANSID = A.STAN,
                            MERCHANT_CODE = A.ACCT_ID2,
                            TRANS_DESCR = A.CARD_ACC_NAME,
                            A.RESPONSE_CODE,
                            TRANS_AMOUNT = A.AMOUNT,
                            A.TRANS_DATE,
                            EXTERNAL_TRANSID = A.SWITCH_KEY,
                            A.FEE,
                            REVERSAL_KEY = A.TRANS_KEY,
                            TRANS_NO = A.TRANS_SEQ,
                            A.TRANS_DATA,
                            A.TARGET,
                            CHANNELID = "03",
                            BANK_CODE = A.TARGET == "NIBBS_TMS" && A.PAN.Substring(1, 1) == "4" ? A.TERMINAL_ID.Substring(2, 3) : joinRecord.AQISSUER_CODE

                        };
            var tmcreq = query.ToList().Take(Settings.number_of_record_perround);

            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + tmcreq.Count());
        }

        #endregion

    }
}
