using CoreLibrary;
using CoreLibrary.Models;
using Microsoft.Extensions.Configuration;

namespace HubSpotDealCreator.Handlers
{
    public interface IDealHandler
    {
        Task<(Deal, bool)> Handle(Deal deal, IConfiguration config);
        void SetNextHandler(IDealHandler nextHandler);
    }
}
