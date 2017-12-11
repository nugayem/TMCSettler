using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DALContext.Model;

namespace TmcSettler.ConsoleApp.Services.FeeService
{
    public  class ProcessAbsoluteFee 
    {
        public static List<E_FEE_DETAIL_BK> ProcessCardloadSplit(E_TRANSACTION e_transaction, List<E_COMMISSION_MAP> splitFormular)
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

                    fee = trans_amount - item.RATIO;                     

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
    }
}
