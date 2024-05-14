using HubSpotDealCreator.DB;
using HubSpotDealCreator.Handlers;
using HubSpotDealCreator.Models;
using HubSpotDealCreator.Utilities;
using Microsoft.Extensions.Configuration;
using Serilog.Events;
using Serilog;

public class Program
{
    public static async Task Main(string[] args)
    {       
        // Set up configuration
        IConfiguration config = Configure();

        Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug() 
                .WriteTo.File($"{config["Logging:Path"]} - {DateTime.Now.ToString("yyyyMMdd_HHmmss")}.txt", 
                    rollingInterval: RollingInterval.Day, 
                    restrictedToMinimumLevel: LogEventLevel.Debug,
                    shared:true) 
                .CreateLogger();

        Log.Information("---HubSpotDealCreator Started---");

        try
        {
            // Initializing database configuration
            InitializeDatabase(config);

            // Loading necessary data
            var (hubSpotProductList, systemParameters) = LoadData();

            if (hubSpotProductList.Count > 0 && systemParameters.Count > 0)
            {
                // Preparing sample data
                Deal deal = PrepareDeal();

                // Creating handlers for checking company name, domain, and ABN
                var companyNameHandler = new CompanyNameSearchHandler();
                var domainHandler = new DomainSearchHandler();
                var abnHandler = new AbnSearchHandler();

                // Setting the next handler in the chain
                companyNameHandler.SetNextHandler(domainHandler);
                domainHandler.SetNextHandler(abnHandler);

                // Starting the chain with the company name handler
                bool isCompanyFound = await companyNameHandler.Handle(deal, config);

                // If processing is not complete, proceeding with company creation, purchase order upload and deal creation
                if (!isCompanyFound)
                {
                    var companyCreationHandler = new CompanyCreationHandler();
                    var purchaseOrderUploadHandler = new PurchaseOrderUploadHandler(systemParameters);
                    var dealCreationHandler = new DealCreationHandler();

                    // Connecting the handlers sequentially
                    companyCreationHandler.SetNextHandler(purchaseOrderUploadHandler);
                    purchaseOrderUploadHandler.SetNextHandler(dealCreationHandler);

                    // Starting the processing chain with the first handler
                    await companyCreationHandler.Handle(deal, config);
                }
                else
                {
                    // If processing is complete, proceeding with purchase order upload and deal creation
                    var purchaseOrderUploadHandler = new PurchaseOrderUploadHandler(systemParameters);
                    var createDeal = new DealCreationHandler();

                    purchaseOrderUploadHandler.SetNextHandler(createDeal);

                    await purchaseOrderUploadHandler.Handle(deal, config);
                }
            }
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
            Log.CloseAndFlush();

            System.Environment.Exit(1);
        }
    }

    //Load the configuration file
    static IConfiguration Configure() =>
        new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddUserSecrets<Program>(true)
            .Build();

    static (List<HubSpotProduct>, List<SystemParameters>) LoadData()
    {
        // Load HubSpot products
        var hubSpotProductList = DBAccess.LoadHubSpotProducts();

        // Get system parameters
        var systemParameters = DBAccess.GetSystemParameters();

        return (hubSpotProductList, systemParameters);
    }

    static void InitializeDatabase(IConfiguration config)
    {
        DBConfiguration.Config = config;
        DBConfiguration.Initialize();    
    }

    // Prepare sample data
    static Deal PrepareDeal() => new Deal
        {
            Company = new Company { ABN = "61166259025", Name = "gfhfh", Domain = "www.sss.com" },
            DeliveryAddress = new Address() { StreetAddress = "123 Street",State = "QLD", PostCode="3111",Suburb ="Newland",Country="Australia"},
            DealName = "Test Deal",
            FileName = "Purchase_Order_No_42363.pdf",
            LineItems = new List<LineItems>() { new LineItems { SKU = "SY14G",Name = "Sand Yellow 1-4mm",Quantity = 80,UnitPrice = 2.05,NetPrice = 164 }, 
                                                new LineItems { SKU = "Prod3", Name = "prod 3 description", Quantity = 180, UnitPrice = 1.05, NetPrice = 1464 } }            
        };   

    

}

