using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Configuration;
using System.IO;

namespace AttachmentDownloader
{
    public partial class Service1 : ServiceBase
    {

        ExchangeClass exchange = new ExchangeClass();
        //string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timer1.Start();
        }

        protected override void OnStop()
        {
            timer1.Stop();
            this.Dispose();
        }

        private void GetValues()
        {
            exchange.Username = "";// System.Configuration.ConfigurationManager.AppSettings["username"];
            exchange.Password = "";//System.Configuration.ConfigurationManager.AppSettings["password"];
            exchange.ExchangeType = "Exchange2010_SP1";//System.Configuration.ConfigurationManager.AppSettings["exchangeType"];
            timer1.Interval = Convert.ToInt32(5000);//System.Configuration.ConfigurationManager.AppSettings["timer"]);
            exchange.Path = "";//System.Configuration.ConfigurationManager.AppSettings["path"];
            exchange.FilterSender = "";//System.Configuration.ConfigurationManager.AppSettings["fromFilter"];
            exchange.FilterExtension = ".xls";//System.Configuration.ConfigurationManager.AppSettings["extension"];
            exchange.LogPath = "";//System.Configuration.ConfigurationManager.AppSettings["logPath"];
            exchange.LogName = "";//System.Configuration.ConfigurationManager.AppSettings["logName"];
            exchange.EmailFetch = Convert.ToInt32(15);//System.Configuration.ConfigurationManager.AppSettings["fetch"]);
        }

        private void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timer1.Stop();
            GetValues();
            exchange.GetAttachments();
            timer1.Start();
        }
    }
}
