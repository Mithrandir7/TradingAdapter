using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Diagnostics;
using System.Windows.Forms;

namespace TCMonitor
{
    public class TouchanceMonitor
    {

        public static TouchanceMonitor Instance = new TouchanceMonitor();

        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
   
        private TouchanceMonitor()
        {

        }

        public void init()
        {
            
        }

        ~TouchanceMonitor()
        {
            
            if (TouchanceXmlReader.Instance.closedTCOnExit)
            {
                closeTouchance();
            }
        }

        public void ifTouchanceNotExistCloseApplication()
        {
            if (!isTouchanceExist())
            {
                if (TouchanceXmlReader.Instance.checkTCOnStartup)
                {
                    MessageBox.Show("Please run touchance before running this program." + TouchanceXmlReader.Instance.checkTCOnStartup.ToString());
                    Environment.Exit(1);
                }
            }
        }

        public bool isTouchanceExist()
        {
            Process[] processlist = Process.GetProcesses();
            foreach (Process theprocess in processlist)
            {
                //logger.Info(theprocess.ProcessName + "..." + theprocess.MachineName);
                if (String.Compare(theprocess.ProcessName, "TC") == 0)
                {
                    return true;
                }

            }
            return false;
        }


        public void closeTouchance()
        {
            Process[] processlist = Process.GetProcesses();
            foreach (Process theprocess in processlist)
            {
                //logger.Info(theprocess.ProcessName + "..." + theprocess.MachineName);
                if (String.Compare(theprocess.ProcessName, "TC") == 0)
                {
                    logger.Info("kill TC process ... " + theprocess.ProcessName + "..." + theprocess.Id);
                    theprocess.Kill();
                }

            }
        }



    }
}
