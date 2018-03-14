using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Skype.Bot.Services
{
    public interface IUserMessageService
    {
        Dictionary<string, List<string>> GetMessage();
    }
    public class UserMessageService : IUserMessageService
    {
        public Dictionary<string, List<string>> GetMessage()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\UserMessages.json");
            var content = File.ReadAllText(filePath);

            return JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(content);
        }
    }
}
