﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DALContext;
using DALContext.Model;

namespace TmcSettler.ConsoleApp.Services
{

    public class splitValueModule
    {


    }
    public  class FeeProcessing
    {

        public static decimal CalculateFeeBeneficiary( decimal ratio , decimal TrnxFee)
        {
            decimal fee = 0;

            fee = (ratio / 100) * TrnxFee;
            
            return fee;


        }
        public static List<E_FEE_DETAIL_BK> ProcessCardloadSplit(E_TRANSACTION e_transaction, List<E_CARDLOAD_COMMISSION_SPLIT> splitFormular)
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
                            select card;
                //Cehck Split is not configured for the Card, Pick the Defaul split
                if (items.Count() == 0)
                    items = splitFormular.Where(def => def.BANK_CODE == "000");
                foreach (var item in items)
                {
                    decimal fee = CalculateFeeBeneficiary(item.RATIO, e_transaction.FEE);
                  
                    E_FEE_DETAIL_BK feeDetail = new E_FEE_DETAIL_BK()
                    {
                        CARD_NUM = item.COMM_SUSPENCE == null || item.COMM_SUSPENCE.Trim() == "" ? bankCode + ChannelAlias.GetChannelAlias(e_transaction.CHANNELID.ToString()) + "9999" : item.COMM_SUSPENCE,
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
                        MERCHANT_CODE = item.SPLIT_CARD.Contains("%") ? e_transaction.CARD_NUM.Substring(0,3) + ChannelAlias.GetChannelAlias(e_transaction.CHANNELID.ToString())+ item.SPLIT_CARD.Substring(item.SPLIT_CARD.IndexOf("%") + 1): item.SPLIT_CARD,
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
                    Console.WriteLine("Split transaction Merchant_Code : " + feeDetail.MERCHANT_CODE + " Card_num: "+ feeDetail.CARD_NUM+" Descr " + feeDetail.TRANS_DESCR+ " isssuer" + feeDetail.ISSUER_CODE + " unique_trans: " + feeDetail.UNIQUE_TRANSID + " Value: "+ feeDetail.TRANS_AMOUNT);
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
                string feeCardNum;
                string feeMerchantCode;
                decimal feeTransAmount;


                foreach (var cardLoadSplit in splitFormular)
                {
                    if (cardLoadSplit.SPLIT_CARD == "0")
                    {

                    }
                }

            }
        }

        //public double getCalcFee(double amount)
        //{
        //    double fee = 0.0;       
        //    double ld_from = 0.0;
        //    double ld_to = 0.0;
        //    double ld_value = 0.0;
        //    int retCode = -1;

        //    try
        //    {
        //        con = SimplePool.ds.getConnection();
        //        stmt = con.createStatement();
        //        ls_sql = "SELECT * FROM E_CATSCALE WHERE CAT_ID='" + getFeeCategory() + "'";
        //        //System.out.println("Scale:"+ls_sql);
        //        stmt = con.createStatement();
        //        rs = stmt.executeQuery(ls_sql);
        //        while (rs.next())
        //        {
        //            ld_from = rs.getDouble("SCALE_FROM");
        //            ld_to = rs.getDouble("SCALE_TO");
        //            ld_value = rs.getDouble("SCALE_VALUE");
        //            ls_type = rs.getString("SCALE_TYPE");

        //            if (amount >= ld_from && amount <= ld_to)
        //            {
        //                fee = (ls_type.equals("1") ? ld_value / 100 * amount : ld_value); //0 - value, 1- percentage
        //                retCode = 1; //0 - value, 1- percentage
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        retCode = -1;
        //    }
        //    finally
        //    {
        //        try
        //        {
        //            if (rs != null) rs.close();
        //            if (stmt != null) stmt.close();
        //            if (con != null) con.close();
        //        }
        //        catch (Exception ee)
        //        {
        //            ;
        //        }
        //    }

        //    return fee;
        //}

        //public void CalCulateCardLoadFee(double amount)
        //{

        //    double fee = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);

        //    foreach( item in )


        //}
    }


    public static class ChannelAlias
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
                    value = "OUT";
                    break;
                case "05":
                    value = "OUT";
                    break;

            }

            return value;
        }
    }
}
