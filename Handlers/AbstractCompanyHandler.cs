using HubSpotDealCreator.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace HubSpotDealCreator.Handlers
{
    public abstract class AbstractCompanyHandler : IVerifyCompanyHandler
    {
        protected IVerifyCompanyHandler _nextHandler;

        public IVerifyCompanyHandler SetNext(IVerifyCompanyHandler handler)
        {
            _nextHandler = handler;
            return handler;
        }

        public virtual async Task<(Deal, bool)> HandleAsync(Deal deal,IConfiguration config)
        {
            if (_nextHandler != null)
            {
                return await _nextHandler.HandleAsync(deal, config);
            }
            else
            {
                return (null,false);
            }
        }
    }
}
