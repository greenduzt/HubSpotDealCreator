using HubSpotDealCreator.Models;
using Microsoft.Extensions.Configuration;

namespace HubSpotDealCreator.Handlers
{
    public abstract class DealHandlerBase : IDealHandler
    {
        private IDealHandler _nextHandler;

        public abstract Task<(Deal,bool)> Handle(Deal deal, IConfiguration config);

        public void SetNextHandler(IDealHandler nextHandler)
        {
            _nextHandler = nextHandler;
        }

        protected async Task<(Deal,bool)> PassToNextHandler(Deal deal, IConfiguration config)
        {
            if (_nextHandler != null)
            {
                return await _nextHandler.Handle(deal, config);
            }
            return (null,false);
        }
    }
}
