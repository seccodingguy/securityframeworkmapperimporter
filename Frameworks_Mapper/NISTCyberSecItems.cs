using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Frameworks_Mapper
{
    public class NISTCyberSecItems
    {
        private List<NISTCyberSec> nistCyberItems = null;
        private string connStr = "";
        private MySqlConnection conn = null;

        public string ConnectionString { set { this.connStr = value; } }

        public void AddNISTCyberSecurityItem(NISTCyberSec item)
        {
            if (this.nistCyberItems == null)
            {
                this.nistCyberItems = new List<NISTCyberSec>();
            }

            this.nistCyberItems.Add(item);
        }

        public NISTCyberSecItems() { }

        public void SaveNISTCyberItems()
        {
            this.conn = new MySqlConnection(this.connStr);
            this.conn.Open();

            foreach(NISTCyberSec item in this.nistCyberItems)
            {
                Console.WriteLine("Saving NIST Cyber Security {0:S}", item.SubCategoryID);

                this.saveNISTCyberSec(item);
            }

            this.conn.Close();
        }

        private void saveNISTCyberSec(NISTCyberSec item)
        {
            //@functionname,@catname,@catnameid,@subcatid,@subcatdescr
            int nistCyberId = 0;
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = this.conn;

            MySqlParameter subCatParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "subcatid", item.SubCategoryID);
            cmd.Parameters.Add(subCatParam);
            cmd.CommandText = SQLStatements.NISTCyberSelect;

            object returnValue = cmd.ExecuteScalar();

            if(returnValue == null)
            {
                MySqlParameter functionParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "functionname", item.Function);
                MySqlParameter categoryParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "catname", item.Category);
                MySqlParameter categoryIdParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "catnameid", item.CategoryID);
                MySqlParameter categoryDescrParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "catnamedescr", item.CategoryDescription);
                MySqlParameter subCatIdParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "subcatid", item.SubCategoryID);
                MySqlParameter subCatDescrParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "subcatdescr", item.SubCategoryDescription);
                cmd.Parameters.Add(functionParam);
                cmd.Parameters.Add(categoryParam);
                cmd.Parameters.Add(categoryIdParam);
                cmd.Parameters.Add(categoryDescrParam);
                cmd.Parameters.Add(subCatDescrParam);

                cmd.CommandText = SQLStatements.NISTCyberInsert;
                cmd.ExecuteNonQuery();

                cmd.Parameters.Clear();

                cmd.Parameters.Add(subCatParam);
                cmd.CommandText = SQLStatements.NISTCyberSelect;
                returnValue = cmd.ExecuteScalar();

            }

            nistCyberId = (int)returnValue;

            foreach(string nistControl in item.NIST80053Rev4Controls)
            {
                this.saveNISTCyberSecNISTControlXREF(nistCyberId, nistControl);
            }
        }

        private void saveNISTCyberSecNISTControlXREF(int nistCyberSecId, string nistControlNo)
        {
            int nistId = 0;
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = this.conn;

            MySqlParameter nistControlParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "nistcontrol", nistControlNo);
            MySqlParameter revisionParam = getSqlParameter(System.Data.DbType.Int32, System.Data.ParameterDirection.Input, "revision", 4);
            cmd.Parameters.Add(nistControlParam);
            cmd.Parameters.Add(revisionParam);
            cmd.CommandText = SQLStatements.NISTSelectControl;
            object returnValue = cmd.ExecuteScalar();

            if(returnValue != null)
            {
                nistId = (int)returnValue;

                cmd.Parameters.Clear();

                MySqlParameter nistIdParam = getSqlParameter(System.Data.DbType.Int32, System.Data.ParameterDirection.Input, "nistid", nistId);
                MySqlParameter nistCyberIdParam = getSqlParameter(System.Data.DbType.Int32, System.Data.ParameterDirection.Input, "cyberid", nistCyberSecId);
                cmd.Parameters.Add(nistIdParam);
                cmd.Parameters.Add(nistCyberIdParam);
                cmd.CommandText = SQLStatements.NISTCyberNISTControlsSelectXREF;

                returnValue = cmd.ExecuteScalar();

                if(returnValue == null)
                {
                    cmd.CommandText = SQLStatements.NISTCyberNISTControlsInsertXREF;
                    cmd.ExecuteNonQuery();
                }
            }

        }

        public static MySqlParameter getSqlParameter(System.Data.DbType dataType, System.Data.ParameterDirection direction, string parameterName, object parameterValue = null)
        {
            MySql.Data.MySqlClient.MySqlParameter retVal = new MySql.Data.MySqlClient.MySqlParameter();

            retVal.DbType = dataType;
            retVal.ParameterName = parameterName;
            retVal.Direction = direction;

            if (parameterValue != null)
            {
                retVal.Value = parameterValue;

            }

            return retVal;
        }
    }
}
