using HubSpotDealCreator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubSpotDealCreator.JSON
{
    public static class JSONPayLoad
    {
        public static List<string> ConstructCompanyJSON(Company company)
        {
            List<string> jsonLst = new List<string>();

            // Define the JSON payload for the search request by name
            jsonLst.Add(@"{
                    ""filterGroups"": [
                        {
                            ""filters"": [
                                {
                                    ""operator"": ""EQ"",
                                    ""propertyName"": ""name"",
                                    ""value"": """ + company.Name + @"""                                    
                                }
                            ]
                        }
                    ],
                    ""properties"": [""abn"",""name"",""domain"",""customer_type""]
                }");

            // Define the JSON payload for the search request by website
            jsonLst.Add(@"{
                    ""filterGroups"": [
                        {
                            ""filters"": [
                                {
                                    ""operator"": ""EQ"",
                                    ""propertyName"": ""domain"",
                                    ""value"": """ + company.Domain + @"""
                                }
                            ]
                        }
                    ],
                    ""properties"": [""abn"",""name"",""domain"",""customer_type""]
                }");

            // Define the JSON payload for the search request by ABN
            jsonLst.Add(@"{
                    ""filterGroups"": [
                        {
                            ""filters"": [
                                {
                                    ""operator"": ""EQ"",
                                    ""propertyName"": ""abn"",
                                    ""value"": """ + company.ABN + @"""
                                }
                            ]
                        }
                    ],
                    ""properties"": [""abn"",""name"",""domain"",""customer_type""]
                }");
            return jsonLst;
        }
    }
}
