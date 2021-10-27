using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frameworks_Mapper
{
    public class FEDRAMPControl
    {
        private string sortid = "";
        private string nistdescr = "";
        private string nistcontrolid = "";
        private string organizationdescr = "";
        private string controlname = "";
        private string supplementguidance = "";
        private string level = "";
        private int fedrampid = 0;
        private int nistid = 0;
        private int nistrevno = 0;
        private List<ControlParameter> controlparams = null;
        private List<ControlRequirement> controlrequirements = null;

        public string FEDRAMPDescription { get { return this.organizationdescr; } set { this.organizationdescr = value; } }

        public string FEDRAMPSupplementalGuidance { get { return this.supplementguidance; } set { this.supplementguidance = value; } }

        public string FEDRAMPControlName { get { return this.controlname; } set { this.controlname = value; } }

        public string FEDRAMPSortID { get { return this.sortid; } set { this.sortid = value; } }

        public string NISTDescription { get { return this.nistdescr; } set { this.nistdescr = value; } }


        public string NISTControlID { get { return this.nistcontrolid; } set { this.nistcontrolid = value; } }
        public List<ControlRequirement> FEDRAMPRequirements { get { return this.controlrequirements; } set { this.controlrequirements = value; } }

        public List<ControlParameter> FEDRAMPSelectionParams { get { return this.controlparams; } set { this.controlparams = value; } }

        public string FEDRAMPLevel { get { return this.level; } set { this.level = value; } }

        public int FEDRAMPID { get { return this.fedrampid; } set { this.fedrampid = value; } }

        public int NISTID { get { return this.nistid; } set { this.nistid = value; } }

        public int NISTRevisionNumber { get { return this.nistrevno; } set { this.nistrevno = value; } }
        public void AddControlRequirement(ControlRequirement item)
        {
            if(this.controlrequirements == null)
            {
                this.controlrequirements = new List<ControlRequirement>();
            }

            this.controlrequirements.Add(item);
        }

        public void AddControlRequirements(List<ControlRequirement> controlrequirements)
        {
            this.controlrequirements = controlrequirements;
        }

        public void AddControlParameters(List<ControlParameter> controlparameters)
        {
            this.controlparams = controlparameters;
        }

        public void AddControlParameter(ControlParameter item)
        {
            if(this.controlparams == null)
            {
                this.controlparams = new List<ControlParameter>();
            }

            this.controlparams.Add(item);
        }
    }
}
