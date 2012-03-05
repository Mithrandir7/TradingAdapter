using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using log4net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Timers;
using System.Diagnostics;
using UtilityClass;
using System.Configuration;
using DataManager;
using SymbolClass;
using TradeManager;
using AccountClass;
using MarginClass;
using PositionClass;
using OrderClass;

namespace tradebox
{
    public partial class tradebox : Form
    {
        public bool isCloseTC = true;

        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
   
        public tradebox()
        {
            InitializeComponent();
            configuration();
            SymbolManager.Instance.init();
            QuoteAdapter.Instant.init();
            DataCenter.Instance.init();
            TradeCenter.Instance.init();
            MarginMonitor.Instance.init();
            PositionMonitor.Instance.init();
            OrderManager.Instance.init();
            RandomEntryController.Instance.init();

         
            logger.Info("Working Directory : " + Misc.getWorkingDirectory());
        }

        private void configuration()
        {
            String lCloseTC = ConfigurationManager.AppSettings["isCloseTc"].ToUpper().Trim();
            logger.Info("tradebox : isCloseTc = " + lCloseTC);
            if (String.Compare(lCloseTC, "TRUE") == 0)
            {
                isCloseTC = true;
            }
            else
            {
                isCloseTC = false;
            }
        }

        private void tradebox_Load(object sender, EventArgs e)
        {
            System.Timers.Timer myTimer = new System.Timers.Timer();
            myTimer.Elapsed += new ElapsedEventHandler( StopCheck );
            myTimer.Interval = 30*60000;
            myTimer.Start();            
        }

        private void StopCheck( object source, ElapsedEventArgs e )
        {
            int lhhmmss = DateTimeFunc.getHHMMSS();
            if (lhhmmss > 153000)
            {
                Application.Exit();
            }            
        }


        private void tradebox_FormClosed(object sender, FormClosedEventArgs e)
        {
            RedisAccountHandler.Instance.clear();
            if (!isCloseTC)
            {
                return;
            }
            else
            {
                closeTC();
            }            
        }

        private void closeTC()
        {
            Process[] processlist = Process.GetProcesses();
            foreach(Process theprocess in processlist){
                //logger.Info(theprocess.ProcessName + "..." + theprocess.MachineName);
                if(String.Compare(theprocess.ProcessName,"TC")==0){
                    logger.Info("kill TC process ... "+theprocess.ProcessName + "..." + theprocess.Id);
                    theprocess.Kill();
                }
                
            }
        }
    
    }
}
