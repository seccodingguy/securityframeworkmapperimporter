using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Frameworks_Mapper
{
    public class NISTControl
    {
        private string nistclass = "";
        private string nistnumber = "";
        private string nistfamily = "";
        private string nisttitle = "";
        private string nistpriority = "";
        private string nistdescription = "";
        private string nistimpact = "";
        private string nistguidance = "";
        
        private int classId = 0;
        private int familyId = 0;
        private int nistid = 0;
        private int revisionNumber = 0;
        
        public string NISTClassName { get { return this.nistclass; } set { this.nistclass = value; } }

        public int NISTClassID { get { return this.classId; } set { this.classId = value; } }

        public string NISTNumber { get { return this.nistnumber; } set { this.nistnumber = value; } }

        public string NISTFamilyName { get { return this.nistfamily; } set { this.nistfamily = value; } }

        public int NISTFamilyID { get { return this.familyId; } set { this.familyId = value; } }

        public string NISTTitle { get { return this.nisttitle; } set { this.nisttitle = value; } }

        public string NISTPriority { get { return this.nistpriority; } set { this.nistpriority = value; } }

        public string NISTDescription { get { return this.nistdescription; } set { this.nistdescription = value; } }

        public string NISTImpact { get { return this.nistimpact; } set { this.nistimpact = value; } }

        public string NISTGuidance { get { return this.nistguidance; } set { this.nistguidance = value; } }

        public int NISTID { get { return this.nistid; } set { this.nistid = value; } }

        public int NISTRevisionNumber { get { return this.revisionNumber; } set { this.revisionNumber = value; } }

        



    }
}
