using HubSpotDealCreator.Models;
using Microsoft.Extensions.Configuration;
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
        IProgramBuilder GetHubSpotProducts();
        IProgramBuilder UploadFile(string fileName, IConfiguration config, List<SystemParameters> sp);                        
        IProgramBuilder SetConnectionString(string connectionString);
        Program Build();
    }
}
