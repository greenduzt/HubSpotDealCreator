using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubSpotDealCreator.Message
{
    public class FileUploadMessage : Message
    {
        public string FilePath { get; }

        public FileUploadMessage(string filePath)
        {
            FilePath = filePath;
        }
    }
}
