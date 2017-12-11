using LoggerHelper.Services;
using StructureMap;
using System;
using DALContext;
using System.Threading;
using System.Diagnostics;
using System.Configuration;
using TmcSettler.ConsoleApp.Services;
using DALContext.Services;


namespace TmcSettler.ConsoleApp
{
    public class Program
    {
      //  public static StructureMapDependencyResolver StructureMapResolver { get; set; }

        static void Main(string[] args)
        {
            var container = Container.For<ConsoleRegistry>();

            //StructureMapResolver = new StructureMapDependencyResolver(container);
            //DependencyResolver.SetResolver(StructureMapResolver);

            Console.WriteLine(container.WhatDidIScan());
            var app = container.GetInstance<Application>();
            app.Run();
            Console.ReadLine();

        }
    }

    public class ConsoleRegistry : Registry
    {

        public ConsoleRegistry()
        {
            Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.WithDefaultConventions();
                scan.AssembliesAndExecutablesFromApplicationBaseDirectory();
                scan.AddAllTypesOf<IRunAtStartup>();
            });

           // For<ILogger>().Use<Logger>();

            AutoMapperConfig.Execute();
            // requires explicit registration; doesn't follow convention
            // For<ILog>().Use<ConsoleLogger>();
        }
    }


    public class Application 
    {
        private ILogger logger;
        int number_of_record_perround = int.Parse(ConfigurationManager.AppSettings["number_of_record_perround"]);
        int number_of_backlogdays = int.Parse(ConfigurationManager.AppSettings["number_of_record_round"]);
        string[] successKeys = new string[] { "00", "000" };
         

        DateTime startdate;//= DateTime.Today.AddDays(-number_of_backlogdays);

        public Application(ILogger logger)
        {
            this.logger = logger;
            this.startdate = DateTime.Today.AddDays(-number_of_backlogdays);


            EtzbkDataContext etzbk = new EtzbkDataContext();

            Settings.LoadSettings();
        }

        public void Run()
        {

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            logger.LogInfoMessage(nameof(TmcSettler) + " starting TMC settler.....");
            logger.LogInfoMessage(nameof(TmcSettler) + " Start Job schedule for ");

            logger.LogInfoMessage(nameof(TmcSettler) + "Creating multithreading Threads to spool from 145 to e_transaction for each channel");


            logger.LogInfoMessage(nameof(TmcSettler) + "Instantiating EtranzactChannelTransaction  Threads ");
            EtranzactChannelTransaction etzTrx = new EtranzactChannelTransaction();
            Thread etzTrxThread = new Thread(new ThreadStart(etzTrx.Run));
            logger.LogInfoMessage(nameof(TmcSettler) + "Starting the EtranzactChannelTransaction  Threads ");
            etzTrxThread.Start();

            logger.LogInfoMessage(nameof(TmcSettler) + "Instantiating NonEtzCardTransaction  Threads ");
            NonEtzCardTransaction nonEtzCard = new NonEtzCardTransaction();
            Thread nonEtzCardThread = new Thread(new ThreadStart(nonEtzCard.Run));
            logger.LogInfoMessage(nameof(TmcSettler) + "Starting the NonEtzCardTransaction  Threads ");
            nonEtzCardThread.Start();

            logger.LogInfoMessage(nameof(TmcSettler) + "Instantiating MastercardTransactions  Threads ");
            MastercardTransactions mastercardTrx = new MastercardTransactions();
            Thread mastercardTrxThread = new Thread(new ThreadStart(mastercardTrx.Run));
            logger.LogInfoMessage(nameof(TmcSettler) + "Starting the MastercardTransactions  Threads ");
            mastercardTrxThread.Start();


            mastercardTrxThread.Join();
            nonEtzCardThread.Join();
            etzTrxThread.Join();

            stopwatch.Stop();


            logger.LogInfoMessage(nameof(TmcSettler) + "Instantiating CardloadProducer  Threads ");
            CardloadProducer cardloadProducer = new CardloadProducer();
            Thread cardloadProducerThread = new Thread(new ThreadStart(cardloadProducer.Run));
            logger.LogInfoMessage(nameof(TmcSettler) + "Starting the CardloadProducer  Threads ");
            cardloadProducerThread.Start();

            logger.LogInfoMessage(nameof(TmcSettler) + "Instantiating TransferProducer  Threads ");
            TransferProducer transferdProducer = new TransferProducer();
            Thread transferProducerThread = new Thread(new ThreadStart(transferdProducer.Run));
            logger.LogInfoMessage(nameof(TmcSettler) + "Starting the TransferProducer  Threads ");
            transferProducerThread.Start();



            logger.LogInfoMessage(nameof(TmcSettler) + "Instantiating PaymentProducer  Threads ");
            PaymentProducer paymentProducer = new PaymentProducer();
            Thread paymentProducerThread = new Thread(new ThreadStart(paymentProducer.Run));
            logger.LogInfoMessage(nameof(TmcSettler) + "Starting the PaymentProducer  Threads ");
            paymentProducerThread.Start();



            Console.WriteLine("Round completed in "+ stopwatch.Elapsed);
            Console.ReadLine();
        }

     
    


    }


}