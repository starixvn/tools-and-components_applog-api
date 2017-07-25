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
        private readonly Dictionary<string, string> groupCategoryMapper;

        public GroupManagerImpl(Dictionary<string, string> groupCategoryMapper)
        {
            this.groupCategoryMapper = groupCategoryMapper ?? GetCategoryGroupList();
        }

        public string GetGroupByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return string.Empty;
            }

            return groupCategoryMapper.Keys.Contains(category) ? groupCategoryMapper[category] : string.Empty;
        }

        private static Dictionary<string, string> GetCategoryGroupList()
            => JsonConvert
            .DeserializeObject<Dictionary<string, string>>(File.ReadAllText(AppSettings.CategoryGroupMapperFilePath));

       
    }
}