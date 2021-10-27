using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frameworks_Mapper
{
    public class STIGRule
    {
        private string ruleIdNo = "";
        private string severity = "";
        private string weight = "";
        private string version = "";
        private string descr = "";
        private STIGFix fixDetail = null;
        private STIGCheck checkDetail = null;
        
        public string RuleIDNumber { get { return this.ruleIdNo; } set { this.ruleIdNo = value; } }

        public string Severity { get { return this.severity; } set { this.severity = value; } }

        public string Weight { get { return this.weight; } set { this.weight = value; } }

        public string Version { get { return this.version; } set { this.version = value; } }

        public string Description { get { return this.descr; } set { this.descr = value; } }

        public STIGFix FixDetails { get { return this.fixDetail; } set { this.fixDetail = value; } }

        public STIGCheck CheckDetails { get { return this.checkDetail; } set { this.checkDetail = value; } }

        public STIGRule() { }

    }
}
