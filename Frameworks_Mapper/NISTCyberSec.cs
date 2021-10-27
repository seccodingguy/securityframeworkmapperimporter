using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frameworks_Mapper
{
    public class NISTCyberSec
    {
        private string function = "";
        private string category = "";
        private string subCategoryId = "";
        private string subCategoryDescr = "";
        private string categoryId = "";
        private string categoryDescr = "";

        private List<string> nistControlsMapping = null;

        public string Function { get { return this.function; } set { this.function = value; } }

        public string Category { get { return this.category; } set { this.category = value; } }

        public string CategoryID { get { return this.categoryId; } set { this.categoryId = value; } }

        public string CategoryDescription { get { return this.categoryDescr; } set { this.categoryDescr = value; } }

        public string SubCategoryID { get { return this.subCategoryId; } set { this.subCategoryId = value; } }

        public string SubCategoryDescription { get { return this.subCategoryDescr; } set { this.subCategoryDescr = value; } }

        public IEnumerable<string> NIST80053Rev4Controls { get { return this.nistControlsMapping; } }

        public void AddNIST80053Rev4Control(string item)
        {
            if(this.nistControlsMapping == null)
            {
                this.nistControlsMapping = new List<string>();
            }

            this.nistControlsMapping.Add(item);
        }

        public NISTCyberSec() { }


    }
}
