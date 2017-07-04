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
    }
}
