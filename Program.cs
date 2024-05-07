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
    public Task<Dictionary<Deal, bool>> companyResult;
    private static Deal deal;

    public static async Task Main(string[] args)
    {

        deal = new Deal();
        deal.Company = new Company() { ABN = "61166259025",Name = "proone" };
        deal.FileName = "Purchase_Order_No_42363.pdf";

        // Set up the config to load the user secrets
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddUserSecrets<Program>(true)
            .Build();

        // Set the configuration for DBConfiguration
        DBConfiguration.Config = config;

        ProgramBuilder programBuilder = new ProgramBuilder();

        // Build the program instance here
        IProgramBuilder programBuilderInstance = await programBuilder
            .SetConnectionString(DBConfiguration.GetConnectionString())
            .GetSystemParameters()
            .GetHubSpotProducts()
            .CheckCompanyExists(deal, config); // Correct placement of CheckCompanyExists

        Program program = await programBuilderInstance.BuildAsync(); // Use BuildAsync

        
        // Upload file and check company existence
        program = await programBuilderInstance
            .UploadFile(deal.FileName, config, program.systemParameters)
            .BuildAsync(); // Use BuildAsync directly

        await program.AddToHubSpot();
    }

    public async Task AddToHubSpot()
    {
        // Adding to HubSpot
        await Task.Delay(0);
    }
}

