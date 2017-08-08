using System.Collections.Generic;
using System.IO;
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

            categoryGroupDictionary = JsonConvert
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