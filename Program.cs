using HubSpotDealCreator.DB;
using HubSpotDealCreator.Message;
using HubSpotDealCreator.Models;
using HubSpotDealCreator.Services;
using Microsoft.Extensions.Configuration;

public class Program
{
    public static async Task Main(string[] args)
    {
        //Sample data
        Deal deal = new Deal();
        deal.Company = new Company() { ABN = "61166259025", Name = "proone" };
        deal.FileName = "Purchase_Order_No_42363.pdf";

        // Set up the config to load the user secrets
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddUserSecrets<Program>(true)
            .Build();

        // Set the configuration for DBConfiguration
        DBConfiguration.Config = config;
        // Set the connection string
        DBConfiguration.Initialize();

        // Load HubSpot products
        List<HubSpotProduct> hubSpotProductList = DBAccess.LoadHubSpotProducts();

        // Get system parameters
        List<SystemParameters> systemParameters = DBAccess.GetSystemParameters();

        var attchLoc = systemParameters.FirstOrDefault(x=>x.Type.Equals("po_location"));

        // Initialize message queue
        var messageQueue = new Queue<Message>();

        // Enqueue messages representing different actions
        messageQueue.Enqueue(new FileUploadMessage(attchLoc.AttchmentLocation + @"\" +deal.FileName ));
        messageQueue.Enqueue(new CheckCompanyNameMessage());

        // Process messages asynchronously
        while (messageQueue.Count > 0)
        {
            var message = messageQueue.Dequeue();

            try
            {
                await ProcessMessageAsync(message, config);
            }
            catch (Exception ex)
            {
                // Handle exceptions gracefully and log errors
                Console.WriteLine($"Error processing message: {ex.Message}");
            }
        }
    }

    static async Task ProcessMessageAsync(Message message, IConfiguration config)
    {
        switch (message)
        {
            case FileUploadMessage fileUploadMessage:
                await HandleFileUploadAsync(fileUploadMessage, config);
                break;

            case CheckCompanyNameMessage checkCompanyNameMessage:
                await HandleGetDataAsync(checkCompanyNameMessage, config);
                break;

                
        }
    }

    static async Task HandleGetDataAsync(CheckCompanyNameMessage message, IConfiguration config)
    {
        // Retrieve data asynchronously
        //var expenseDocumentsTemp = Data.GetData();

        // Enqueue next message or perform further processing
        // messageQueue.Enqueue(new NextMessage());
    }

    static async Task HandleFileUploadAsync(FileUploadMessage message, IConfiguration config)
    {
        // Perform file upload asynchronously
        string constructedFileName = POUpload.UploadFile(message.FilePath, config);

        // Enqueue next message or perform further processing
        // messageQueue.Enqueue(new NextMessage());
    }

}

