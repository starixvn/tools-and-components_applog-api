using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Corona.EntityModel;
using Corona.Models;

namespace Corona.Services
{
    public interface IChannelAccountService
    {
        void Insert(ChannelAccountModel model);
        void Update(string id, ChannelAccountModel model);

        void Delete(string id);

        bool Existing(string id);

        IEnumerable<ChannelAccountModel> ChannelAccounts { get; }

        ChannelAccountModel ChannelAccount(string id);
    }

    public class ChannelAccountService : QueryService, IChannelAccountService
    {
        public IEnumerable<ChannelAccountModel> ChannelAccounts
        {
            get
            {
                return Entities.ChannelAccounts.Select(x => new ChannelAccountModel { Id = x.Id, Name = x.Name }).ToList();
            }
        }

        public ChannelAccountModel ChannelAccount(string id)
        {
            if (ChannelAccounts.Any() && !string.IsNullOrWhiteSpace(id))
            {
                return ChannelAccounts.FirstOrDefault(x => x.Id == id);
            }

            return new ChannelAccountModel();
        }

        public void Delete(string id)
        {
            var entity = Entities.ChannelAccounts.Find(id);

            if (entity != null)
            {
                Entities.Entry(entity).State = EntityState.Deleted;
                Entities.ChannelAccounts.Remove(entity);
                Entities.SaveChanges();
            }
        }

        public bool Existing(string id)
        {
            using (var context = new DBEntities())
            {
                return context.ChannelAccounts.Any(x => x.Id == id);
            }
        }

        public void Insert(ChannelAccountModel model)
        {
            var entity = new EntityModel.ChannelAccount
            {
                Id = model.Id,
                Name = model.Name
            };

            Entities.ChannelAccounts.Add(entity);

            Entities.SaveChanges();
        }

        public void Update(string id, ChannelAccountModel model)
        {
            var entity = Entities.ChannelAccounts.Find(id);

            if (entity != null)
            {
                Entities.Entry(entity).State = EntityState.Modified;
                entity.Name = model.Name;
                Entities.SaveChanges();
            }
        }
    }
}