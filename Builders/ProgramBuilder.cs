using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubSpotDealCreator.Builders
{
    public class ProgramBuilder
    {
        private Program program;
        private string connectionString;

        public ProgramBuilder()
        {
            program = new Program();
        }
    }
}
