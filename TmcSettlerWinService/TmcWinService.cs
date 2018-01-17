using DALContext;
using LoggerHelper.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TmcOperationService;

namespace TmcWinServiceWinService
{
    public partial class TmcWinService : ServiceBase
    {
        private Logger logger;

        int count = 0;


     
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);
        private System.Timers.Timer timer;
        private System.Timers.Timer timer2;
        private object _timerLock = new object();
        private object _timerLock2 = new object();

        public TmcWinService()
        {
            logger = new Logger();
            InitializeComponent();
            Settings.LoadSettings();
        }

        protected override void OnStart(string[] args)
        {

            //Check for Time Processing 
            if (DateTime.Now.Hour >= 1 && DateTime.Now.Hour < 4)
            {
                //Check if there is  a closed SettleBatch--- if Closed for the day, settlement has been completed.Assumed
                // E_SETTLE_BATCH settle_batch = new E_SETTLE_BATCH();

                var settle_batch = Settlement.GetSettleBatch();

                if (settle_batch.CLOSED == null || settle_batch == null)
                {
                    string batchID = Settlement.SetSettleBatch();

                    using (var db = new EtzbkDataContext())
                    {
                        db.Database.ExecuteSqlCommand("UPDATE E_SETTLEMENT_DOWNLOAD_BK SET  SETTLE_BATCH ='" + batchID + "' WHERE SETTLE_BATCH IS NULL");
                        db.Database.ExecuteSqlCommand("UPDATE E_FEE_DETAIL_BK SET  SETTLE_BATCH ='" + batchID + "' WHERE SETTLE_BATCH IS NULL");
                        db.Database.ExecuteSqlCommand("UPDATE E_SETTLE_BATCH SET CLOSED='1' WHERE BATCH_ID='" + batchID + "'");
                    }

                    //Set settlebatch on strations with Settlebatch null and less than today's date
                }

            }
            //

            logger.LogInfoMessage("Service Time started at" + DateTime.Now.ToString());
           // System.IO.Directory.CreateDirectory("C:/Users/ope/Documents/tmc/tmc/Opeyemi Folder");
            // Update the service state to Start Pending.  
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            logger.LogInfoMessage(nameof(TmcWinService) + "  starting TMC settler.....");
            logger.LogInfoMessage(nameof(TmcWinService) + "  Start Job schedule for ");


            this.timer = new System.Timers.Timer();
            this.timer.AutoReset = true;
            this.timer.Interval = this.timer.Interval = Settings.timer_interval * 60000;
            this.timer.Elapsed += new System.Timers.ElapsedEventHandler(this.Timer_Elapsed);

            this.timer.Enabled = true;
            this.timer.Start();


            this.timer2 = new System.Timers.Timer();
            this.timer2.AutoReset = true;
            this.timer2.Interval = this.timer.Interval = Settings.timer_interval * 60000;
            this.timer2.Elapsed += new System.Timers.ElapsedEventHandler(this.Timer2_Elapsed);

            this.timer2.Enabled = true;
            this.timer2.Start();

            StartMethod();
            StartSettler();

            logger.LogInfoMessage(nameof(TmcWinService) + "  Timer Started ");

            // Update the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (_timerLock)
            {
                count = +1;
                logger.LogInfoMessage("Service Running on round....." + count);
                logger.LogInfoMessage("Service Time started at" + DateTime.Now.ToString());
                logger.LogInfoMessage(nameof(TmcWinServiceWinService) + "  starting a new round at time." + DateTime.Now.ToString());
                StartMethod();
            }
        }
        private void Timer2_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (_timerLock2)
            {
                count = +1;
                logger.LogInfoMessage("Settler service Running on round....." + count);
                logger.LogInfoMessage("Settler Service Time started at" + DateTime.Now.ToString());

                StartSettler();

            }
        }
        public void StartSettler()
        {

            logger.LogInfoMessage(nameof(TmcWinService) + "Instantiating TaskProducerConsumer  Threads ");
            TaskProducerConsumer taskProducerConsumer = new TaskProducerConsumer();
            Thread taskProducerConsumerThread = new Thread(new ThreadStart(taskProducerConsumer.Run));
            logger.LogInfoMessage(nameof(TmcWinService) + "Starting the TaskProducerConsumer  Threads ");
            taskProducerConsumerThread.Start();

        }
        protected override void OnStop()
        {
            // Update the service state to Start Pending.  
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            this.timer.Stop();
            this.timer = null;

            this.timer2.Stop();
            this.timer2 = null;

            logger.LogInfoMessage(nameof(TmcWinServiceWinService) + "  service stopped at " + DateTime.Now.ToString());

            

            // Update the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        private void StartMethod()
        {

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
             

            logger.LogInfoMessage(nameof(TmcWinService) + " starting TMC settler.....");
            logger.LogInfoMessage(nameof(TmcWinService) + " Start Job schedule for ");

            logger.LogInfoMessage(nameof(TmcWinService) + "Creating multithreading Threads to spool from 145 to e_transaction for each channel");


            logger.LogInfoMessage(nameof(TmcWinService) + "Instantiating EtranzactChannelTransaction  Threads ");
            EtranzactChannelTransaction etzTrx = new EtranzactChannelTransaction();
            Thread etzTrxThread = new Thread(new ThreadStart(etzTrx.Run));
            logger.LogInfoMessage(nameof(TmcWinService) + "Starting the EtranzactChannelTransaction  Threads ");
            etzTrxThread.Start();

            logger.LogInfoMessage(nameof(TmcWinService) + "Instantiating NonEtzCardTransaction  Threads ");
            NonEtzCardTransaction nonEtzCard = new NonEtzCardTransaction();
            Thread nonEtzCardThread = new Thread(new ThreadStart(nonEtzCard.Run));
            logger.LogInfoMessage(nameof(TmcWinService) + "Starting the NonEtzCardTransaction  Threads ");
            nonEtzCardThread.Start();

            logger.LogInfoMessage(nameof(TmcWinService) + "Instantiating MastercardTransactions  Threads ");
            MastercardTransactions mastercardTrx = new MastercardTransactions();
            Thread mastercardTrxThread = new Thread(new ThreadStart(mastercardTrx.Run));
            logger.LogInfoMessage(nameof(TmcWinService) + "Starting the MastercardTransactions  Threads ");
            mastercardTrxThread.Start();


            //mastercardTrxThread.Join();
            //nonEtzCardThread.Join();
            //etzTrxThread.Join();

            stopwatch.Stop();




        }

        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public int dwServiceType;
            public ServiceState dwCurrentState;
            public int dwControlsAccepted;
            public int dwWin32ExitCode;
            public int dwServiceSpecificExitCode;
            public int dwCheckPoint;
            public int dwWaitHint;
        };

    }
}
