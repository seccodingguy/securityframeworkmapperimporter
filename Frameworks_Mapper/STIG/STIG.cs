using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frameworks_Mapper
{
    public class STIG
    {
        private List<string> cciReferences = null;
        private string groupIdNo = "";
        private string descr = "";
        private string title = "";
        private string target = "";
        private List<STIGRule> rules = null;

        public string GroupIDNumber
        {
            get { return this.groupIdNo; }
            set { this.groupIdNo = value; }
        }

        public string Description
        {
            get { return this.descr; }
            set { this.descr = value; }
        }

        public string TargetSystem { get { return this.target; } set { this.target = value; } }

        public string Title { get { return this.title; } set { this.title = value; } }

        public IEnumerable<string> CCIReferenceList { get { return this.cciReferences; } }

        public List<STIGRule> GetSTIGRules { get { return this.rules; } }

        public void AddCCIReference(string item)
        {
            if(this.cciReferences == null)
            {
                this.cciReferences = new List<string>();
            }

            this.cciReferences.Add(item);
        }

        public IEnumerable<STIGRule> RuleList { get { return this.rules; } }

        public void AddRule(STIGRule item)
        {
            if(this.rules == null)
            {
                this.rules = new List<STIGRule>();
            }

            this.rules.Add(item);
        }

        public STIG() { }

    }
}
