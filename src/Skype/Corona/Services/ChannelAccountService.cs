using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Corona.Models;

namespace Corona.Services
{
    public interface IChannelAccountService
    {
        void Insert(ChannelAccountModel model);
        void Update(string id, ChannelAccountModel model);

        void Delete(string id);

        bool Existing(string id);

        IList<ChannelAccountModel> ChannelAccounts { get; }

        ChannelAccountModel ChannelAccount(string id);

    }

    public class ChannelAccountService : BaseService, IChannelAccountService
    {       
        public IList<ChannelAccountModel> ChannelAccounts
        {
            get
            {
                return entities.ChannelAccounts.Select(x => new ChannelAccountModel { Id = x.Id, Name = x.Name }).ToList();
            }
        }

        public ChannelAccountModel ChannelAccount(string id)
        {
            return ChannelAccounts.First(x => x.Id == id);
        }

        public void Delete(string id)
        {
            var entity = entities.ChannelAccounts.Find(id);

            entities.ChannelAccounts.Remove(entity);
            entities.SaveChanges();
        }

        public bool Existing(string id)
        {
            return entities.ChannelAccounts.Any(x => x.Id == id);
        }

        public void Insert(ChannelAccountModel model)
        {
            entities.ChannelAccounts.Add(new EntityModel.ChannelAccount
            {
                Id = model.Id,
                Name = model.Name
            });

            entities.SaveChanges();
        }

        public void Update(string id, ChannelAccountModel model)
        {
            var entity = entities.ChannelAccounts.Find(id);

            entity.Name = model.Name;
            entities.SaveChanges();
        }
    }
}