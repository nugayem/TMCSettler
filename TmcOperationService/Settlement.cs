using DALContext;
using DALContext.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TmcOperationService
{
    public class Settlements
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
                    keyID = batchnumber.KEYID + 1;

                    if (keyID == 999998)
                        keyID = 1;
                    batchnumber = new E_FEEBATCH() { KEYID = keyID };
                    etzbk.E_FEEBATCH.Add(batchnumber);

                }
                else
                {
                    keyID = 1;
                    batchnumber = new E_FEEBATCH() { KEYID = 1 };
                    etzbk.E_FEEBATCH.Add(batchnumber);
                }

                String key = keyID.ToString("D6");
                int mm = int.Parse(currentTime.Substring(2, 2));
                int dd = int.Parse(currentTime.Substring(4, 2));
                int hh = int.Parse(currentTime.Substring(6, 2));
                if (mm == 0)
                    mm = 1;
                if (dd == 0)
                    dd = 1;
                if (hh == 0)
                    hh = 1;
                finalkey = currentTime.Substring(0, 2) + month[mm - 1]
                      + day[dd - 1] + hour[hh - 1] + currentTime.Substring(8)
                      + key;
                etzbk.SaveChanges();
            }
            catch (Exception _e)
            {
                Console.WriteLine(_e.Message);
            }
            return finalkey;
        }



    }
}
