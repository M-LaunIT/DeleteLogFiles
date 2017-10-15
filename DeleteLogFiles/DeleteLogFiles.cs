using System;
using System.Configuration;
using System.ServiceProcess;
using System.Timers;

namespace DeleteLogFiles
{
    public partial class Delete_Log_Files : ServiceBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        Timer timer = null;

        public Delete_Log_Files()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            log.Debug("DeleteLogFiles wurde gestartet - " + DateTime.Now.ToShortTimeString());
            double fetchInterval = Convert.ToDouble(ConfigurationManager.AppSettings["fetchInterval"]);
            // Set up a timer to trigger every minute.  
            timer = new Timer()
            {
                //1 Sekunde = 1000
                Interval = (fetchInterval * 60 * 1000),
            };
            timer.Elapsed += new ElapsedEventHandler(OnTimer);
            timer.Start();
            OnTimer(null, null);
        }

        protected override void OnStop()
        {
        }
        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            try
            {
                FindPathAndDeleteFiles fpad = new FindPathAndDeleteFiles();

                fpad.GetPaths();
            }
            catch (Exception ex)
            {
                log.Error("OnTimer: " + ex);
            }
        }
    }
}
