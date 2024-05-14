using HubSpotDealCreator.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubSpotDealCreator.Handlers
{
    public interface IDealHandler
    {
        Task<bool> Handle(Deal deal, IConfiguration config);
        void SetNextHandler(IDealHandler nextHandler);
    }
}
