using DALContext;
using DALContext.Model;
using LoggerHelper.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TmcOperationService
{
    public static class Settings
    {
        public static readonly int number_of_record_perround;
        public static readonly int number_of_backlogdays;
        public static readonly int timer_interval;
        public static readonly int tmcSpoolingThreadNumber;
        public static readonly int settlementThreadNumber;
        public static readonly string mailOperations;
        public static readonly string mailTrxProcessing;
        public static readonly string mailpayoutlet;
        public static readonly string mailsettlementsupport;

        public static readonly string host;
        public static readonly int port;
        public static readonly bool ssl;
        public static readonly string fromEmail;
        public static readonly string username;
        public static readonly string password;



            public static readonly string[] successKeys = new string[] { "00", "000" };
        public static readonly string[] targets = new string[] { "FCMB", "SKYE833NODE", "SKYE", "UNITYBANK", "UNION", "UBA", "PMCBAXCHANGE-WALLET", "DIAMOND", "JAIZ_BANK", "GTBANK", "FIRSTBANK", "PMCBAXCHANGE", "ACCESS", "WEMA", "STANBIC", "ECOBANK", "ZENITHBANK", "FIDELITY", "KEYSTONE", "ABMFB", "SMARTMICRO", "MASTERPASS", "HERITAGE" };
        public static DateTime startdate;

        static Settings()
        {

            host = ConfigurationManager.AppSettings["host"];
            port = int.Parse( ConfigurationManager.AppSettings["port"]);
            ssl = bool.Parse( ConfigurationManager.AppSettings["ssl"]);
            fromEmail = ConfigurationManager.AppSettings["fromEmail"];
            username = ConfigurationManager.AppSettings["username"];
            password = ConfigurationManager.AppSettings["password"];


            number_of_record_perround = int.Parse(ConfigurationManager.AppSettings["number_of_record_perround"]);
            number_of_backlogdays = int.Parse(ConfigurationManager.AppSettings["number_of_record_round"]);
            timer_interval = int.Parse(ConfigurationManager.AppSettings["timer_interval"]);
            tmcSpoolingThreadNumber = int.Parse(ConfigurationManager.AppSettings["tmcSpoolingThreadNumber"]);
            settlementThreadNumber = int.Parse(ConfigurationManager.AppSettings["settlementThreadNumber"]);
            mailOperations = ConfigurationManager.AppSettings["mailOperations"];
            mailTrxProcessing = ConfigurationManager.AppSettings["mailTrxProcessing"];
            mailpayoutlet = ConfigurationManager.AppSettings["mailpayoutlet"];
            mailsettlementsupport = ConfigurationManager.AppSettings["mailsettlementsupport"];
            startdate = DateTime.Today.AddDays(-Settings.number_of_backlogdays);
        }
        
        public static void LoadSettings()
        {
            using (EtzbkDataContext etzTrx = new EtzbkDataContext())
            {
                try
                {
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
                catch (Exception ex)
                {
                    Logger logger = new Logger();
                    Console.WriteLine("Exception loading settings" + ExceptionExtensions.GetFullMessage(ex));
                    logger.LogDebugMessage("Exception loading settings " + nameof(Settings) + " " + ExceptionExtensions.GetFullMessage(ex));
                }

            }
        }
    }
}
