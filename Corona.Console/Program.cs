using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Serialization;
using Corona.Models;
using Skype.Bot.Services;

namespace Corona.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string shutdownString = @"/c shutdown -s -t 00 -f -m \\10.19.10.134";
            try
            {
                ProcessStartInfo psiOpt = new ProcessStartInfo("cmd.exe", shutdownString);
                psiOpt.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System);
                psiOpt.WindowStyle = ProcessWindowStyle.Hidden;
                psiOpt.RedirectStandardOutput = true;
                psiOpt.UseShellExecute = false;
                psiOpt.CreateNoWindow = true;
                Process procCommand = Process.Start(psiOpt);
                procCommand.WaitForExit();                            
            }
            catch (Exception e)
            {
                string eError = e.Message;
                
            }
                      

            //var conversationEventService = new ConversationEventService();
            //var conversationService = new ConversationService();

            //var eventConversations = conversationEventService.ConversationEvents
            //  .Where(x => x.EventId == 1);

            //var conversations = conversationService.Conversations.Where(x => eventConversations.Any(a => a.ConversationId == x.Id));

            //foreach(var conver in conversations)

            //{
            //    System.Console.WriteLine(conver.ConversationId);
            //}

            //var messageModel = new MessageModel
            //{
            //    ConversationId = 1,
            //    Message = "testing"
            //};

            //RunAsync(messageModel).Wait();

            // var result = Ping("10.18.100.111");

            // System.Console.WriteLine(result);


            //var path = Environment.CurrentDirectory + "\\App_Data\\ProductionServer.xml";
            //var serverList = CoreService.GetXMLData(path);
            //var message = "";
            //foreach (var site in serverList.Site)
            //{
            //    message = site.Name + " is rebooting...";
            //    System.Console.WriteLine(message);

            //    foreach (var server in site.Servers)
            //    {
            //        if (server.Waiting != null)
            //        {                        
            //            var result = false;
            //            message = server.Ip + " is waiting...";
            //            System.Console.WriteLine(message);
            //            var waitingServerIp = site.Servers.First(x => x.Id == server.Waiting).Ip;
            //            while (!result)
            //            {
            //                result = CoreService.Ping(waitingServerIp);
            //            }
            //        }

            //        message = server.Ip + " is rebooting...";
            //        System.Console.WriteLine(message);

            //        CoreService.Reboot(server.Ip);
            //    }
            //}


            //foreach(var res in result)
            //{
            //    System.Console.WriteLine(res.Name);
            //}

            //System.Timers.Timer aTimer = new System.Timers.Timer();
            //aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            //aTimer.Interval = 5000;
            ////aTimer.Enabled = true;
            //aTimer.Start();

            System.Console.WriteLine("Press \'q\' to quit the sample.");
            while (System.Console.Read() != 'q') ;
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Thread.Sleep(9000);
            System.Console.WriteLine("Hello World! " + e.SignalTime);
            
        }

        static async Task RunAsync(MessageModel messageModel)
        {
            string apiBasePath = ConfigurationManager.AppSettings["API.BasePath"];

            System.Console.WriteLine("Calling the back-end API");

            CustomDelegatingHandler customDelegatingHandler = new CustomDelegatingHandler();

            HttpClient client = HttpClientFactory.Create(customDelegatingHandler);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.PostAsJsonAsync(apiBasePath + "api/SendMessage", messageModel);

            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                System.Console.WriteLine(responseString);
                System.Console.WriteLine("HTTP Status: {0}, Reason {1}. Press ENTER to exit", response.StatusCode, response.ReasonPhrase);
            }
            else
            {
                System.Console.WriteLine("Failed to call the API. HTTP Status: {0}, Reason {1}, Response header {2}", response.StatusCode, response.ReasonPhrase, response.Headers.ToString());

            }

            System.Console.ReadLine();
        }

        public static bool Ping(string ip)
        {
            var pingSender = new Ping();
            var pingOptions = new PingOptions();
            pingOptions.DontFragment = true;

            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 120;
            bool result = false;
            while (!result)
            {
                PingReply reply = pingSender.Send(ip, timeout, buffer, pingOptions);
                result = reply.Status == IPStatus.Success;                
                Thread.Sleep(1000);
            }

            return result;
        }

        public static Sites GetServerList(string xmlPath)
        {
            var sites = new Sites();

            try
            {
                if (File.Exists(xmlPath))
                {
                    var deserializer = new XmlSerializer(typeof(Sites), new XmlRootAttribute("sites"));

                    using (FileStream stream = File.OpenRead(xmlPath))
                    {
                        sites = deserializer.Deserialize(stream) as Sites;
                    }
                }
            }
            catch (Exception ex)
            {

                System.Console.WriteLine(ex.Message);
            }

            return sites;
        }
    }
}
