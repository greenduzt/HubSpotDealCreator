using HubSpotDealCreator.DB;
using HubSpotDealCreator.Handlers;
using HubSpotDealCreator.Models;
using HubSpotDealCreator.Utilities;
using Microsoft.Extensions.Configuration;

public class Program
{
    public static async Task Main(string[] args)
    {       
        // Set up configuration
        IConfiguration config = Configure();

        // Initialize database configuration
        InitializeDatabase(config);

        // Load necessary data
        var (hubSpotProductList, systemParameters) = LoadData();

        // Prepare sample data
        Deal deal = PrepareDeal();

        // Create handlers for sequential checks
        var companyNameHandler = new CompanyNameSearchHandler();
        var domainHandler = new DomainSearchHandler();
        var abnHandler = new AbnSearchHandler();
        var companyCreationHandler = new CompanyCreationHandler();
        var purchasOrderUploadHandler = new PurchaseOrderUploadHandler(systemParameters);
        var createDeal = new DealCreationHandler();

        // Set up chain of responsibility
         companyNameHandler.SetNext(domainHandler)
            .SetNext(abnHandler)
            .SetNext(companyCreationHandler)
            .SetNext(purchasOrderUploadHandler)
            .SetNext(createDeal);
                                             

        // Initiate search process
        var (finalDeal, isFound) = await companyNameHandler.HandleAsync(deal, config);

        // Finally, create line items and deal if required
        if (isFound)
        {
            await CreateLineItemsAndDeal.CreateNewDeal(finalDeal, config);
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
            Company = new Company { ABN = "61166259025", Name = "gfhfh", Domain = "www.proone.com.au" },
            DeliveryAddress = new Address(),
            FileName = "Purchase_Order_No_42363.pdf",
            LineItems = new List<LineItems>() { new LineItems { SKU = "SY14G",Name = "Sand Yellow 1-4mm",Quantity = 80,UnitPrice = 2.05,NetPrice = 164 } }            
        };   

    

}

