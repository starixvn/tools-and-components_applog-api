using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Corona.EntityModel;

namespace Corona.Services
{
    public class QueryService : IDisposable
    {
        private readonly DBEntities _entities;

        public QueryService() : this(null) { }

        public QueryService(DBEntities entities)
        {
            _entities = entities ?? new DBEntities();
        }

        protected DBEntities Entities => _entities;
        ~QueryService()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Entities.Dispose();
            }
        }
    }
}