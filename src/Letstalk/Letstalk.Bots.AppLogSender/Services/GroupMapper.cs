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
    public interface GroupMapper
    {
        string GetGroupByCategory(string category);        
    }

    public class GroupMapperImpl : GroupMapper
    {
        private readonly StringDictionary dataMapper;

        public GroupMapperImpl()
        {
            dataMapper = dataMapper ?? GetCategoryGroupList();
        }

        public string GetGroupByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return string.Empty;
            }

            return dataMapper[category];
        }      

        private static StringDictionary GetCategoryGroupList()
        {
            var configPath = AppDomain.CurrentDomain.BaseDirectory + "/Configurations/CategoryGroupMapper.json";
            if (File.Exists(configPath))
            {
                var allText = File.ReadAllText(configPath);

                return JsonConvert.DeserializeObject<StringDictionary>(allText);
            }

            return null;
        }
    }
}