using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TmcSettler.ConsoleApp.Services
{
    public class Settings
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
    }
}
