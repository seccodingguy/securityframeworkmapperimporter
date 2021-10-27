using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frameworks_Mapper
{
    public class ControlRequirement
    {
        private string requirementtext = "";
        private string nistcontrolid = "";

        public string RequirementText { get { return this.requirementtext; } set { this.requirementtext = value; } }

        public string NISTControlID { get { return this.nistcontrolid; } set { this.nistcontrolid = value; } }
    }
}
