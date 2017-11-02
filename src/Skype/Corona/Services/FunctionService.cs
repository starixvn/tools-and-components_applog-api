using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Corona.Models;

namespace Corona.Services
{
    public interface IFunctionService
    {
        IList<FunctionModel> Functions { get; }

        FunctionModel Function(int funtionId);
    }
    public class FunctionService : QueryService, IFunctionService
    {       
        public IList<FunctionModel> Functions
        {
            get
            {
                return Entities.Functions.Select(x => new FunctionModel {
                    Id = x.Id,
                    Name = x.Name,
                    Key = x.Key
                }).ToList();
            }
        }

        public FunctionModel Function(int funtionId)
        {
            if(Functions.Any())
            {
                return Functions.First(x => x.Id == funtionId);
            }

            return new FunctionModel();
        }
    }
}