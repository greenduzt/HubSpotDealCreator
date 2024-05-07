using Amazon.Textract.Model;
using HubSpotDealCreator.Builders;
using HubSpotDealCreator.DB;
using HubSpotDealCreator.Models;
using HubSpotDealCreator.Services;
using Microsoft.Extensions.Configuration;
using System.Text;

public class Program
{
    public List<SystemParameters> systemParameters;
    public List<HubSpotProduct> hubSpotProductList;
    public List<ExpenseDocument> expenseDocumentsTemp;
    public string constructedFileName;
    public StringBuilder transStringBuilder;
    public bool companyFound;
    public bool isNewCompanyCreated;
    public object deal; // Define the type of deal object

    public static async Task Main(string[] args)
    {
        //Set up the config to load the user secrets
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddUserSecrets<Program>(true)
            .Build();

        Program program = new ProgramBuilder()
            .SetConnectionString(DBConfiguration.GetConnectionString())
            .GetSystemParameters()
            .BuildHubSpotProducts()                                             
            .Build(); 
      
        await program.AddToHubSpot();
    }

    public async Task AddToHubSpot()
    {
        // Adding to HubSpot
        await Task.Delay(0);
    }
}

