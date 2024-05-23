using CoreLibrary.Models;
using HubSpotDealCreator;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

class Program
{
    public static async Task Main(string[] args)
    {

        var lineItem = new { ExpenseRaw = "Hello, World! This is a test.\nRemove special\n\rcharacters       & spaces!" };
        string pattern = @"[^a-zA-Z0-9]+";
        string cleanedString = Regex.Replace(lineItem.ExpenseRaw, pattern, " ").Trim();
       

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
           
            new LineItems 
            { 
                SKU = "Charge Line", Name = "$50 per Pallet. (20 rolls per\npallet)", Quantity = 180, UnitPrice = 50.00m, NetPrice = 50.00m,
                ExpenseRaw="9 24Kg Binder Procure 24Kg VIC Freight Launceston 24Kg Pail Procure Binder 10/03/2024 $187.20 $1,684.80"
            } ,
             new LineItems
            {
                SKU = "CF953-18",Name = "AM8505R 1.2X10MTRS = 12M2.\n20 ROLLS PER PALLET\nAM8505R",Quantity = 200.00m,UnitPrice = 105.00m,NetPrice = 105.00m,
                ExpenseRaw = "2g 20 bagged Bag Rubber CSBR EMERALD GREEN \nEmerald Green 23/03/2024 $41.60 $832.00"
            }
           // new LineItems { SKU = "CF953-18",Name = "AM8505R 1.2X10MTRS = 12M2. 20 ROLLS PER PALLET AM8505R",Quantity = 200.00m,UnitPrice = 105.00M,NetPrice = 105.00M },
           // new LineItems { SKU = "Charge Line", Name = "$50 per Pallet. (20 rolls per pallet)", Quantity = 180, UnitPrice = 50.00m, NetPrice = 50.00m }        
        }
    };



}

