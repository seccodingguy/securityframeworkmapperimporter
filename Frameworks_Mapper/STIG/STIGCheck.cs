using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frameworks_Mapper
{
    public class STIGCheck
    {
        private string uiCheckText = "";
        private string cliCheckText = "";
        private string descr = "";
        private string checkSystemNo = "";

        public string UICheckSteps { get { return this.uiCheckText; } set { this.uiCheckText = value; } }

        public string CLICheckSteps { get { return this.cliCheckText; } set { this.cliCheckText = value; } }

        public string Description { get { return this.descr; } set { this.descr = value; } }

        public string CheckSystemNumber { get { return this.checkSystemNo; } set { this.checkSystemNo = value; } }
        public STIGCheck() { }
    }
}
