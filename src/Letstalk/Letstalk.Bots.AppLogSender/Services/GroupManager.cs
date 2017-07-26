using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Letstalk.Bots.AppLogSender.Models;
using Newtonsoft.Json;

namespace Letstalk.Bots.AppLogSender.Services
{
    public interface GroupManager
    {
        string GetGroupByCategory(string category);
    }

    public class GroupManagerImpl : GroupManager
    {
        private readonly Dictionary<string, string> categoryGroupDictionary;        

        public GroupManagerImpl(string jsonContent = null)
        {
            var categoryContentJsonContent = string.IsNullOrWhiteSpace(jsonContent) 
                ? File.ReadAllText(AppSettings.CategoryGroupMapperFilePath) 
                : jsonContent;            

            this.categoryGroupDictionary = JsonConvert
                .DeserializeObject<Dictionary<string, string>>(categoryContentJsonContent);
        }

        public string GetGroupByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return string.Empty;
            }

            return categoryGroupDictionary.ContainsKey(category) 
                ? categoryGroupDictionary[category] 
                : string.Empty;
        }
    }
}