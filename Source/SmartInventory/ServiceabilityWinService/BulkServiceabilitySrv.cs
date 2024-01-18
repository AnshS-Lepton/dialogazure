using System;
using System.Configuration;
using System.IO;
using System.ServiceProcess;
using System.Timers;

namespace ServiceabilityWinService
{
    public partial class BulkServiceabilitySrv : ServiceBase
    {
        Timer timer = new Timer();
        public BulkServiceabilitySrv()
        {
            InitializeComponent();
        }

        private void StartProcess(object sender, ElapsedEventArgs args)
        {
            this.WriteToFile("BulkServiceability Started: {0}");
            FileProcess.BulkServiceability();
        }
        protected override void OnStart(string[] args)
        {
            try
            {
                StartProcess(null, null);
                timer.Interval = double.Parse(ConfigurationManager.AppSettings["ServiceTimer"].ToString());
                timer.Enabled = true;
                timer.Elapsed += StartProcess;
                timer.Start();
            }
            catch (Exception ex)
            {
                this.WriteToFile("OnStart Exception: " + ex.Message+ " {0}");
            }
        }
        private void WriteToFile(string text)
        {
            string path = "C:\\BulkServiceabilityLog.txt";
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(string.Format(text, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
                writer.Close();
            }
        }
        public void OnDebug()
        {
            FileProcess.BulkServiceability();
        }
        protected override void OnStop()
        {
            this.WriteToFile("Service Stopped... {0}");            
        }
    }
}
