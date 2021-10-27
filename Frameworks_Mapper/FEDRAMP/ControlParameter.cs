using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frameworks_Mapper
{
    public class ControlParameter
    {
        private string parametertext = "";
        private string nistcontrolid = "";

        public string ParameterText { get { return this.parametertext; } set { this.parametertext = value; } }

        public string NISTControlID { get { return this.nistcontrolid; } set { this.nistcontrolid = value; } }
    }
}
