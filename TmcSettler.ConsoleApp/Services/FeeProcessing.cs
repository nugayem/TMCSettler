using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TmcSettler.ConsoleApp.Services
{
    public class FeeProcessing
    {

        public double getCalcFee(double amount)
        {
            double fee = 0.0;       
            double ld_from = 0.0;
            double ld_to = 0.0;
            double ld_value = 0.0;
            int retCode = -1;

            try
            {
                con = SimplePool.ds.getConnection();
                stmt = con.createStatement();
                ls_sql = "SELECT * FROM E_CATSCALE WHERE CAT_ID='" + getFeeCategory() + "'";
                //System.out.println("Scale:"+ls_sql);
                stmt = con.createStatement();
                rs = stmt.executeQuery(ls_sql);
                while (rs.next())
                {
                    ld_from = rs.getDouble("SCALE_FROM");
                    ld_to = rs.getDouble("SCALE_TO");
                    ld_value = rs.getDouble("SCALE_VALUE");
                    ls_type = rs.getString("SCALE_TYPE");

                    if (amount >= ld_from && amount <= ld_to)
                    {
                        fee = (ls_type.equals("1") ? ld_value / 100 * amount : ld_value); //0 - value, 1- percentage
                        retCode = 1; //0 - value, 1- percentage
                    }
                }
            }
            catch (Exception e)
            {
                retCode = -1;
            }
            finally
            {
                try
                {
                    if (rs != null) rs.close();
                    if (stmt != null) stmt.close();
                    if (con != null) con.close();
                }
                catch (Exception ee)
                {
                    ;
                }
            }

            return fee;
        }
    }
}
