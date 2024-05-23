using CoreLibrary.Data;
using CoreLibrary.Models;
using HubSpotDealCreator.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace HubSpotDealCreator
{
    public class DealCreator
    {
        private readonly IConfiguration _config;
        public DealCreator(IConfiguration config)
        {
            _config = config;
        }

        public async Task CreateDealAsync(Deal deal)
        { 
            Log.Information("---HubSpotDealCreator Started---");

            try
            {
                if(deal == null)
                {
                    Log.Information("Deal object is empty");
                    return;
                }
                //var services = ConfigureServices(_config);
                //using (var serviceProvider = services.BuildServiceProvider())
                //{   

                    // Creating handlers for checking company name, domain, and ABN
                    var companyNameHandler = new CompanyNameSearchHandler();
                    var domainHandler = new DomainSearchHandler();
                    var abnHandler = new AbnSearchHandler();

                    // Setting the next handler in the chain
                    companyNameHandler.SetNextHandler(domainHandler);
                    domainHandler.SetNextHandler(abnHandler);

                    // Starting the chain with the company name handler
                    var (dealUpdated, isCompanyFound) = await companyNameHandler.Handle(deal, _config);

                    // If processing is not complete, proceeding with company creation, purchase order upload and deal creation
                    if (!isCompanyFound)
                    {
                        var salesRepHandler = new SalesRepHandler();
                        var companyCreationHandler = new CompanyCreationHandler();
                        var purchaseOrderUploadHandler = new PurchaseOrderUploadHandler();
                        var dealCreationHandler = new DealCreationHandler();

                        // Connecting the handlers sequentially
                        salesRepHandler.SetNextHandler(companyCreationHandler);
                        companyCreationHandler.SetNextHandler(purchaseOrderUploadHandler);
                        purchaseOrderUploadHandler.SetNextHandler(dealCreationHandler);

                        // Starting the processing chain with the first handler
                        await salesRepHandler.Handle(deal, _config);
                    }
                    else
                    {
                        // If customer is found or created, proceeding with salesrep allocation, purchase order upload and deal creation
                        var salesRepHandler = new SalesRepHandler();
                        var purchaseOrderUploadHandler = new PurchaseOrderUploadHandler();
                        var createDeal = new DealCreationHandler();

                        salesRepHandler.SetNextHandler(purchaseOrderUploadHandler);
                        purchaseOrderUploadHandler.SetNextHandler(createDeal);

                        await salesRepHandler.Handle(deal, _config);
                    }

                //}
            }
            catch (Exception ex)
            {
                // Logging any unhandled exceptions
                Log.Error(ex, "An unhandled exception occurred");
            }
            finally
            {
                Log.Information("---HubSpotDealCreator Ended---");

                // Close and flush the Serilog logger
                //Log.CloseAndFlush();                
            }
        }

        private static IServiceCollection ConfigureServices(IConfiguration config)
        {
            var services = new ServiceCollection();
            services.AddDataAccessServices(config); // Pass IConfiguration
            return services;
        }
    }
}
