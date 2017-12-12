using DALContext;
using DALContext.Model;
using LoggerHelper.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TmcSettler.ConsoleApp.Services
{
    public static class Settings 
    {
        public static readonly int number_of_record_perround;
        public static readonly int number_of_backlogdays;
        public static readonly string[] successKeys = new string[] { "00", "000" };
        public static DateTime startdate;
        static Settings()
        {
            number_of_record_perround = int.Parse(ConfigurationManager.AppSettings["number_of_record_perround"]);
            number_of_backlogdays = int.Parse(ConfigurationManager.AppSettings["number_of_record_round"]);
            startdate = DateTime.Today.AddDays(-Settings.number_of_backlogdays);
        }

        public static void GenerateBatchID()
        {
//            string [] alphabet= {'A'}
            string year = DateTime.Now.Year.ToString("yy");
            string month = Number2String(DateTime.Now.Month, true);
            string day = Number2String(DateTime.Now.Day, true);
            string settle_batch = year + month + day;

        }
        private static String Number2String(int number, bool isCaps)

        {

            Char c = (Char)((isCaps ? 65 : 97) + (number - 1));

            return c.ToString();

        }

        public static void LoadSettings()
        {
            EtzbkDataContext etzTrx = new EtzbkDataContext();
            // var etzTrx = db.E_CARDLOAD_COMMISSION_SPLIT.ToList();
            List<E_CARDLOAD_COMMISSION_SPLIT> cardLoadSplitList = new List<E_CARDLOAD_COMMISSION_SPLIT>()
            {
                new E_CARDLOAD_COMMISSION_SPLIT (){ BANK_CODE="011", CREATED=DateTime.Now, MAIN_FLAG="0", RATIO=40, SPLIT_CARD="%9999", SPLIT_DESCR="Bank Commission" },
                new E_CARDLOAD_COMMISSION_SPLIT (){ BANK_CODE = "011", CREATED = DateTime.Now, MAIN_FLAG = "0", RATIO = 60, SPLIT_CARD = "0441234567", SPLIT_DESCR = "Bank Commission" },
                new E_CARDLOAD_COMMISSION_SPLIT (){ BANK_CODE = "033", CREATED = DateTime.Now, MAIN_FLAG = "0", RATIO = 30, SPLIT_CARD = "%9999", SPLIT_DESCR = "Bank Commission", COMM_SUSPENCE="033PAYABLE" },
                new E_CARDLOAD_COMMISSION_SPLIT (){ BANK_CODE = "033", CREATED = DateTime.Now, MAIN_FLAG = "0", RATIO = 70, SPLIT_CARD = "0441234567", SPLIT_DESCR = "Bank Commission", COMM_SUSPENCE="033PAYABLE" },
                new E_CARDLOAD_COMMISSION_SPLIT (){ BANK_CODE = "033", CREATED = DateTime.Now, MAIN_FLAG = "1", RATIO = 0, SPLIT_CARD = "0447777567", SPLIT_DESCR = "Bank Commission", COMM_SUSPENCE="033PAYABLE" },
                new E_CARDLOAD_COMMISSION_SPLIT (){ BANK_CODE = "000", CREATED = DateTime.Now, MAIN_FLAG = "0", RATIO = 50, SPLIT_CARD = "%9999", SPLIT_DESCR = "Bank Commission" },
                new E_CARDLOAD_COMMISSION_SPLIT (){ BANK_CODE = "000", CREATED = DateTime.Now, MAIN_FLAG = "0", RATIO = 50, SPLIT_CARD = "0441234567", SPLIT_DESCR = "Bank Commission" }
            };

            //EtzbkDataContext etzTrx = new EtzbkDataContext();
            // var etzTrx = db.E_CARDLOAD_COMMISSION_SPLIT.ToList();
            List<E_TRANSFER_COMMISSION_SPLIT> transferSplitList = new List<E_TRANSFER_COMMISSION_SPLIT>()
            {
                new E_TRANSFER_COMMISSION_SPLIT (){ BANK_CODE="011", CREATED=DateTime.Now, MAIN_FLAG="0", RATIO=40, SPLIT_CARD="%9999", SPLIT_DESCR="Bank Commission" },
                new E_TRANSFER_COMMISSION_SPLIT (){ BANK_CODE = "011", CREATED = DateTime.Now, MAIN_FLAG = "0", RATIO = 60, SPLIT_CARD = "0441234567", SPLIT_DESCR = "Bank Commission" },
                new E_TRANSFER_COMMISSION_SPLIT (){ BANK_CODE = "033", CREATED = DateTime.Now, MAIN_FLAG = "0", RATIO = 30, SPLIT_CARD = "%9999", SPLIT_DESCR = "Bank Commission", COMM_SUSPENCE="033PAYABLE" },
                new E_TRANSFER_COMMISSION_SPLIT (){ BANK_CODE = "033", CREATED = DateTime.Now, MAIN_FLAG = "0", RATIO = 70, SPLIT_CARD = "0441234567", SPLIT_DESCR = "Bank Commission", COMM_SUSPENCE="033PAYABLE" },                
                new E_TRANSFER_COMMISSION_SPLIT (){ BANK_CODE = "000", CREATED = DateTime.Now, MAIN_FLAG = "0", RATIO = 50, SPLIT_CARD = "%9999", SPLIT_DESCR = "Bank Commission" },
                new E_TRANSFER_COMMISSION_SPLIT (){ BANK_CODE = "000", CREATED = DateTime.Now, MAIN_FLAG = "0", RATIO = 50, SPLIT_CARD = "0441234567", SPLIT_DESCR = "Bank Commission" }
            };

//            EtzbkDataContext etzTrx = new EtzbkDataContext();
            List<E_FUNDGATE_COMMISSION_SPLIT> fundGateSplitList = etzTrx.E_FUNDGATE_COMMISSION_SPLIT.ToList();

            CachingProvider.AddItem("CardLoad", cardLoadSplitList);
            CachingProvider.AddItem("Transfer", transferSplitList);
            CachingProvider.AddItem("FundGate", fundGateSplitList);




        }
    }
}
