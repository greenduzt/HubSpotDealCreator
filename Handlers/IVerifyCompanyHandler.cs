using HubSpotDealCreator.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubSpotDealCreator.Handlers
{
    public interface IVerifyCompanyHandler
    {
        Task<(Deal, bool)> HandleAsync(Deal deal, IConfiguration config);
        IVerifyCompanyHandler SetNext(IVerifyCompanyHandler handler);
    }
}
