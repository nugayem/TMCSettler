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
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);
        System.Timers.Timer timer;
        public TmcWinService()
        {
            logger = new Logger();
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // Update the service state to Start Pending.  
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            logger.LogInfoMessage(nameof(TmcWinServiceWinService) + " starting TMC settler.....");
            logger.LogInfoMessage(nameof(TmcWinServiceWinService) + " Start Job schedule for ");

            logger.LogInfoMessage(nameof(TmcWinServiceWinService) + "Creating multithreading Threads to spool from 145 to e_transaction for each channel");

            //Timer timer = new System.Timers.Timer();
            this.timer.Interval = 600000;
            this.timer.Elapsed += new System.Timers.ElapsedEventHandler(this.Timer_Elapsed);

            timer.Enabled = true;
            timer.Start();

            // Update the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            logger.LogInfoMessage(nameof(TmcWinServiceWinService) + " starting a new round at time." +  DateTime.Now.ToString());

        }
        protected override void OnStop()
        {
            // Update the service state to Start Pending.  
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);


            logger.LogInfoMessage(nameof(TmcWinServiceWinService) + " service stopped at " + DateTime.Now.ToString());

            StartMethods();

            // Update the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        private void StartMethods()
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


            mastercardTrxThread.Join();
            nonEtzCardThread.Join();
            etzTrxThread.Join();

            stopwatch.Stop();


            logger.LogInfoMessage(nameof(TmcWinService) + "Instantiating CardloadProducer  Threads ");
            CardloadProducer cardloadProducer = new CardloadProducer();
            Thread cardloadProducerThread = new Thread(new ThreadStart(cardloadProducer.Run));
            logger.LogInfoMessage(nameof(TmcWinService) + "Starting the CardloadProducer  Threads ");
            cardloadProducerThread.Start();

            logger.LogInfoMessage(nameof(TmcWinService) + "Instantiating TransferProducer  Threads ");
            TransferProducer transferdProducer = new TransferProducer();
            Thread transferProducerThread = new Thread(new ThreadStart(transferdProducer.Run));
            logger.LogInfoMessage(nameof(TmcWinService) + "Starting the TransferProducer  Threads ");
            transferProducerThread.Start();



            logger.LogInfoMessage(nameof(TmcWinService) + "Instantiating PaymentProducer  Threads ");
            PaymentProducer paymentProducer = new PaymentProducer();
            Thread paymentProducerThread = new Thread(new ThreadStart(paymentProducer.Run));
            logger.LogInfoMessage(nameof(TmcWinService) + "Starting the PaymentProducer  Threads ");
            paymentProducerThread.Start();



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
