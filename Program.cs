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
    private static Deal deal; 

    public static async Task Main(string[] args)
    {
       
        //Set up the config to load the user secrets
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddUserSecrets<Program>(true)
            .Build();

        // Set the configuration for DBConfiguration
        DBConfiguration.Config = config;

        ProgramBuilder programBuilder = new ProgramBuilder();

        Program program = programBuilder
        .SetConnectionString(DBConfiguration.GetConnectionString())
        .GetSystemParameters()
        .GetHubSpotProducts()
        .Build(); // Build the program instance here
                
        program = programBuilder
            .UploadFile(deal.FileName, config, program.systemParameters) // Use the program instance here
            .Build();

        await program.AddToHubSpot();
    }

    public async Task AddToHubSpot()
    {
        // Adding to HubSpot
        await Task.Delay(0);
    }
}

