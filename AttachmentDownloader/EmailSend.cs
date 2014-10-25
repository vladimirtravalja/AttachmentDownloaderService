using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.ComponentModel;

namespace SendEmail
{
    public class EmailSend
    {

        private string smtpClient_ = "";
        private string from_ = "";
        private string to_ = "";
        private string subject_ = "";
        private string body_ = "";
        private string attachment_ = "";
        private int port_ = 0;
        private string credentialName_ = "";
        private string credentialPasswd_ = "";
        private string bcc_ = "";
        private string cc_ = "";
        private string selectInbox_ = "";

        /// <summary>
        /// SMTPClient property - SMTP URL - server/host
        /// </summary>
        public string SMTPClient
        {
            get { return smtpClient_; }
            set { smtpClient_ = value; }
        }
        /// <summary>
        /// From property - sending email address / sender
        /// </summary>
        public string From
        {
            get { return from_; }
            set { from_ = value; }
        }
        /// <summary>
        /// TO property - to email address / recipient
        /// </summary>
        public string To
        {
            get { return to_; }
            set { to_ = value; }
        }
        /// <summary>
        /// Subject property - title of the email message
        /// </summary>
        public string Subject
        {
            get { return subject_; }
            set { subject_ = value; }
        }
        /// <summary>
        /// Body property - email content / message
        /// </summary>
        public string Body
        {
            get { return body_; }
            set { body_ = value; }
        }
        /// <summary>
        /// Attachement property - path of the attachment file
        /// </summary>
        public string Attachment
        {
            get { return attachment_; }
            set { attachment_ = value; }
        }
        /// <summary>
        /// Port property - SMTP server port number for communication
        /// </summary>
        public int Port
        {
            get { return port_; }
            set { port_ = value; }
        }
        /// <summary>
        /// CredentialName property - username for loging into SMTP server
        /// </summary>
        public string CredentialName
        {
            get { return credentialName_; }
            set { credentialName_ = value; }
        }
        /// <summary>
        /// CredentialPasswd property - password for loging into SMTP server
        /// </summary>
        public string CredentialPasswd
        {
            get { return credentialPasswd_; }
            set { credentialPasswd_ = value; }
        }
        /// <summary>
        /// Bcc property - add email address to blind carbon copy
        /// </summary>
        public string Bcc
        {
            get { return bcc_; }
            set { bcc_ = value; }
        }
        /// <summary>
        /// Cc property - add email address to carbon copy
        /// </summary>
        public string Cc
        {
            get { return cc_; }
            set { cc_ = value; }
        }
        public string SelectInbox
        {
            get { return selectInbox_; }
            set { selectInbox_ = value; }
        }
        /// <summary>
        /// SendEmail method - for sending email message
        /// </summary>
        public string SendEmail()
        {
            MailMessage mail = new MailMessage();
            SmtpClient client = new SmtpClient(SMTPClient, Port);
            try
            {
                //client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Port = Port;
                mail.From = new MailAddress(From);
                // ovdje bi trebalo dodati foreach za slanje multiple address odvojene ";".

                //string [] toAddress = To.Split(';');
                //foreach (string toAdd in toaddress) {
                //mail.To.Add(toAdd);
                mail.To.Add(To);
                mail.Body = Body;
                mail.Subject = Subject;
                System.Net.Mail.Attachment attachment;
                if (!String.IsNullOrEmpty(Attachment))
                {
                    attachment = new System.Net.Mail.Attachment(Attachment);
                    mail.Attachments.Add(attachment);
                }
                client.Credentials = new System.Net.NetworkCredential(CredentialName, CredentialPasswd);
                client.Send(mail);
                //}
                return "250";
            }
            // vidi samo koji od catch exceptiona odgovara odnosno ti treba... 
            catch (SmtpFailedRecipientException exc)
            {
                return exc.StatusCode.ToString();
            }
            catch (SmtpException ex)
            {
                return ex.StatusCode.ToString();
            }
        }
    }
}
