using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skype.Bot.DataAccess;

namespace Skype.Bot.Services
{
    public class QueryService : IDisposable
    {
        private readonly BotEntities _entities;

        public QueryService() : this(null) { }

        public QueryService(BotEntities entities)
        {
            _entities = entities ?? new BotEntities();
        }

        protected BotEntities Entities => _entities;
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
