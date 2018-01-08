using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace TmcOperationService
{
    public class MailMessanger
    {
        private readonly static MailMessage mail = new MailMessage();

        public static void SendMail(string to, string to2 , string subject, string body)
        {
            mail.To.Add(new MailAddress(to));
            mail.To.Add(new MailAddress(to2));
            mail.CC.Add(new MailAddress(Settings.mailOperations));
            mail.From = new MailAddress(Settings.fromEmail);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient(Settings.host);
            smtp.EnableSsl = Settings.ssl;

            NetworkCredential network = new NetworkCredential(Settings.username, Settings.password);
            smtp.UseDefaultCredentials = true;
            smtp.Port = Settings.port;

            //smtp.Credentials= 
            //smtp.Send(mail);
        }
    }
}
