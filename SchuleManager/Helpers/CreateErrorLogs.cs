using System;
using System.Globalization;
using System.IO;

namespace SchuleManager.Helpers
{
    public class CreateErrorLogs
    {
        public string LogFormat; //used to create log files data format..dd/mm/yy hh:mm:ss AM/PM ==> Log Message
        public string ErrorFileName; //used to create log file name

        #region "Default Constructors"
        public CreateErrorLogs(string username) //when the class is initialized a log data format is established based on date and time, and the log filename is also creaed
        {
            LogFormat = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + "==>";

            var sYear = DateTime.Now.Year.ToString(CultureInfo.InvariantCulture);
            var sMonth = DateTime.Now.Month.ToString(CultureInfo.InvariantCulture);
            var sDay = DateTime.Now.Day.ToString(CultureInfo.InvariantCulture);

            ErrorFileName = "Error_Log_" + username + sYear + sMonth + sDay + ".txt";
        }

        public CreateErrorLogs() //when the class is initialized a log data format is established based on date and time, and the log filename is also creaed
        {
            LogFormat = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + "==>";

            var sYear = DateTime.Now.Year.ToString(CultureInfo.InvariantCulture);
            var sMonth = DateTime.Now.Month.ToString(CultureInfo.InvariantCulture);
            var sDay = DateTime.Now.Day.ToString(CultureInfo.InvariantCulture);

            ErrorFileName = "Error_Log_" + sYear + sMonth + sDay + ".txt";
        }
        #endregion

        #region "Create Log Files Helper Methods"
        public void CreateErrorLog(string sPathName, string sErrMsg)
        {
            //a .txt file is created at a specified path and error message is written into it
            var sw = new StreamWriter(sPathName + ErrorFileName, true);
            sw.WriteLine(LogFormat + sErrMsg);
            sw.Flush();
            sw.Close();
        }
        #endregion
    }
}
