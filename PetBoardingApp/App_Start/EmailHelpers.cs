using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;

namespace PetBoardingApp.App_Start
{
    public class EmailHelpers
    {

        public static MailMessage GenerateMailMessage(string destination, string subject, string body)
        {
            MailMessage mailMessage = new MailMessage(new MailAddress(EmailServiceCredentials.EmailFromAddress, EmailServiceCredentials.EmailFromName), new MailAddress(destination));
            mailMessage.Subject = EmailServiceCredentials.EmailAppName + " - " + subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;

            return mailMessage;
        }

        public static SmtpClient GetSmtpClient()
        {
            SmtpClient smtpClient = new SmtpClient(EmailServiceCredentials.EmailSMTPUrl, Convert.ToInt32(EmailServiceCredentials.PortNumber));
            smtpClient.Credentials = new NetworkCredential(EmailServiceCredentials.EmailFromAddress,EmailServiceCredentials.EmailSMTPPasswordHash);
            smtpClient.EnableSsl = true;

            return smtpClient;
        }
    }
    
}
