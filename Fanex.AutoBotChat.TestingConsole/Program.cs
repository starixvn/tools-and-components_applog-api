using ONEbook.NotificationService;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Fanex.AutoBotChat.TestingConsole
{
    internal class Program
    {
        private static string _environmentServers = string.Empty;
        private static string _environmentAuthKey = string.Empty;

        private static void Main(string[] args)
        {
            _environmentServers = ConfigurationManager.AppSettings["Environtment-Servers"];
            _environmentAuthKey = ConfigurationManager.AppSettings["Environtment-AuthKey"];
            var targetName = string.Empty;
            var message = string.Empty;
            Console.WriteLine("Target Name:");
            targetName = Console.ReadLine();
            Console.WriteLine("Message :");
            message = Console.ReadLine();
            var vars = new Dictionary<string, string>()
                            {
                               { "Title", "Testing Title" },
                                { "ServerName","Testing Server Name" },
                                { "LogDate", DateTime.Now.ToString() },
                                { "Message", message }
                            };
            try
            {
                var result = SendMessage(targetName, vars);

                if (result)
                {
                    Console.WriteLine("Message is sent");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadKey();
        }

        public static bool SendMessage(string targetName, Dictionary<string, string> vars)
        {
            var serverUrls = _environmentServers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            Notification.Initialize(serverUrls, _environmentAuthKey);

            return Notification.Send(targetName, vars, null, null);
        }
    }
}