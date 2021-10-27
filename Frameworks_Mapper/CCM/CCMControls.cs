using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Frameworks_Mapper
{
    public class CCMControls
    {
        private List<CCMControl> ccmControlCollection = null;
        private string connStr = "";
        private MySqlConnection conn = null;

        public IEnumerable<CCMControl> CCMControlsCollection { get { return this.ccmControlCollection; } }

        public void AddCCMControl(CCMControl item)
        {
            if (this.ccmControlCollection == null)
            {
                this.ccmControlCollection = new List<CCMControl>();
            }

            this.ccmControlCollection.Add(item);

        }

        public string ConnectionString { set { this.connStr = value; } }

        public void SaveCCMControls()
        {
            this.conn = new MySqlConnection(this.connStr);
            this.conn.Open();

            foreach (CCMControl control in this.ccmControlCollection)
            {
                saveCCMControl(control);
            }

            this.conn.Close();

        }

        private void saveCCMControl(CCMControl currentRecord)
        {
            Console.WriteLine("Saving CCM {0:S}", currentRecord.CCMNumber);

            currentRecord.CCMDomainID = saveDomain(currentRecord.CCMDomain);
            //currentRecord.NISTClassID = saveClass(currentRecord.NISTClassName);
            saveControl(currentRecord);

        }

        private int saveDomain(string domainName)
        {
            int retVal = 0;

            MySqlParameter domainNameParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "domainname", domainName);

            MySqlCommand cmd = new MySqlCommand(SQLStatements.CCMSelectDomain, this.conn);
            cmd.Parameters.Add(domainNameParam);
            object returnValue = cmd.ExecuteScalar();

            if (returnValue != null)
            {
                retVal = (int)returnValue;
            }

            if (retVal == 0)
            {
                cmd.CommandText = SQLStatements.CCMInsertDomain;
                cmd.ExecuteNonQuery();

                cmd.CommandText = SQLStatements.CCMSelectDomain;
                retVal = (int)cmd.ExecuteScalar();
            }

            return retVal;

        }

        private void saveControl(CCMControl item)
        {
            //@ccmcontrolid,@specification,@domainid
            MySqlParameter numberParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "ccmcontrolid", item.CCMNumber);
            
            MySqlCommand cmd = new MySqlCommand(SQLStatements.CCMSelectControl, this.conn);
            cmd.Parameters.Add(numberParam);

            object returnValue = cmd.ExecuteScalar();

            if (returnValue == null)
            {
                MySqlParameter specificationParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "specification", item.CCMSpecification);
                MySqlParameter domainParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "domainid", item.CCMDomainID);
                cmd.Parameters.Add(specificationParam);
                cmd.Parameters.Add(domainParam);

                cmd.CommandText = SQLStatements.CCMInsertControl;
                cmd.ExecuteNonQuery();

                cmd.Parameters.Remove(specificationParam);
                cmd.Parameters.Remove(domainParam);

                cmd.CommandText = SQLStatements.CCMSelectControl;
                returnValue = cmd.ExecuteScalar();

                item.CCMID = (int)returnValue;
            }
            else
            {
                item.CCMID = (int)returnValue;
            }

            saveXREF(item);
        }

        private void saveXREF(CCMControl item)
        {
            MySqlParameter ccmidParam = getSqlParameter(System.Data.DbType.Int32, System.Data.ParameterDirection.Input, "cloudid", item.CCMID);
            
            
            object returnValue = null;

            foreach(NISTControl nistItem in item.MappedNISTControls)
            {
                if (nistItem.NISTNumber.Length > 3)
                {
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = this.conn;

                    MySqlParameter nistNumberParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "nistcontrol", nistItem.NISTNumber.Trim());
                    MySqlParameter revisionParam = getSqlParameter(System.Data.DbType.Int32, System.Data.ParameterDirection.Input, "revision", nistItem.NISTRevisionNumber);

                    cmd.CommandText = SQLStatements.NISTSelectControl;
                    cmd.Parameters.Add(nistNumberParam);
                    cmd.Parameters.Add(revisionParam);

                    returnValue = cmd.ExecuteScalar();

                    if (returnValue != null)
                    {
                        MySqlParameter nistIdParam = getSqlParameter(System.Data.DbType.Int32, System.Data.ParameterDirection.Input, "nistid", returnValue);

                        cmd.Parameters.Remove(nistNumberParam);
                        cmd.Parameters.Remove(revisionParam);

                        cmd.Parameters.Add(nistIdParam);
                        cmd.Parameters.Add(ccmidParam);

                        cmd.CommandText = SQLStatements.CCMSelectNISTXREF;
                        returnValue = cmd.ExecuteScalar();

                        if (returnValue == null)
                        {
                            cmd.CommandText = SQLStatements.CCMInserNISTXREF;
                            cmd.ExecuteNonQuery();
                        }
                    }

                    cmd.Dispose();
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
