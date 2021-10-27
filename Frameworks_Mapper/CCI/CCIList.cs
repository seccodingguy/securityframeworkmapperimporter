using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frameworks_Mapper
{
    public class CCIList
    {
        private string cciidno = "";
        private string descr = "";
        private string type = "";
        private List<string> nistreferences = null;

        public string CCIIDNo { get { return this.cciidno; } set { this.cciidno = value; } }

        public string CCIDescription { get { return this.descr; } set { this.descr = value; } }

        public string CCIType { get { return this.type; } set { this.type = value; } }

        public IEnumerable<string> NISTReferences { get { return this.nistreferences; } }

        public void AddNISTReference(string item)
        {
            if(this.nistreferences == null)
            {
                this.nistreferences = new List<string>();
            }

            this.nistreferences.Add(item);
        }
    }
}
