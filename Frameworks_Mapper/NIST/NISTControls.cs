using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Frameworks_Mapper
{
    public class NISTControls
    {
        private List<NISTControl> nistControlCollection = null;
        private string connStr = "";
        private MySqlConnection conn = null;

        public IEnumerable<NISTControl> NISTControlsCollection { get { return this.nistControlCollection; } }

        public void AddNISTControl(NISTControl item)
        {
            if (this.nistControlCollection == null)
            {
                this.nistControlCollection = new List<NISTControl>();
            }

            this.nistControlCollection.Add(item);

        }

        public string ConnectionString { set { this.connStr = value; } }
            
        public void SaveNISTControls()
        {
            this.conn = new MySqlConnection(this.connStr);
            this.conn.Open();

            foreach(NISTControl control in this.nistControlCollection)
            {
                saveNISTControl(control);
            }

            this.conn.Close();
            
        }

        private void saveNISTControl(NISTControl currentRecord)
        {
            Console.WriteLine("Saving NIST {0:S}", currentRecord.NISTNumber);

            currentRecord.NISTFamilyID = saveFamily(currentRecord.NISTFamilyName.Trim());

            if (currentRecord.NISTClassName != "")
            {
                currentRecord.NISTClassID = saveClass(currentRecord.NISTClassName.Trim());
            }

            saveControl(currentRecord);
            
        }

        private void saveControl(NISTControl item)
        {
            //@number,@title,@impact,@priority,@descr,@guidance,@classid,@familyid
            MySqlParameter numberParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "nistcontrol", item.NISTNumber.Trim());
            MySqlParameter revisionParam = getSqlParameter(System.Data.DbType.Int32, System.Data.ParameterDirection.Input, "revision", item.NISTRevisionNumber);
            MySqlCommand cmd = new MySqlCommand(SQLStatements.NISTSelectControl, this.conn);
            cmd.Parameters.Add(numberParam);
            cmd.Parameters.Add(revisionParam);

            object returnValue = cmd.ExecuteScalar();

            if (returnValue == null)
            {
                cmd.Parameters.Remove(numberParam);

                numberParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "number", item.NISTNumber.Trim());
                MySqlParameter titleParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "title", item.NISTTitle.Trim());
                MySqlParameter impactParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "impact", item.NISTImpact);
                MySqlParameter priorityParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "priority", item.NISTPriority);
                MySqlParameter descrParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "descr", item.NISTDescription);
                MySqlParameter guidanceParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "guidance", item.NISTGuidance);
                MySqlParameter classIdParam = getSqlParameter(System.Data.DbType.Int32, System.Data.ParameterDirection.Input, "classid", item.NISTClassID);
                MySqlParameter familyIdParam = getSqlParameter(System.Data.DbType.Int32, System.Data.ParameterDirection.Input, "familyid", item.NISTFamilyID);
                

                cmd.Parameters.Add(numberParam);
                cmd.Parameters.Add(titleParam);
                cmd.Parameters.Add(impactParam);
                cmd.Parameters.Add(priorityParam);
                cmd.Parameters.Add(descrParam);
                cmd.Parameters.Add(guidanceParam);
                cmd.Parameters.Add(classIdParam);
                cmd.Parameters.Add(familyIdParam);
                

                cmd.CommandText = SQLStatements.NISTInsertControl;
                cmd.ExecuteNonQuery();
            }
        }

        private int saveFamily(string nistFamilyName)
        {
            int retVal = 0;

            MySqlParameter familyNameParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "familyname", nistFamilyName);
            
            MySqlCommand cmd = new MySqlCommand(SQLStatements.NISTSelectFamily, this.conn);
            cmd.Parameters.Add(familyNameParam);
            object returnValue = cmd.ExecuteScalar();

            if (returnValue != null)
            {
                retVal = (int)returnValue;
            }

            if(retVal == 0)
            {
                cmd.CommandText = SQLStatements.NISTInsertFamily;
                cmd.ExecuteNonQuery();

                cmd.CommandText = SQLStatements.NISTSelectFamily;
                retVal = (int)cmd.ExecuteScalar();
            }

            return retVal;

        }

        private int saveClass(string nistClassName)
        {
            int retVal = 0;

            MySqlParameter familyNameParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "classname", nistClassName);

            MySqlCommand cmd = new MySqlCommand(SQLStatements.NISTSelectClass, this.conn);
            cmd.Parameters.Add(familyNameParam);
            object returnValue = cmd.ExecuteScalar();

            if (returnValue != null)
            {
                retVal = (int)returnValue;
            }

            if (retVal == 0)
            {
                cmd.CommandText = SQLStatements.NISTInsertClass;
                cmd.ExecuteNonQuery();

                cmd.CommandText = SQLStatements.NISTSelectClass;
                retVal = (int)cmd.ExecuteScalar();
            }

            return retVal;
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
