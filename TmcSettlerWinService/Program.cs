using DALContext.Services;
using LoggerHelper.Services;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using TmcWinServiceWinService;

namespace TmcSettlerWinService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {


            Logger logger = new Logger();
            logger.LogInfoMessage("Starting Application");

            try
            {
                var container = new Container();
                container.Configure(config =>
                {
                    // Register stuff in container, using the StructureMap APIs...
                    config.Scan(scan =>
                        {
                            scan.TheCallingAssembly();
                            scan.WithDefaultConventions();
                            scan.AssembliesAndExecutablesFromApplicationBaseDirectory();
                            scan.AddAllTypesOf<IRunAtStartup>();
                        });
                    // Populate the container using the service collection               
                });

                AutoMapperConfig.Execute();


                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new TmcWinService()
                };
                ServiceBase.Run(ServicesToRun);
                logger.LogInfoMessage("Started");
            }
            catch(Exception ex) { logger.LogFatalMessage(ex.Message); }
        }
    }

}
