//using AppModel.Models;
using CommonUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace CommonUtils
{
    public class Mailer
    {
        public string DisplayName = string.Empty;
        public string FromAddress = string.Empty;
        public string Host = string.Empty;
        public string UserName = string.Empty;
        public string Password = string.Empty;
        public int Port;
        public bool EnableSsl;
        public bool IsBodyHtml;
        public bool UseDefaultCredentials;

        public string Head { get; set; }
        public string Type { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        //Add the Creddentials- use your own email id and password
        public MailMessage mailmessage = null;
        public SmtpClient client = null;

        public Mailer
        (
            string DisplayName = "",
            string FromAddress = "",
            string Host = "",
            int? Port = null,
            string UserName = "",
            string Password = "",
            bool? EnableSsl = null,
            bool? IsBodyHtml = null,
            bool? UseDefaultCredentials = null
        )
        {
            this.DisplayName = (!string.IsNullOrEmpty(DisplayName)) ? DisplayName : Utils.MailServerItemValue("DisplayName");
            this.FromAddress = (!string.IsNullOrEmpty(FromAddress)) ? FromAddress : Utils.MailServerItemValue("FromMail");
            this.Host = (!string.IsNullOrEmpty(Host)) ? Host : Utils.MailServerItemValue("Host");
            this.Port = (Port != null) ? Port.Value : Utils.MailServerItemIntegerValue("Port");
            this.UserName = (!string.IsNullOrEmpty(UserName)) ? UserName : Utils.MailServerItemValue("Username");
            this.Password = (!string.IsNullOrEmpty(Password)) ? Password : Utils.MailServerItemValue("Password");
            this.EnableSsl = (EnableSsl != null) ? EnableSsl.Value : Utils.MailServerItemBooleanValue("EnableSsl");
            this.IsBodyHtml = (IsBodyHtml != null) ? IsBodyHtml.Value : Utils.MailServerItemBooleanValue("IsBodyHtml");
            this.UseDefaultCredentials = (UseDefaultCredentials != null) ? UseDefaultCredentials.Value : Utils.MailServerItemBooleanValue("UseDefaultCredentials");
        }
        public void FillValues(List<Utility> valuecollection)
        {
            if (valuecollection == null) return;
            foreach (Utility item in valuecollection)
            {
                Body = Body.Replace(item.Key, item.Value);
            }
            Body = Body.Replace("<SiteName>", Utils.GetAppSettingValue("SiteName"));
            Body = Body.Replace("<RootUrl>", Utils.RootUrl);
        }
        public void SendMailMessage(string toAddress, string subject, string body, MailPriority priority = MailPriority.Normal)
        {
            SendMailMessage(this.FromAddress, toAddress, subject, body, priority);
        }
        public void SendMailMessage(string fromAddress, string toAddress, string subject, string body, MailPriority priority = MailPriority.Normal)
        {
            SendMailMessage(this.FromAddress, new List<string> { toAddress }, subject, body, priority);
        }
        public void SendMailMessage(List<string> toAddresses, string subject, string body, MailPriority priority = MailPriority.Normal)
        {
            SendMailMessage(this.FromAddress, toAddresses, subject, body, priority);
        }
        public void SendMailMessage(string fromAddress, List<string> toAddresses, string subject, string body, MailPriority priority = MailPriority.Normal)
        {
            mailmessage = new MailMessage();
            mailmessage.From = new MailAddress(fromAddress);
            foreach (string toAddress in toAddresses)
            {
                mailmessage.To.Add(new MailAddress(toAddress));
            }
            mailmessage.Subject = subject;
            mailmessage.BodyEncoding = System.Text.Encoding.UTF8;
            mailmessage.Body = body;
            mailmessage.Priority = priority;
            mailmessage.IsBodyHtml = this.IsBodyHtml;

            try
            {
                //mailmessage.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", "1");
                //mailmessage.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusername", this.UserName);
                //mailmessage.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendpassword", this.Password);

                client = new SmtpClient();
                client.Host = this.Host;
                client.Port = this.Port;
                client.EnableSsl = this.EnableSsl;
                //oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;
                if (this.UseDefaultCredentials)
                {
                    client.Credentials = new NetworkCredential(this.UserName, this.Password);
                }
                client.Send(mailmessage);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void SendMailMessageWithAttachment(List<string> toAddresses, string subject, string body, byte[] bytes, string attachmentName, MailPriority priority = MailPriority.Normal)
        {
            SendMailMessageWithAttachment(this.FromAddress, toAddresses, subject, body, bytes, attachmentName, priority);
        }
        public void SendMailMessageWithAttachment(string fromAddress, List<string> toAddresses, string subject, string body, byte[] bytes, string attachmentName, MailPriority priority = MailPriority.Normal)
        {



            mailmessage = new MailMessage();
            mailmessage.From = new MailAddress(fromAddress);
            foreach (string toAddress in toAddresses)
            {
                mailmessage.To.Add(new MailAddress(toAddress));
            }
            mailmessage.Subject = subject;
            mailmessage.BodyEncoding = System.Text.Encoding.UTF8;
            mailmessage.Body = body;
            mailmessage.Priority = priority;
            mailmessage.IsBodyHtml = this.IsBodyHtml;
            System.IO.MemoryStream mstream = new System.IO.MemoryStream(bytes);
            mailmessage.Attachments.Add(new Attachment(mstream, attachmentName));
            

            try
            {
                //mailmessage.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", "1");
                //mailmessage.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusername", this.UserName);
                //mailmessage.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendpassword", this.Password);

                client = new SmtpClient();
                client.Host = this.Host;
                client.Port = this.Port;
                client.EnableSsl = this.EnableSsl;
                if (this.UseDefaultCredentials)
                {
                    client.Credentials = new NetworkCredential(this.UserName, this.Password);
                }
                client.Send(mailmessage);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                mstream.Close();
                mstream.Dispose();
            }
        }


    }

  
}