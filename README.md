# HubSpot Deal Creator

This is a C# application for creating deals in HubSpot CRM. It allows you to automate the process of creating deals by searching for companies based on their name, domain, or ABN (Australian Business Number), uploading purchase orders, and creating line items and deals.

## Features

- **Search by ABN**: Search for companies in HubSpot based on their ABN.
- **Search by Domain**: Search for companies in HubSpot based on their domain.
- **Search by Company Name**: Search for companies in HubSpot based on their name.
- **Upload Purchase Order**: Upload purchase orders to HubSpot.
- **Create Line Items and Deals**: Create line items and deals in HubSpot CRM.

## Prerequisites

Before running the application, make sure you have the following installed:

- .NET Core SDK
- HubSpot API Key
- Configuration file (`appsettings.json`) containing necessary settings
 
## Usage

1. Clone the repository.
2. Configure the application by providing the necessary settings in the `appsettings.json` file.
3. Build and run the application.

## Configuration

You need to provide the following configuration settings:

- **HubSpot API Key**: Obtain your HubSpot API key from your HubSpot account.
- **Logging Path**: Specify the path where log files will be saved.
- **HubSpot-API:File-Upload-Location**: Specify the location where uploaded files will be stored in HubSpot.
- **Other HubSpot API endpoints**: Update the API endpoints in the utility classes (`CheckHBABN`, `CheckHBCompanyDomain`, `CheckHBCompanyName`, `CreateLineItemsAndDeal`, `CreateNewHBCompany`, `UploadPurchaseOrder`) if necessary.

## Dependencies

- `Serilog`: For logging.
- `Newtonsoft.Json`: For JSON serialization and deserialization.
- `RestSharp`: For making HTTP requests.

## Contributing

Contributions are welcome! Feel free to fork the repository, make changes, and submit pull requests.

## License

This project is licensed under the [MIT License](LICENSE).

Chamara Walaliyadde
