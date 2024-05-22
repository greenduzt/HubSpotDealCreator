using CoreLibrary.Models;
using HubSpotDealCreator;
using Microsoft.Extensions.Configuration;

class Program
{
    public static async Task Main(string[] args)
    {
        // Set up configuration
        IConfiguration config = Configure();

        // Preparing sample data
        Deal deal = PrepareDeal();
        DealCreator dealCreator = new DealCreator(config);
        await dealCreator.CreateDealAsync(deal);
        
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
        DeliveryAddress = new Address() { StreetAddress = "123 Street", State = "QLD", PostCode = "3111", Suburb = "Newland", Country = "Australia" },
        DealName = "Test Deal",
        FileName = "Purchase_Order_No_42363.pdf",
        Emails = new List<string> { "chamara@a1rubber.com", "leeanne@a1rubber.com", "alex@yahoo.com", "david@gmail.com" },
        LineItems = new List<LineItems>() 
        {
            new LineItems { SKU = "CF953-18",Name = "AM8505R 1.2X10MTRS = 12M2.\n20 ROLLS PER PALLET\nAM8505R",Quantity = 200.00m,UnitPrice = 105.00m,NetPrice = 105.00m },
            new LineItems { SKU = "Charge Line", Name = "$50 per Pallet. (20 rolls per\npallet)", Quantity = 180, UnitPrice = 50.00m, NetPrice = 50.00m } 
           // new LineItems { SKU = "CF953-18",Name = "AM8505R 1.2X10MTRS = 12M2. 20 ROLLS PER PALLET AM8505R",Quantity = 200.00m,UnitPrice = 105.00M,NetPrice = 105.00M },
           // new LineItems { SKU = "Charge Line", Name = "$50 per Pallet. (20 rolls per pallet)", Quantity = 180, UnitPrice = 50.00m, NetPrice = 50.00m }        
        }
    };



}

