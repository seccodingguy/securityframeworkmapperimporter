using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Frameworks_Mapper
{
    public class CCILists
    {
        private List<CCIList> cciItems = null;
        private string connStr = "";
        private MySqlConnection conn = null;
        private int revisionNo = 0;

        public IEnumerable<CCIList> CCIItemsCollection { get { return this.cciItems; } }
        
        public void AddCCEList(CCIList item)
        {
            if(this.cciItems == null)
            {
                this.cciItems = new List<CCIList>();
            }

            this.cciItems.Add(item);
        }

        public int NISTRevisionNumber { get { return this.revisionNo; } set { this.revisionNo = value; } }
        public string ConnectionString { set { this.connStr = value; } }

        public void SaveCCIListItems()
        {
            this.conn = new MySqlConnection(this.connStr);
            this.conn.Open();

            foreach (CCIList control in this.cciItems)
            {
                saveCCIItem(control);
            }

            this.conn.Close();
        }

        private void saveCCIItem(CCIList item)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = this.conn;

            MySqlParameter cciIdNoParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "cciid", item.CCIIDNo);
            cmd.Parameters.Add(cciIdNoParam);
            cmd.CommandText = SQLStatements.CCISelectItem;

            object returnValue = cmd.ExecuteScalar();
            int cciid = 0;

            if (returnValue == null)
            {
                MySqlParameter cciDescrParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "definition", item.CCIDescription);
                MySqlParameter cciTypeParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "type", item.CCIType);

                cmd.Parameters.Add(cciDescrParam);
                cmd.Parameters.Add(cciTypeParam);
                cmd.CommandText = SQLStatements.CCIInsertItem;
                cmd.ExecuteNonQuery();

                cmd.Parameters.Remove(cciDescrParam);
                cmd.Parameters.Remove(cciTypeParam);

                cmd.CommandText = SQLStatements.CCISelectItem;

                returnValue = cmd.ExecuteScalar();


            }

            cciid = (int)returnValue;

            if (item.NISTReferences != null)
            {
                foreach (string nistIdNo in item.NISTReferences)
                {
                    saveCCINISTXREF(cciid, nistIdNo);
                }
            }

        }

        private void saveCCINISTXREF(int cciId, string nistIdNo)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = this.conn;

            MySqlParameter nistIdNoParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "nistcontrol", nistIdNo.Trim());
            MySqlParameter revisionParam = getSqlParameter(System.Data.DbType.Int32, System.Data.ParameterDirection.Input, "revision", this.revisionNo);
            cmd.Parameters.Add(nistIdNoParam);
            cmd.Parameters.Add(revisionParam);
            cmd.CommandText = SQLStatements.NISTSelectControl;

            object returnValue = cmd.ExecuteScalar();

            cmd.Parameters.Clear();

            if(returnValue == null)
            {
                Console.WriteLine("");
            }

            MySqlParameter nistIdParam = getSqlParameter(System.Data.DbType.Int32, System.Data.ParameterDirection.Input, "nistid", returnValue);
            MySqlParameter cciIdParam = getSqlParameter(System.Data.DbType.Int32, System.Data.ParameterDirection.Input, "cciid", cciId);
            cmd.Parameters.Add(nistIdParam);
            cmd.Parameters.Add(cciIdParam);
            cmd.CommandText = SQLStatements.CCISelectNISTXREF;

            returnValue = cmd.ExecuteScalar();

            if(returnValue == null)
            {
                cmd.CommandText = SQLStatements.CCIInsertNISTXREF;
                cmd.ExecuteNonQuery();
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
