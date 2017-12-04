using LoggerHelper.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TmcSettler.ConsoleApp
{
    public abstract class BaseContainer
    {
        protected  readonly Logger logger ;

        public BaseContainer()
        {
        }
        public BaseContainer(Logger logger) :this()
        {
            this.logger = logger;
        }

    }
}
