using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
            System.Console.Write("Input your message: ");
            var message = System.Console.ReadLine();

            var messageModel = new MessageModel
            {
                ConversationId = 1,
                Message = message
            };

            RunAsync(messageModel).Wait();
        }

        static async Task RunAsync(MessageModel messageModel)
        {
            string apiBasePath = ConfigurationManager.AppSettings["API.BasePath"];

            System.Console.WriteLine("Calling the back-end API");                        

            CustomDelegatingHandler customDelegatingHandler = new CustomDelegatingHandler();

            HttpClient client = HttpClientFactory.Create(customDelegatingHandler);           

            HttpResponseMessage response = await client.PostAsJsonAsync(apiBasePath + "api/SendMessage", messageModel);

            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                System.Console.WriteLine(responseString);
                System.Console.WriteLine("HTTP Status: {0}, Reason {1}. Press ENTER to exit", response.StatusCode, response.ReasonPhrase);
            }
            else
            {
                System.Console.WriteLine("Failed to call the API. HTTP Status: {0}, Reason {1}", response.StatusCode, response.ReasonPhrase);
            }

            System.Console.ReadLine();
        }       
    }
}
