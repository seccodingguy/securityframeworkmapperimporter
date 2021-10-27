using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frameworks_Mapper
{
    public class RMF
    {
        private int id = 0;
        private string step = "";
        private string taskId = "";
        private string taskname = "";
        private string taskdescr = null;
        private List<NISTCyberSec> nistcyberitems = null;

        public int ID { get { return this.id; } set { this.id = value; } }
        
        public string Step { get { return this.step; } set { this.step = value; } }

        public string TaskID { get { return this.taskId; } set { this.taskId = value; } }

        public string TaskName { get { return this.taskname; } set { this.taskname = value; } }

        
        public List<NISTCyberSec> NISTCyberRiskItems { get { return this.nistcyberitems; } set { this.nistcyberitems = value; } }

        public string TaskDescription { get { return this.taskdescr; } set { this.taskdescr = value; } }


        public IEnumerable<NISTCyberSec> NISTCyberRiskMapping { get { return this.nistcyberitems; } }
    }
}
