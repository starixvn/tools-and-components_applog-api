using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Letstalk.Bots.AppLogSender.Services
{
    public interface GroupService
    {
        string GetGroupByCategory(string category);
    }

    public class GroupServiceImpl : GroupService
    {
        private StringDictionary dataMapper;

        public string GetGroupByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return string.Empty;
            }

            return dataMapper[category];
        }
    }
}