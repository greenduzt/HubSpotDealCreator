using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubSpotDealCreator.Builders
{
    public interface IProgramBuilder
    {
        IProgramBuilder BuildHubSpotProducts();
        IProgramBuilder UploadFile(string filePath, string apiKey);
        IProgramBuilder GetPODetails();
        IProgramBuilder SearchCompanyInfo(string apiKey);
        IProgramBuilder CreateNewCompany(string apiKey);
        IProgramBuilder CreateNewDeal(string apiKey, string constructedFileName);
        IProgramBuilder SetConnectionString(string connectionString);
        Program Build();
    }
}
