using DALContext;
using DALContext.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TmcSettler.ConsoleApp.Services
{
    public class Settlement
    {
        public static String GenerateBatch()
        {

            EtzbkDataContext etzbk = new EtzbkDataContext();


            string currentTime = DateTime.Now.ToString("yyMMddHHmm");
            string[] month = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J",
                "K", "L" };
            string[] day = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K",
                "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W",
                "X", "Y", "Z", "1", "2", "3", "4", "5" };
            string[] hour = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J",
                "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V",
                "W", "X" };
            string finalkey = "";
            try
            {
                int keyID = 0;
                var batchnumber = etzbk.E_FEEBATCH.FirstOrDefault();
                if (batchnumber != null)
                {
                    keyID = batchnumber.KEYID;

                    if (keyID == 999998)
                        keyID = 1;
                    batchnumber = new E_FEEBATCH() { KEYID = keyID + 1 };
                    etzbk.E_FEEBATCH.Add(batchnumber);

                }
                else
                {
                    batchnumber = new E_FEEBATCH() { KEYID = 1 };
                    etzbk.E_FEEBATCH.Add(batchnumber);
                }

                String key = keyID.ToString("D6");
                int mm = int.Parse(currentTime.Substring(2, 4));
                int dd = int.Parse(currentTime.Substring(4, 6));
                int hh = int.Parse(currentTime.Substring(6, 8));
                if (mm == 0)
                    mm = 1;
                if (dd == 0)
                    dd = 1;
                if (hh == 0)
                    hh = 1;
                finalkey = currentTime.Substring(0, 2) + month[mm - 1]
                      + day[dd - 1] + hour[hh - 1] + currentTime.Substring(8)
                      + key;

            }
            catch (Exception _e)
            {
                
            }
            return finalkey;
        }



    }
}
