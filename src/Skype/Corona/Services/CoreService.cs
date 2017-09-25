using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Corona.Services
{
    public static class CoreService
    {
        public static void WriteLog(string message)
        {
            var mess = new StringBuilder();
            mess.Append(Environment.NewLine);
            mess.Append(message);
            File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\log.txt", mess.ToString());
        }

        public static string FormatMessage(string message) => message.Replace("@Corona", string.Empty).Trim();        
    }
}