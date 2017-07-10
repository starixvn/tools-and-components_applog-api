using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace AppLogApi.Models.AppLog
{
    public class AppLog
    {
        public string Message { get; set; }

        [XmlAttribute]
        public string Categories { get; set; }

        public string SiteAddress { get; set; }

        public bool IsIgnoreCategory(string category)
        {
            if (!string.IsNullOrWhiteSpace(Categories) && !string.IsNullOrWhiteSpace(category))
            {
                var listOfCategory = Categories.Split(new[] { ';', ',' }).Select(p=>p.Trim()).ToList();
                if (listOfCategory.Count > 0)
                {
                    return listOfCategory.Contains(category);
                }

                return false;
            }

            return true;
        }

        public bool IsIgnoreMessage(string message)
        {
            if (message.Trim().IndexOf(Message.Trim(), StringComparison.OrdinalIgnoreCase) != -1)
            {
                return true;
            }

            return false;
        }

        public bool IsIgnoreDomain(string siteAddress)
        {
            if (!string.IsNullOrWhiteSpace(SiteAddress) && siteAddress != "N/A")
            {
                if (SiteAddress.IndexOf(siteAddress, StringComparison.OrdinalIgnoreCase) != -1)
                {
                    return true;
                }

                return false;
            }

            return true;
        }
    }
}
