using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Exchange;
using Microsoft.Exchange.WebServices;
using Microsoft.Exchange.WebServices.Data;
using System.Threading;
using System.IO;
using System.IO.Compression;
using SendEmail;

namespace AttachmentDownloader
{
    class ExchangeClass
    {
        #region variables with private modifier
        private string path_ = "";
        private string filterSender_ = "";
        private string subject_ = "";
        private string id_ = "";
        private string username_ = "";
        private string password_ = "";
        private string exchange_ = "";
        private string filterExcel_ = "";
        private string attachmentName_ = "";
        private int emailFetch_ = 0;
        private DateTime current_;
        private string logOutput = "";
        private string logPath_ = "";
        private string logName_ = "";
        private string exchangeType_;
        private static readonly ExtendedPropertyDefinition PidTagMessageSizeExtended = new ExtendedPropertyDefinition(0xe08, MapiPropertyType.Long);
        private double mailboxCurrentSize = 0.00;
        #endregion

        /// <summary>
        /// Path property - for setting download path for attachment, type string
        /// </summary>
        public string Path
        {
            get { return path_; }
            set { path_ = value; }
        }
        /// <summary>
        /// FilterSender property - used for filtering by email address - type string
        /// </summary>
        public string FilterSender
        {
            get { return filterSender_; }
            set { filterSender_ = value; }
        }
        /// <summary>
        /// Subject property - email subject used for log output - type string
        /// </summary>
        public string Subject
        {
            get { return subject_; }
            set { subject_ = value; }
        }
        /// <summary>
        /// ID property - generally not used for now, ID of email header - type string
        /// </summary>
        public string ID
        {
            get { return id_; }
            set { id_ = value; }
        }
        /// <summary>
        /// Username property - username used for logging into exchange server - type string
        /// </summary>
        public string Username
        {
            get { return username_; }
            set { username_ = value; }
        }
        /// <summary>
        /// Password property - used for logging into exchange server - type string
        /// </summary>
        public string Password
        {
            get { return password_; }
            set { password_ = value; }
        }
        /// <summary>
        /// Exchange property - used in autodiscovery of exchange webservice, usually it is the same as email address / domain - type string
        /// </summary>
        public string Exchange
        {
            get { return exchange_; }
            set { exchange_ = value; }
        }
        /// <summary>
        /// FilterExtension property - used for filtering attachment by extension - type string
        /// </summary>
        public string FilterExtension
        {
            get { return filterExcel_; }
            set { filterExcel_ = value; }
        }
        /// <summary>
        /// AttachmentName property - used for deifining attachment name, used for logging purpose - type string
        /// </summary>
        public string AttachementName
        {
            get { return attachmentName_; }
            set { attachmentName_ = value; }
        }
        /// <summary>
        /// EmailFetch property - used for defining how many emails should application fetch - type integer
        /// </summary>
        public int EmailFetch
        {
            get { return emailFetch_; }
            set { emailFetch_ = value; }
        }
        /// <summary>
        /// CurrentDate property - used for output/logging purpose - type datetime
        /// </summary>
        public DateTime CurrentDate
        {
            get { return current_; }
            set { current_ = value; }
        }
        /// <summary>
        /// LogPath property - used for defining path of storing log file - type string
        /// </summary>
        public string LogPath
        {
            get { return logPath_; }
            set { logPath_ = value; }
        }
        /// <summary>
        /// LogName property - used for defining log name - type string
        /// </summary>
        public string LogName
        {
            get { return logName_; }
            set { logName_ = value; }
        }
        /// <summary>
        /// ExchangeType property - used for defining which version of Exchange server is used for communication - type string
        /// </summary>
        public string ExchangeType
        {
            get { return exchangeType_; }
            set { exchangeType_ = value; }
        }

