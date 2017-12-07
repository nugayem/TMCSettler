using System;
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

        public decimal CalculateFeeBeneficiary( decimal ratio , decimal TrnxFee)
        {
            decimal fee = 0;

            fee = (ratio / 100) * TrnxFee;
            
            return fee;


        }
        public void splitCardLoad(E_TRANSACTION e_transaction, List<E_CARDLOAD_COMMISSION_SPLIT> splitFormular)
        {
            if (e_transaction == null)
            {
                throw new ArgumentNullException(nameof(e_transaction));
            }
            else
            {

                string bankCode = e_transaction.CARD_NUM.Substring(1, 3);

                var items = from card in splitFormular
                            where card.BANK_CODE == bankCode
                            select card;

                if(items.Count() > 0)
                {
                    foreach(var item in items)
                    {
                        decimal fee = CalculateFeeBeneficiary(item.RATIO, e_transaction.FEE);
                         
                        //Compute E_feeDetail Value
                        if(item.COMM_SUSPENCE==null | item.COMM_SUSPENCE.Trim() == "")
                        {

                        }

                        E_TRANSACTION eff_Detail = new E_TRANSACTION() { CARD_NUM= item.COMM_SUSPENCE==null || item.COMM_SUSPENCE.Trim() == "" ? bankCode + ChannelAlias.GetChannelAlias(e_transaction.CHANNELID)+ "9999", CHANNELID=item.ch };

                       

                    }
                }
                else
                {
                    items = splitFormular.Where(def => def.BANK_CODE == "000");
                    foreach (var item in items)
                    {
                        decimal fee = CalculateFeeBeneficiary(item.RATIO, e_transaction.FEE);
                    }

                }
                

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
