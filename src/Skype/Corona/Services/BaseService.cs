using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Corona.EntityModel;

namespace Corona.Services
{
    public class BaseService
    {
        public static DBEntities entities => new DBEntities();

        protected BaseService()
        {

        }       
    }
}