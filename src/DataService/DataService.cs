using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using log4net;
using DataManager;

namespace DataService
{
    public partial class DataService : ServiceBase
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
   
        public DataService()
        {
            InitializeComponent();
            MongoTXO.Instance.init();
            
        }

        protected override void OnStart(string[] args)
        {
            logger.Info("demoservice started");
            doWork();
        }

        private void doWork()
        {
        
        }   

        protected override void OnStop()
        {
            logger.Info("demoservice stoped");
        }

        protected override void OnContinue()
        {
            logger.Info("demoservice is continuing in working");
        }


    }
}
