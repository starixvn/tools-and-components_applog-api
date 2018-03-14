using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Corona.Models;

namespace Skype.Bot.Services
{
    public class CoreService
    {
        public static Sites GetXMLData(string xmlPath)
        {
            var sites = new Sites();

            if (File.Exists(xmlPath))
            {
                var deserializer = new XmlSerializer(typeof(Sites), new XmlRootAttribute("sites"));

                using (FileStream stream = File.OpenRead(xmlPath))
                {
                    sites = deserializer.Deserialize(stream) as Sites;
                }
            }

            return sites;
        }

        public static bool Ping(string ip)
        {
            var pingSender = new Ping();
            var pingOptions = new PingOptions();
            pingOptions.DontFragment = true;

            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 120;

            PingReply reply = pingSender.Send(ip, timeout, buffer, pingOptions);

            return reply.Status == IPStatus.Success;
        }

        public static bool Reboot(string ip)
        {
            try
            {
                RunPowerShellCommand("Reboot.ps1", "-ip " + ip);

                return true;
            }
            catch
            {
                return false;

            }
        }

        private static void RunPowerShellCommand(string scriptName, string param)
        {
            var exePath = Environment.SystemDirectory + @"\WindowsPowerShell\v1.0\powershell.exe";
            try
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Scripts", scriptName);

                //* Create your Process
                Process process = new Process();
                process.StartInfo.FileName = exePath;
                process.StartInfo.Arguments = "-executionpolicy unrestricted -command " + path + " " + param;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                //* Set ONLY ONE handler here.
                process.ErrorDataReceived += new DataReceivedEventHandler(OutputHandler);
                //* Start process
                process.Start();
                //* Read one element asynchronously
                process.BeginErrorReadLine();
                //* Read the other one synchronously
                string output = process.StandardOutput.ReadToEnd();
                //Console.WriteLine(output);
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            //* Do your stuff with the output (write to console/log/StringBuilder)
            if (!string.IsNullOrWhiteSpace(outLine.Data))
            {
                Fanex.Logging.LogService.Error(outLine.Data);
            }
        }
    }
}
