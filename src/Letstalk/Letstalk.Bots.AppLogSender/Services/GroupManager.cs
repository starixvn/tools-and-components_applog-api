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
        private readonly StringDictionary groupCategoryMapper;

        public GroupManagerImpl(StringDictionary groupCategoryMapper)
        {
            this.groupCategoryMapper = groupCategoryMapper ?? GetCategoryGroupList();
        }

        public string GetGroupByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return string.Empty;
            }

            return groupCategoryMapper[category];
        }

        private static StringDictionary GetCategoryGroupList()
            => JsonConvert
            .DeserializeObject<StringDictionary>(File.ReadAllText(AppSettings.CategoryGroupMapperFilePath));
    }
}