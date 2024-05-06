using Amazon.Textract.Model;
using HubSpotDealCreator.DB;
using HubSpotDealCreator.Models;
using HubSpotDealCreator.Services;
using Microsoft.Extensions.Configuration;
using System.Text;

public class Program
{
    public List<HubSpotProduct> hubSpotProductList;
    public List<ExpenseDocument> expenseDocumentsTemp;
    public string constructedFileName;
    public StringBuilder transStringBuilder;
    public bool companyFound;
    public bool isNewCompanyCreated;
    public object deal; // Define the type of deal object

    public static void Main(string[] args)
    {
        //Set up the config to load the user secrets
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddUserSecrets<Program>(true)
            .Build();

        // Set the configuration in DBConfiguration
        DBConfiguration.Config = config;

        // Initialize DBConfiguration
        DBConfiguration.Initialize();

        HubSpotProductService productService = new HubSpotProductService();
        List<HubSpotProduct> sp = productService.GetHubSpotProducts();
    }
}

