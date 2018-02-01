using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Corona.Models;

namespace Corona.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var messageModel = new MessageModel
            {
                ConversationId = Convert.ToInt32(args[0]),
                Message = args[1]
            };
            
            RunAsync(messageModel).Wait();
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
    }
}
