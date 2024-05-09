using HubSpotDealCreator.DB;
using HubSpotDealCreator.Handlers;
using HubSpotDealCreator.Models;
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

        // Set up chain of responsibility
         companyNameHandler.SetNext(domainHandler).SetNext(abnHandler).SetNext(companyCreationHandler);
                                             

        // Initiate search process
        var (finalDeal, isFound) = await companyNameHandler.SearchAsync(deal, config);

        // Finally, create line items and deal if required
        if (finalDeal.CompanyFound || finalDeal.DomainFound || finalDeal.AbnFound || finalDeal.newCompanyCreated)
        {
        //    await CreateLineItemsAndDeal(apiKey, finalDeal);
        }
    }

    static IConfiguration Configure()
    {
        // Set up the config to load the user secrets
        return new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddUserSecrets<Program>(true)
            .Build();
    }

    static void InitializeDatabase(IConfiguration config)
    {
        // Set the configuration for DBConfiguration
        DBConfiguration.Config = config;

        // Set the connection string
        DBConfiguration.Initialize();
    }

    static (List<HubSpotProduct>, List<SystemParameters>) LoadData()
    {
        // Load HubSpot products
        var hubSpotProductList = DBAccess.LoadHubSpotProducts();

        // Get system parameters
        var systemParameters = DBAccess.GetSystemParameters();

        return (hubSpotProductList, systemParameters);
    }

    static Deal PrepareDeal()
    {
        // Prepare sample data
        return new Deal
        {
            Company = new Company { ABN = "61166259025", Name = "proone" },
            FileName = "Purchase_Order_No_42363.pdf"
        };
    }

    

}

