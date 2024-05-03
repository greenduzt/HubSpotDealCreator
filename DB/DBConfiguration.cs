using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubSpotDealCreator.DB
{
    public class DBConfiguration
    {
        private static string dbConnectionString;
        public static IConfiguration Config { get; set; }

        public static void Initialize()
        {
            dbConnectionString = $"Server={Config["DBConnection:Server"]};Database={Config["DBConnection:Database"]};Uid={Config["DBConnection:UserId"]};Pwd={Config["DBConnection:Password"]}";
        }

        public static string GetConnectionString()
        {
            return dbConnectionString;
        }
    }
}
