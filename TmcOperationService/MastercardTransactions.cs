using AutoMapper;
using DALContext;
using DALContext.Model;
using LoggerHelper.Services;
using System;
using System.Collections.Generic;
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
            EtzbkDataContext db = new EtzbkDataContext();

            Task<IList<E_TRANSACTION>> t1 = Task.Factory.StartNew(MastercardTrx1);
            Task<IList<E_TRANSACTION>> t2 = Task.Factory.StartNew(MastercardTrx2);
            Task<IList<E_TRANSACTION>> t3 = Task.Factory.StartNew(MastercardTrx3);

            List<Task> taskList = new List<Task> { t1, t2, t3 };
            Task.WaitAll(taskList.ToArray());

            var allTmcData = DataManupulation.MergeEntityList(new List<List<E_TRANSACTION>>() { t1.Result.ToList(), t2.Result.ToList(), t3.Result.ToList() });
            /*

            List<E_TRANSACTION> li = t1.Result.ToList();
            List<E_TRANSACTION> l2 = t2.Result.ToList();
            List<E_TRANSACTION> l3 = t3.Result.ToList();

            List<List<E_TRANSACTION>> etList = new List<List<E_TRANSACTION>>();
            etList.Add(li);
            etList.Add(l2);
            etList.Add(l3);
            DataManupulation.MergeEntityList(etList);

            //Merge All Data Spooled
            var allTmcData = new List<E_TRANSACTION>(t1.Result.Count + t2.Result.Count + t3.Result.Count);

            allTmcData.AddRange(t1.Result);
            allTmcData.AddRange(t2.Result);
            allTmcData.AddRange(t3.Result);
            */

            //Check if Data already exist on eTransactions

            var uniqueIDs = allTmcData.Select(u => u.UNIQUE_TRANSID).Distinct().ToArray();
            var uniqueIDsOnDB = db.E_TRANSACTION.Where(u => uniqueIDs.Contains(u.UNIQUE_TRANSID)).Select(u => u.UNIQUE_TRANSID).ToArray();
            var etrxData = allTmcData.Where(u => !uniqueIDsOnDB.Contains(u.UNIQUE_TRANSID));

            db.E_TRANSACTION.AddRange(etrxData);
            db.SaveChanges();

        }

        #region eTz Mastercard trans
        public IList<E_TRANSACTION> MastercardTrx1()
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
                        select new EtransactionViewModel
                        {
                            TRANS_CODE = "F",
                            CARD_NUM = joinRecord2.ISSUER_CODE + A.PAN,
                            TRANSID = joinRecord.REFERENCE,
                            TRANS_NO = joinRecord.STAN,
                            MERCHANT_CODE = A.CARD_ACC_ID,
                            TRANS_DESCR = A.CARD_ACC_NAME,
                            RESPONSE_CODE=A.RESPONSE_CODE,
                            TRANS_AMOUNT = A.AMOUNT,
                            TRANS_DATE=A.TRANS_DATE,
                            CHANNELID = A.TRANS_DATA.Substring(0, 2),
                            TRANS_TYPE = "1",
                            EXTERNAL_TRANSID = A.SWITCH_KEY,
                            UNIQUE_TRANSID=A.TRANS_DATA,
                            FEE=A.FEE,
                            REVERSAL_KEY= A.REVERSAL_KEY,
                            TERMINAL_ID = A.TERMINAL_ID,
                            CARD_SCHEME = A.PRO_CODE,
                            RESP_RESPONSE_CODE = joinRecord.RESPONSE_CODE,
                            
                        };


            var tmcreq = query.ToList().Take(Settings.number_of_record_perround);

            var e_Transaction = Mapper.Map<IList<E_TRANSACTION>>(tmcreq);
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + e_Transaction.Count());

            logger.LogInfoMessage(nameof(EtranzactChannelTransaction) + " MastercardTrx1 Completed ");
            return e_Transaction;
        }


        public IList<E_TRANSACTION> MastercardTrx2()
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
                        select new EtransactionViewModel
                        {
                            TRANS_CODE = "G",
                            CARD_NUM = joinRecord2.ISSUER_CODE + A.ACCT_ID1.Substring(1, 3) + A.PAN.Substring(1, 6) + "XXX" + A.PAN.Substring(A.PAN.Length - 4, 4),
                            TRANSID = A.TRANS_SEQ,
                            TRANS_NO = A.STAN,
                            MERCHANT_CODE = A.CARD_ACC_ID,
                            TRANS_DESCR = A.CARD_ACC_NAME,
                            RESPONSE_CODE=A.RESPONSE_CODE,
                            TRANS_AMOUNT = A.AMOUNT,
                            TRANS_DATE=A.TRANS_DATE,
                            CHANNELID = A.TRANS_DATA.Substring(0, 2),
                            TRANS_TYPE = "1",
                            EXTERNAL_TRANSID = A.SWITCH_KEY,
                            UNIQUE_TRANSID = A.TRANS_DATA,
                            FEE = joinRecord2.INT_FEE,
                            REVERSAL_KEY=A.REVERSAL_KEY,
                            TERMINAL_ID = A.TERMINAL_ID,
                            CARD_SCHEME = A.PRO_CODE,
                            RESP_RESPONSE_CODE = A.RESPONSE_CODE

                


                        };


            var tmcreq = query.ToList().Take(Settings.number_of_record_perround);

            var e_Transaction = Mapper.Map<IList<E_TRANSACTION>>(tmcreq);
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + e_Transaction.Count());


            logger.LogInfoMessage(nameof(EtranzactChannelTransaction) + " MastercardTrx2 Completed ");
            return e_Transaction;
        }

        public IList<E_TRANSACTION> MastercardTrx3()
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
                            TERMINAL_ID = A.TERMINAL_ID,
                            CARD_SCHEME = A.PRO_CODE,
                            RESP_RESPONSE_CODE = A.RESPONSE_CODE,                         
                           

                        };


            var tmcreq = query.ToList().Take(Settings.number_of_record_perround);

            var e_Transaction = Mapper.Map<IList<E_TRANSACTION>>(tmcreq);
            //EtzbkDataContext etzbkData = new EtzbkDataContext();
            //etzbkData.E_TRANSACTION.AddRange(e_Transaction);
            Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + " " + e_Transaction.Count());


            logger.LogInfoMessage(nameof(EtranzactChannelTransaction) + " MastercardTrx3 Completed ");
            return e_Transaction;

        }


        #endregion

    }
}
