﻿using CoreLibrary.Data;
using CoreLibrary.Models;
using HubSpotDealCreator.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

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
            var services = ConfigureServices(config);
            using (var serviceProvider = services.BuildServiceProvider())
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
                    var (dealUpdated, isCompanyFound) = await companyNameHandler.Handle(deal, config);

                    // If processing is not complete, proceeding with company creation, purchase order upload and deal creation
                    if (!isCompanyFound)
                    {
                        var salesRepHandler = new SalesRepHandler();
                        var companyCreationHandler = new CompanyCreationHandler();
                        var purchaseOrderUploadHandler = new PurchaseOrderUploadHandler(deal.FilePath);
                        var dealCreationHandler = new DealCreationHandler();

                        // Connecting the handlers sequentially
                        salesRepHandler.SetNextHandler(companyCreationHandler);
                        companyCreationHandler.SetNextHandler(purchaseOrderUploadHandler);
                        purchaseOrderUploadHandler.SetNextHandler(dealCreationHandler);

                        // Starting the processing chain with the first handler
                        await salesRepHandler.Handle(deal, config);
                    }
                    else
                    {
                        // If customer is found or created, proceeding with salesrep allocation, purchase order upload and deal creation
                        var salesRepHandler = new SalesRepHandler();
                        var purchaseOrderUploadHandler = new PurchaseOrderUploadHandler(deal.FilePath);
                        var createDeal = new DealCreationHandler();

                        salesRepHandler.SetNextHandler(purchaseOrderUploadHandler);
                        purchaseOrderUploadHandler.SetNextHandler(createDeal);

                        await salesRepHandler.Handle(deal, config);
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
       

    // Prepare sample data
    static Deal PrepareDeal() => new Deal
        {
            Company = new Company { ABN = "1111", Name = "chamara1", Domain = "" },
            DeliveryAddress = new Address() { StreetAddress = "123 Street",State = "QLD", PostCode="3111",Suburb ="Newland",Country="Australia"},
            DealName = "Test Deal",
            FileName = "Purchase_Order_No_42363.pdf",
            Emails = new List<string> { "chamara@a1rubber.com","leeanne@a1rubber.com","alex@yahoo.com","david@gmail.com" },
            LineItems = new List<LineItems>() { new LineItems { SKU = "SY14G",Name = "Sand Yellow 1-4mm",Quantity = 80,UnitPrice = 2.05M,NetPrice = 164 }, 
                                                new LineItems { SKU = "Prod3", Name = "prod 3 description", Quantity = 180, UnitPrice = 1.05M, NetPrice = 1464 } }            
        };

    private static IServiceCollection ConfigureServices(IConfiguration config)
    {
        var services = new ServiceCollection();
        services.AddDataAccessServices(config); // Pass IConfiguration
        return services;
    }

}

