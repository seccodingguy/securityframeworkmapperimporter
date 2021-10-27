using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frameworks_Mapper
{
    public class CCMControl
    {
        private string ccmdomain = "";
        private int ccmdomainid = 0;
        private string ccmnumber = "";
        private string ccmspecification = "";
        private List<NISTControl> mappedNISTControls = null;
        private int ccmid = 0;

        public string CCMDomain { get { return this.ccmdomain; } set { this.ccmdomain = value; } }

        public int CCMDomainID { get { return this.ccmdomainid; } set { this.ccmdomainid = value; } }

        public string CCMNumber { get { return this.ccmnumber; } set { this.ccmnumber = value; } }

        public string CCMSpecification { get { return this.ccmspecification; } set { this.ccmspecification = value; } }


        public IEnumerable<NISTControl> MappedNISTControls { get { return this.mappedNISTControls; } }

        public void AddNISTControl(NISTControl mappedControl)
        {
            if(this.mappedNISTControls == null)
            {
                this.mappedNISTControls = new List<NISTControl>();
            }

            this.mappedNISTControls.Add(mappedControl);
        }

        public int CCMID { get { return this.ccmid; } set { this.ccmid = value; } }
    }
}
