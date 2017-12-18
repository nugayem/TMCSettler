using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DALContext;
using DALContext.Model;

namespace TmcOperationService
{

    public class splitValueModule
    {


    }
    public class FeeProcessing
    {

        public static decimal CalculateFeeBeneficiary(decimal ratio, decimal TrnxFee)
        {
            decimal fee = 0;

            fee = (ratio / 100) * TrnxFee;

            return fee;


        }
        public static List<E_FEE_DETAIL_BK> ProcessCardloadSplit(E_TRANSACTION e_transaction, List<CommissionMapViewModel> splitFormular)
        {
            List<E_FEE_DETAIL_BK> feeDetailList = new List<E_FEE_DETAIL_BK>();
            if (e_transaction == null)
            {
                throw new ArgumentNullException(nameof(e_transaction));
            }
            else
            {
                int i = 1;
                string bankCode = e_transaction.CARD_NUM.Substring(0, 3);

                var items = from card in splitFormular
                            where card.BANK_CODE == bankCode
                            orderby card.MAIN_FLAG
                            select card;
                //Cehck Split is not configured for the Card, Pick the Defaul split
                if (items.Count() == 0)
                    items = splitFormular.Where(def => def.BANK_CODE == "000").OrderBy(a => a.MAIN_FLAG);
                foreach (var item in items)
                {
                    decimal fee = 0;
                    if (item.RATIO == 0)
                        fee = e_transaction.TRANS_AMOUNT;
                    else
                        fee = CalculateFeeBeneficiary(item.RATIO, e_transaction.FEE);

                    //if (item.MAIN_FLAG == 0)
                    //    //
                    //else if (item.MAIN_FLAG == 1)
                    //    //
                    E_FEE_DETAIL_BK feeDetail = new E_FEE_DETAIL_BK()
                    {
                        CARD_NUM = item.COMM_SUSPENCE == null || item.COMM_SUSPENCE.Trim() == "" ? bankCode + TransactionAlias.GetSuspenseAlias(e_transaction.TRANS_CODE) : item.COMM_SUSPENCE,
                        CHANNELID = e_transaction.CHANNELID,
                        CLOSED = "1",
                        CURRENCY = e_transaction.CURRENCY,
                        EXTERNAL_TRANSID = e_transaction.UNIQUE_TRANSID,
                        UNIQUE_TRANSID = "C" + i.ToString() + e_transaction.UNIQUE_TRANSID,
                        FEE = 0,
                        FEE_BATCH = e_transaction.FEE_BATCH,
                        GFLAG = item.MAIN_FLAG.ToString(),
                        INTSTATUS = "2",
                        ISSUER_CODE = e_transaction.CARD_NUM.Substring(0, 3),
                        MERCHANT_CODE = item.SPLIT_CARD.Contains("%") ? e_transaction.CARD_NUM.Substring(0, 3) + TransactionAlias.GetChannelAlias(e_transaction.CHANNELID.ToString()) + item.SPLIT_CARD.Substring(item.SPLIT_CARD.IndexOf("%") + 1) : item.SPLIT_CARD,
                        PROCESS_STATUS = "1",
                        SUB_CODE = e_transaction.CARD_NUM.Substring(3, 3),
                        SERVICEID = e_transaction.CARD_NUM.Substring(0, 6),
                        SETTLE_BATCH = e_transaction.SETTLE_BATCH,
                        TRANSID = e_transaction.TRANSID,
                        TRANS_AMOUNT = fee,
                        TRANS_CODE = "C",
                        TRANS_DATE = DateTime.Now,
                        TRANS_DESCR = "SETTLEMENT:;" + item.SPLIT_DESCR,
                        TRANS_NO = e_transaction.TRANS_NO,
                        TRANS_REF = e_transaction.MERCHANT_CODE,
                        TRANS_TYPE = "1"
                    };

                    Console.WriteLine("Split transaction Trans_Code=" + e_transaction.TRANS_CODE + "  Merchant_Code : " + feeDetail.MERCHANT_CODE + " Card_num: " + feeDetail.CARD_NUM + " Descr " + feeDetail.TRANS_DESCR + " isssuer" + feeDetail.ISSUER_CODE + " unique_trans: " + feeDetail.UNIQUE_TRANSID + " Value: " + feeDetail.TRANS_AMOUNT);
                    i++;
                    feeDetailList.Add(feeDetail);
                }

                return feeDetailList;

            }
        }
        public static List<E_FEE_DETAIL_BK> ProcessAbsolutePaymentSplit(E_TRANSACTION e_transaction, List<CommissionMapViewModel> splitFormular)
        {

            List<E_FEE_DETAIL_BK> feeDetailList = new List<E_FEE_DETAIL_BK>();
            if (e_transaction == null)
            {
                throw new ArgumentNullException(nameof(e_transaction));
                //return feeDetailList;
            }
            else
            {
                int i = 1;
                string bankCode = e_transaction.CARD_NUM.Substring(0, 3);
                decimal trans_amount = e_transaction.TRANS_AMOUNT;

                foreach (var item in splitFormular.OrderBy(a => a.MAIN_FLAG))
                {
                    decimal fee = 0;

                    if (item.RATIO == 0)
                        fee = e_transaction.TRANS_AMOUNT;
                    else
                        fee = CalculateFeeBeneficiary(item.RATIO, e_transaction.FEE);

                    E_FEE_DETAIL_BK feeDetail = new E_FEE_DETAIL_BK()
                    {
                        CARD_NUM = item.COMM_SUSPENCE,
                        CHANNELID = e_transaction.CHANNELID,
                        CLOSED = "1",
                        CURRENCY = e_transaction.CURRENCY,
                        EXTERNAL_TRANSID = e_transaction.UNIQUE_TRANSID,
                        UNIQUE_TRANSID = "C" + i.ToString() + e_transaction.UNIQUE_TRANSID,
                        FEE = 0,
                        FEE_BATCH = e_transaction.FEE_BATCH,
                        GFLAG = item.MAIN_FLAG.ToString(),
                        INTSTATUS = "2",
                        ISSUER_CODE = e_transaction.CARD_NUM.Substring(0, 3),
                        MERCHANT_CODE = item.SPLIT_CARD.Contains("%") ? e_transaction.CARD_NUM.Substring(0, 3) + TransactionAlias.GetChannelAlias(e_transaction.CHANNELID.ToString()) + item.SPLIT_CARD.Substring(item.SPLIT_CARD.IndexOf("%") + 1) : item.SPLIT_CARD,
                        PROCESS_STATUS = "1",
                        SUB_CODE = e_transaction.CARD_NUM.Substring(3, 3),
                        SERVICEID = e_transaction.CARD_NUM.Substring(0, 6),
                        SETTLE_BATCH = e_transaction.SETTLE_BATCH,
                        TRANSID = e_transaction.TRANSID,
                        TRANS_AMOUNT = fee,
                        TRANS_CODE = "C",
                        TRANS_DATE = DateTime.Now,
                        TRANS_DESCR = "SETTLEMENT:;" + item.SPLIT_DESCR,
                        TRANS_NO = e_transaction.TRANS_NO,
                        TRANS_REF = e_transaction.MERCHANT_CODE,
                        TRANS_TYPE = "1"
                    };

                    Console.WriteLine("Split transaction Trans_Code=" + e_transaction.TRANS_CODE + "  Merchant_Code : " + feeDetail.MERCHANT_CODE + " Card_num: " + feeDetail.CARD_NUM + " Descr " + feeDetail.TRANS_DESCR + " isssuer" + feeDetail.ISSUER_CODE + " unique_trans: " + feeDetail.UNIQUE_TRANSID + " Value: " + feeDetail.TRANS_AMOUNT);
                    i++;

                    feeDetailList.Add(feeDetail);
                }


                return feeDetailList;

            }
        }
        public static List<E_FEE_DETAIL_BK> ProcessRatioPaymentSplit(E_TRANSACTION e_transaction, List<CommissionMapViewModel> splitFormular)
        {

            List<E_FEE_DETAIL_BK> feeDetailList = new List<E_FEE_DETAIL_BK>();
            if (e_transaction == null)
            {
                throw new ArgumentNullException(nameof(e_transaction));
                //return feeDetailList;
            }
            else
            {
                int i = 1;
                //string bankCode = e_transaction.CARD_NUM.Substring(0, 3);
                decimal trans_amount = e_transaction.TRANS_AMOUNT;

                foreach (var item in splitFormular.OrderBy(a => a.MAIN_FLAG))
                {
                    decimal fee = 0;


                    fee = CalculateFeeBeneficiary(item.RATIO, e_transaction.FEE);


                    trans_amount = trans_amount - fee;
                    if (fee == 0)
                        fee = trans_amount;

                    E_FEE_DETAIL_BK feeDetail = new E_FEE_DETAIL_BK()
                    {
                        CARD_NUM = item.COMM_SUSPENCE,
                        CHANNELID = e_transaction.CHANNELID,
                        CLOSED = "1",
                        CURRENCY = e_transaction.CURRENCY,
                        EXTERNAL_TRANSID = e_transaction.UNIQUE_TRANSID,
                        UNIQUE_TRANSID = "C" + i.ToString() + e_transaction.UNIQUE_TRANSID,
                        FEE = 0,
                        FEE_BATCH = e_transaction.FEE_BATCH,
                        GFLAG = item.MAIN_FLAG.ToString(),
                        INTSTATUS = "2",
                        ISSUER_CODE = e_transaction.CARD_NUM.Substring(0, 3),
                        MERCHANT_CODE = item.SPLIT_CARD.Contains("%") ? e_transaction.CARD_NUM.Substring(0, 3) + TransactionAlias.GetChannelAlias(e_transaction.CHANNELID.ToString()) + item.SPLIT_CARD.Substring(item.SPLIT_CARD.IndexOf("%") + 1) : item.SPLIT_CARD,
                        PROCESS_STATUS = "1",
                        SUB_CODE = e_transaction.CARD_NUM.Substring(3, 3),
                        SERVICEID = e_transaction.CARD_NUM.Substring(0, 6),
                        SETTLE_BATCH = e_transaction.SETTLE_BATCH,
                        TRANSID = e_transaction.TRANSID,
                        TRANS_AMOUNT = fee,
                        TRANS_CODE = "C",
                        TRANS_DATE = DateTime.Now,
                        TRANS_DESCR = "SETTLEMENT:;" + item.SPLIT_DESCR,
                        TRANS_NO = e_transaction.TRANS_NO,
                        TRANS_REF = e_transaction.MERCHANT_CODE,
                        TRANS_TYPE = "1"
                    };

                    Console.WriteLine("Split transaction Trans_Code=" + e_transaction.TRANS_CODE + "  Merchant_Code : " + feeDetail.MERCHANT_CODE + " Card_num: " + feeDetail.CARD_NUM + " Descr " + feeDetail.TRANS_DESCR + " isssuer" + feeDetail.ISSUER_CODE + " unique_trans: " + feeDetail.UNIQUE_TRANSID + " Value: " + feeDetail.TRANS_AMOUNT);
                    i++;

                    feeDetailList.Add(feeDetail);
                }


                return feeDetailList;

            }
        }
        public void splittrx(E_TRANSACTION e_transaction, List<E_CARDLOAD_COMMISSION_SPLIT> splitFormular)
        {
            if (e_transaction == null)
            {
                throw new ArgumentNullException(nameof(e_transaction));
            }
            else
            {
                //string feeCardNum;
                //string feeMerchantCode;
                //decimal feeTransAmount;


                foreach (var cardLoadSplit in splitFormular)
                {
                    if (cardLoadSplit.SPLIT_CARD == "0")
                    {

                    }
                }

            }
        }

    }


    public static class TransactionAlias
    {

        public static string GetChannelAlias(string channelCode)
        {
            string value = "";
            switch (channelCode)
            {
                case "01":
                    value = "WEB";
                    break;
                case "02":
                    value = "MOB";
                    break;
                case "03":
                    value = "POS";
                    break;
                case "04":
                    value = "ATM";
                    break;
                case "05":
                    value = "OUT";
                    break;
                case "06":
                    value = "RET";
                    break;
                case "07":
                    value = "RET";
                    break;
                case "08":
                    value = "CPY";
                    break;
                case "09":
                    value = "FND";
                    break;
                default:
                    value = "001";
                    break;

            }

            return value;
        }

        public static string GetSuspenseAlias(string transCode)
        {
            string value = "";
            switch (transCode)
            {
                case "T":
                    value = "000TRNF";
                    break;
                case "D":
                    value = "000LOAD";
                    break;
            }
            return value;
        }
    }
}
