using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubSpotDealCreator.Builders
{
    public interface IProgramBuilder
    {
        IProgramBuilder GetSystemParameters();
        IProgramBuilder BuildHubSpotProducts();
        IProgramBuilder UploadFile(string filePath, string apiKey);                        
        IProgramBuilder SetConnectionString(string connectionString);
        Program Build();
    }
}
