using HubSpotDealCreator.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubSpotDealCreator.Handlers
{
    public interface ICompanySearchHandler
    {
        Task<(Deal, bool)> SearchAsync(Deal deal, IConfiguration config);
        ICompanySearchHandler SetNext(ICompanySearchHandler handler);
    }
}