        ExchangeService serv;
        /// <summary>
        /// SetExchangeType method - void - switch/case for defining exchange server type
        /// </summary>
        private void SetExchangeType()
        {
            switch (ExchangeType)
            {
                case "Exchange2007_SP1":
                    serv = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
                    break;
                case "Exchange2010":
                    serv = new ExchangeService(ExchangeVersion.Exchange2010);
                    break;
                case "Exchange2010_SP1":
                    serv = new ExchangeService(ExchangeVersion.Exchange2010_SP1);
                    break;
                case "Exchange2010_SP2":
                    serv = new ExchangeService(ExchangeVersion.Exchange2010_SP2);
                    break;
                case "Exchange2013":
                    serv = new ExchangeService(ExchangeVersion.Exchange2013);
                    break;
            }
        }
        /// <summary>
        /// GetAttachments method - void used for getting the attachments from exchange server
        /// </summary>
        /// <param name="form">Form1 form object passing as a reference</param>
        public void GetAttachments()
        {
            try
            {
                if (!Directory.Exists(Path))
                {
                    Directory.CreateDirectory(Path);
                }
                SetExchangeType();
                serv.Credentials = new WebCredentials(Username, Password);
                serv.AutodiscoverUrl("");

                mailboxCurrentSize = GetMailBoxSize();

                if (mailboxCurrentSize > 450)
                {
                    SendEmail("Obavjest.", "Mailbox je gotovo pun, molimo provjerite i izbrišite stare mail-ove kako bi oslobodili mjesta...!");
                }

                ItemView view = new ItemView(EmailFetch);
                FindItemsResults<Item> result = serv.FindItems(WellKnownFolderName.Inbox, new ItemView(EmailFetch));

                if (result != null && result.Items != null && result.Items.Count > 0)
                {
                    foreach (Item item in result.Items)
                    {
                        EmailMessage msg = EmailMessage.Bind(serv, item.Id, new PropertySet(BasePropertySet.IdOnly, ItemSchema.Attachments, ItemSchema.HasAttachments,ItemSchema.DateTimeReceived, EmailMessageSchema.From, EmailMessageSchema.Sender));
                        if (msg.Sender.ToString().ToLower().Contains(FilterSender) && msg.From.ToString().ToLower().Contains(FilterSender))
                        {
                            foreach (Attachment att in msg.Attachments)
                            {
                                if (att is FileAttachment)
                                {
                                    FileAttachment file = att as FileAttachment;
                                    if (file.Name.Contains(FilterExtension.ToUpper()) && file.Name.Contains("LOG_FILE"))
                                    {
                                        file.Load(Path + "\\" + file.Name.Replace(FilterExtension.ToUpper(), "_" + msg.DateTimeReceived.ToString("yyyyMMddHHmmss") + FilterExtension.ToUpper()));
                                        if(File.Exists(Path + "\\" + file.Name.Replace(FilterExtension.ToUpper(), "_" + msg.DateTimeReceived.ToString("yyyyMMddHHmmss") + FilterExtension.ToUpper())))
                                        {
                                            logOutput = "[Log - Execution Time] - " + DateTime.Now.ToString() + "\nSender: \"" + FilterSender + "\" || Subject: \"" + item.Subject.ToString() + "\" || Attachment: \"" + file.Name.ToString() + "\" || Path: \"" + Path + "\\" + "\" || Status: \"Success.\"";
                                            LogRotate(logOutput);
                                            item.Delete(DeleteMode.MoveToDeletedItems);
                                        }
                                        else
                                        {
                                            file.Load(Path + "\\" + file.Name.Replace(FilterExtension.ToUpper(), "_" + msg.DateTimeReceived.ToString("yyyyMMddHHmmss") + FilterExtension.ToUpper()));
                                            logOutput = "[Log - Execution Time] - " + DateTime.Now.ToString() + "\nSender: \"" + FilterSender + "\" || Subject: \"" + item.Subject.ToString() + "\" || Attachment: \"" + file.Name.ToString() + "\" || Path: \"" + Path + "\\" + "\" || Status: \"Success.\"";
                                            LogRotate(logOutput);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logOutput = "Execution Time: \"" + DateTime.Now.ToString() + "\" || Sender: \"" + FilterSender + "\" || Subject: \"" + Subject + "\" || Attachment: \"" + FilterSender + "\" || Content:"+AttachementName+" || Path: \"" + Path + "\\" + "\" || Status: \"Failed.\" " + " || Error message: \"" + e.Message + "\"\r\n";
                LogRotate(logOutput);
                SendEmail("Extracting attachment failed...!", logOutput);
            }
        }
        /// <summary>
        /// LogRotate method - void - used for appending new log output into file and rotate log (if log is greater than 10MB it will compress current one, delete old and create new log)
        /// </summary>
        /// <param name="input">input string is log output</param>
        private void LogRotate(string input)
        {
            if (Directory.Exists(LogPath))
            {
                if (File.Exists(LogPath + "\\" + LogName))
                {
                    CompressAndRotate(input);
                }
                else
                {
                    CreateFile(input, LogPath, LogName);
                }
            }
            else
            {
                Directory.CreateDirectory(LogPath);
                if (File.Exists(LogPath + "\\" + LogName))
                {
                    CompressAndRotate(input);
                }
                else
                {
                    CreateFile(input, LogPath, LogName);
                }
            }

        }
        /// <summary>
        /// CompressAndRotate method - checks for filesize and if it is larger
        /// </summary>
        /// <param name="input">input represents log output</param>
        private void CompressAndRotate(string input)
        {
            FileInfo fi = new FileInfo(LogPath + "\\" + LogName);
            if (fi.Length >= 10485760)
            {
                CompressFile(fi);
                fi.Delete();
                LogRotate(input);
            }
            else
            {
                using (StreamWriter sw = File.AppendText(LogPath + "\\" + LogName))
                {
                    sw.WriteLine(input);
                }
            }
        }
        /// <summary>
        /// CompressFile method - void - used for compressing file that is greater than 10MB
        /// </summary>
        /// <param name="input">input represents - log file path</param>
        private void CompressFile(FileInfo input)
        {
            using (FileStream inFile = input.OpenRead())
            {
                if ((File.GetAttributes(LogPath) & FileAttributes.Hidden) != FileAttributes.Hidden & input.Extension != ".gz")
                {
                    string logName = DateTime.Now.ToString("yyyyMMddHHmmss");
                    using (FileStream outFile = File.Create(logName + ".gz"))
                    {
                        using (GZipStream compress = new GZipStream(outFile, CompressionMode.Compress))
                        {
                            inFile.CopyTo(compress);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// CreateFile method - void - used for creating new file if file is not preset.
        /// </summary>
        /// <param name="input">input represents - log output</param>
        /// <param name="inputPath">inputPath represents - path for log file</param>
        /// <param name="inputName">inputName represents - name of the log file</param>
        private void CreateFile(string input, string inputPath, string inputName)
        {
            File.Create(inputPath + "\\" + inputName).Close();
            using (StreamWriter sw = File.AppendText(inputPath + "\\" + inputName))
            {
                sw.WriteLine(input);
                sw.Close();
            }
        }
        /// <summary>
        /// GetMailBoxSize method - recursion through all email folders and calculate the size of all folders.
        /// </summary>
        /// <returns></returns>
        private double GetMailBoxSize()
        {
            var offset = 0;
            const int pagesize = 12;
            long size = 0;

            FindFoldersResults folders;
            do
            {
                folders = serv.FindFolders(WellKnownFolderName.MsgFolderRoot,
                                              new FolderView(pagesize, offset, OffsetBasePoint.Beginning)
                                              {
                                                  Traversal = FolderTraversal.Deep,
                                                  PropertySet =
                                                      new PropertySet(BasePropertySet.IdOnly, PidTagMessageSizeExtended,
                                                                      FolderSchema.DisplayName)
                                              });
                foreach (var folder in folders)
                {
                    long folderSize;
                    if (folder.TryGetProperty(PidTagMessageSizeExtended, out folderSize))
                    {
                        size += folderSize;
                    }
                }
                offset += pagesize;
            } while (folders.MoreAvailable);
            double value = size / 1048576;
            return value;
        }
        /// <summary>
        /// SendEmail - method for sending email notification
        /// </summary>
        /// <param name="log"></param>
        private void SendEmail(string subject, string body)
        {
            EmailSend email = new EmailSend();
            email.From = "";
            email.To = "";
            email.Subject = subject;
            email.Body = body;
            email.Port = 25;
            email.SMTPClient = "";
            email.SendEmail();
        }
    }
}
