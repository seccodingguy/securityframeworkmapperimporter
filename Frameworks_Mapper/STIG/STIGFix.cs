using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frameworks_Mapper
{
    public class STIGFix
    {
        private string uiFixText = "";
        private string cliFixText = "";
        private string descr = "";
        private string fixIdNo = "";
        
        
        public string UIFixSteps { get { return this.uiFixText; } set { this.uiFixText = value; } }

        public string CLIFixSteps { get { return this.cliFixText; } set { this.cliFixText = value; } }

        public string Description { get { return this.descr; } set { this.descr = value; } }

        public string FixIDNumber { get { return this.fixIdNo; } set { this.fixIdNo = value; } }
        public STIGFix() { }
    }
}
