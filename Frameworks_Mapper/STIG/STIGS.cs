using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Frameworks_Mapper
{
    public class STIGS
    {
        private List<STIG> stigs = null;
        private string connStr = "";
        private MySqlConnection conn = null;

        IEnumerable<STIG> STIGList { get { return this.stigs; } }

        public void AddSTIG(STIG item)
        {
            if(this.stigs == null)
            {
                this.stigs = new List<STIG>();
            }

            this.stigs.Add(item);
        }

        public STIGS() { }

        public string ConnectionString { set { this.connStr = value; } }

        public void SaveSTIGS()
        {
            this.conn = new MySqlConnection(this.connStr);
            this.conn.Open();
            
            Console.WriteLine("");

            foreach(STIG item in this.stigs)
            {
                Console.WriteLine("\tSaving STIG {0:S}", item.Title);
                this.saveSTIGDetails(item);
            }

            this.conn.Close();
        }

        private void saveSTIGDetails(STIG item)
        {
            int stigId = 0;

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = this.conn;

            MySqlParameter groupNoParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "@groupno", item.GroupIDNumber);
            cmd.Parameters.Add(groupNoParam);
            cmd.CommandText = SQLStatements.STIGSelectItem;

            object returnValue = cmd.ExecuteScalar();

            if(returnValue == null)
            {
                MySqlParameter descrParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "descr", item.Description);
                MySqlParameter titleParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "title", item.Title);
                MySqlParameter targetParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "target", item.TargetSystem);

                cmd.Parameters.Add(descrParam);
                cmd.Parameters.Add(titleParam);
                cmd.Parameters.Add(targetParam);
                cmd.CommandText = SQLStatements.STIGInsertItem;
                cmd.ExecuteNonQuery();

                cmd.Parameters.Remove(descrParam);
                cmd.Parameters.Remove(titleParam);
                cmd.Parameters.Remove(targetParam);

                cmd.CommandText = SQLStatements.STIGSelectItem;
                returnValue = cmd.ExecuteScalar();
            }

            stigId = (int)returnValue;

            this.saveCCISXREF(item, stigId);

            foreach(STIGRule rule in item.RuleList)
            {
                this.saveSTIGRule(rule, stigId);
            }

        }

        private void saveCCISXREF(STIG item, int stigID)
        {
            if (item.CCIReferenceList != null)
            {
                foreach (string cci in item.CCIReferenceList)
                {
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = this.conn;

                    MySqlParameter ccinoParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "cciid", cci);
                    cmd.Parameters.Add(ccinoParam);
                    cmd.CommandText = SQLStatements.CCISelectItem;
                    object returnValue = cmd.ExecuteScalar();

                    int cciId = (int)returnValue;

                    cmd.Parameters.Clear();

                    MySqlParameter cciIdParam = getSqlParameter(System.Data.DbType.Int32, System.Data.ParameterDirection.Input, "cciid", cciId);
                    MySqlParameter stigIdParam = getSqlParameter(System.Data.DbType.Int32, System.Data.ParameterDirection.Input, "stigid", stigID);
                    cmd.Parameters.Add(cciIdParam);
                    cmd.Parameters.Add(stigIdParam);
                    cmd.CommandText = SQLStatements.STIGCCISelectXREF;
                    returnValue = cmd.ExecuteScalar();

                    if (returnValue == null)
                    {
                        cmd.CommandText = SQLStatements.STIGCCIInsertXREF;
                        cmd.ExecuteNonQuery();
                    }

                }
            }
        }

        private void saveSTIGRule(STIGRule item, int stigID)
        {
            int ruleId = 0;
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = this.conn;

            MySqlParameter ruleIdNoParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "ruleid", item.RuleIDNumber);
            cmd.Parameters.Add(ruleIdNoParam);
            cmd.CommandText = SQLStatements.STIGRuleSelect;

            object returnValue = cmd.ExecuteScalar();


            if(returnValue == null)
            {
                MySqlParameter severityParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "severity", item.Severity);
                MySqlParameter weightParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "weight", item.Weight);
                MySqlParameter versionParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "version", item.Version);
                MySqlParameter descrParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "descr", item.Description);

                cmd.Parameters.Add(severityParam);
                cmd.Parameters.Add(weightParam);
                cmd.Parameters.Add(versionParam);
                cmd.Parameters.Add(descrParam);

                cmd.CommandText = SQLStatements.STIGRuleInsert;
                cmd.ExecuteNonQuery();

                cmd.Parameters.Clear();
                cmd.Parameters.Add(ruleIdNoParam);

                cmd.CommandText = SQLStatements.STIGRuleSelect;

                returnValue = cmd.ExecuteScalar();
            }

            ruleId = (int)returnValue;

            this.saveSTIGRuleXREF(ruleId, stigID);

            this.saveSTIGRuleFix(item.FixDetails, ruleId);
            this.saveSTIGRuleCheck(item.CheckDetails, ruleId);

        }

        private void saveSTIGRuleXREF(int ruleId, int stigId)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = this.conn;

            MySqlParameter ruleIdParam = getSqlParameter(System.Data.DbType.Int32, System.Data.ParameterDirection.Input, "ruleid", ruleId);
            MySqlParameter stigIdParm = getSqlParameter(System.Data.DbType.Int32, System.Data.ParameterDirection.Input, "stigid", stigId);
            cmd.Parameters.Add(ruleIdParam);
            cmd.Parameters.Add(stigIdParm);

            cmd.CommandText = SQLStatements.STIGRuleSelectXREF;

            object returnValue = cmd.ExecuteScalar();

            if(returnValue == null)
            {
                cmd.CommandText = SQLStatements.STIGRuleInsertXREF;
                cmd.ExecuteNonQuery();
            }
        }

        private void saveSTIGRuleFix(STIGFix item, int ruleID)
        {
            //@uifix,@clifix,@descr,@fixid
            int fixId = 0;
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = this.conn;

            MySqlParameter fixIdNoParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "fixid", item.FixIDNumber);
            cmd.Parameters.Add(fixIdNoParam);
            cmd.CommandText = SQLStatements.STIGRuleFixSelect;

            object returnValue = cmd.ExecuteScalar();

            if (returnValue == null)
            {
                MySqlParameter uiFixParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "uifix", item.UIFixSteps);
                MySqlParameter cliFixParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "clifix", item.CLIFixSteps);
                MySqlParameter descrParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "descr", item.Description);
                cmd.Parameters.Add(uiFixParam);
                cmd.Parameters.Add(cliFixParam);
                cmd.Parameters.Add(descrParam);
                cmd.CommandText = SQLStatements.STIGRuleFixInsert;
                cmd.ExecuteNonQuery();

                cmd.Parameters.Clear();

                cmd.Parameters.Add(fixIdNoParam);
                cmd.CommandText = SQLStatements.STIGRuleFixSelect;

                returnValue = cmd.ExecuteScalar();

            }

            fixId = (int)returnValue;

            this.saveSTIGRuleFixXREF(fixId, ruleID);
        }

        private void saveSTIGRuleFixXREF(int fixId, int ruleId)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = this.conn;

            MySqlParameter ruleIdParam = getSqlParameter(System.Data.DbType.Int32, System.Data.ParameterDirection.Input, "ruleid", ruleId);
            MySqlParameter fixIdParm = getSqlParameter(System.Data.DbType.Int32, System.Data.ParameterDirection.Input, "fixid", fixId);
            cmd.Parameters.Add(ruleIdParam);
            cmd.Parameters.Add(fixIdParm);

            cmd.CommandText = SQLStatements.STIGRuleFixSelectXREF;

            object returnValue = cmd.ExecuteScalar();

            if (returnValue == null)
            {
                cmd.CommandText = SQLStatements.STIGRuleFixInsertXREF;
                cmd.ExecuteNonQuery();
            }
        }

        private void saveSTIGRuleCheck(STIGCheck item, int ruleID)
        {
            //@uicheck,@clicheck,@descr,@systemno
            int checkId = 0;
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = this.conn;

            MySqlParameter systemNoParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "systemno", item.CheckSystemNumber);
            cmd.Parameters.Add(systemNoParam);
            cmd.CommandText = SQLStatements.STIGRuleCheckSelect;

            object returnValue = cmd.ExecuteScalar();

            if (returnValue == null)
            {
                MySqlParameter uiCheckParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "uicheck", item.UICheckSteps);
                MySqlParameter cliCheckParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "clicheck", item.CLICheckSteps);
                MySqlParameter descrParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "descr", item.Description);
                cmd.Parameters.Add(uiCheckParam);
                cmd.Parameters.Add(cliCheckParam);
                cmd.Parameters.Add(descrParam);
                cmd.CommandText = SQLStatements.STIGRuleCheckInsert;
                cmd.ExecuteNonQuery();

                cmd.Parameters.Clear();

                cmd.Parameters.Add(systemNoParam);
                cmd.CommandText = SQLStatements.STIGRuleCheckSelect;

                returnValue = cmd.ExecuteScalar();

            }

            checkId = (int)returnValue;

            this.saveSTIGRuleCheckXREF(checkId, ruleID);
        }

        private void saveSTIGRuleCheckXREF(int checkId, int ruleId)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = this.conn;

            MySqlParameter ruleIdParam = getSqlParameter(System.Data.DbType.Int32, System.Data.ParameterDirection.Input, "ruleid", ruleId);
            MySqlParameter checkIdParm = getSqlParameter(System.Data.DbType.Int32, System.Data.ParameterDirection.Input, "checkid", checkId);
            cmd.Parameters.Add(ruleIdParam);
            cmd.Parameters.Add(checkIdParm);

            cmd.CommandText = SQLStatements.STIGRuleCheckSelectXREF;

            object returnValue = cmd.ExecuteScalar();

            if (returnValue == null)
            {
                cmd.CommandText = SQLStatements.STIGRuleCheckInsertXREF;
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
