using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Corona.Models;
using Skype.Bot.DataAccess;
using Skype.Bot.Services;

namespace Skype.Bot.Services
{
    public interface IFunctionService
    {
        List<FunctionModel> Functions { get; }

        FunctionModel Function(int funtionId);
    }
    public class FunctionService : QueryService, IFunctionService
    {
        private FunctionModel Transform(Function model)
        {
            return new FunctionModel
            {
                Id = model.Id,
                Key = model.Key,
                Name = model.Name
            };
        }

        public List<FunctionModel> Functions
        {
            get
            {
                var functions = new List<FunctionModel>();

                Entities.Functions.ToList().ForEach(x => functions.Add(Transform(x)));

                return functions;
            }
        }

        public FunctionModel Function(int funtionId)
        {
            if (Functions.Any())
            {
                return Functions.First(x => x.Id == funtionId);
            }

            return new FunctionModel();
        }
    }
}