using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Frameworks_Mapper
{
    public class RMFItems
    {
        List<RMF> rmfList = null;
        private string connStr = "";
        private MySqlConnection conn = null;

        public string ConnectionString { set { this.connStr = value; } }

        public void AddRMFItem(RMF item)
        {
            if (this.rmfList == null)
            {
                this.rmfList = new List<RMF>();
            }

            this.rmfList.Add(item);
        }

        public RMFItems() { }

        public void SaveRMFItems()
        {
            this.conn = new MySqlConnection(this.connStr);
            this.conn.Open();

            foreach (RMF item in this.rmfList)
            {
                Console.WriteLine("Saving RMF Task {0:S}", item.TaskName);

                this.saveRMF(item);
            }

            this.conn.Close();
        }

        private void saveRMF(RMF item)
        {
            int rmfId = 0;
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = this.conn;

            MySqlParameter rmfParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "taskid", item.TaskID);
            MySqlParameter rmfDescr = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "taskdescr", item.TaskDescription);
            cmd.Parameters.Add(rmfDescr);
            cmd.Parameters.Add(rmfParam);
            cmd.CommandText = SQLStatements.RMFSelectWithTaskDescr;

            object returnValue = cmd.ExecuteScalar();

            if(returnValue == null)
            {
                MySqlParameter stepParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "step", item.Step);
                MySqlParameter nameParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "taskname", item.TaskName);
                MySqlParameter taskDescrParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "descr", item.TaskDescription);
                cmd.Parameters.Add(stepParam);
                cmd.Parameters.Add(nameParam);
                cmd.Parameters.Add(taskDescrParam);
                cmd.CommandText = SQLStatements.RMFInsert;
                cmd.ExecuteNonQuery();

                cmd.Parameters.Remove(stepParam);
                cmd.Parameters.Remove(nameParam);
                

                cmd.CommandText = SQLStatements.RMFSelectWithTaskDescr;
                returnValue = cmd.ExecuteScalar();
            }

            rmfId = (int)returnValue;

            if(item.NISTCyberRiskItems != null)
            {
                this.saveNISTCyberRiskMapping(rmfId, item.NISTCyberRiskItems);
            }

            
        }

        private void saveNISTCyberRiskMapping(int rmfId, List<NISTCyberSec> cyberRiskItems)
        {
            foreach (NISTCyberSec item in cyberRiskItems)
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = this.conn;

                if (item.CategoryID != String.Empty)
                {
                    this.saveNISTCyberMappingWithCategory(rmfId, item.CategoryID); 
                }
                else
                {
                    this.saveNISTCyberMappingWithSubCategory(rmfId, item.SubCategoryID);
                }
            }
        }

        private void saveNISTCyberMappingWithSubCategory(int rmfId, string subCatId)
        {
            int cyberId = 0;
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = this.conn;
            MySqlParameter subcatIdParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "subcatid", subCatId);
            cmd.Parameters.Add(subcatIdParam);
            cmd.CommandText = SQLStatements.NISTCyberSelect;

            object returnValue = cmd.ExecuteScalar();

            if(returnValue != null)
            {
                cyberId = (int)returnValue;
                cmd.Parameters.Remove(subcatIdParam);
                MySqlParameter rmfIdParam = getSqlParameter(System.Data.DbType.Int32, System.Data.ParameterDirection.Input, "rmfid", rmfId);
                MySqlParameter cyberIdParam = getSqlParameter(System.Data.DbType.Int32, System.Data.ParameterDirection.Input, "cyberid", cyberId);

                cmd.Parameters.Add(rmfIdParam);
                cmd.Parameters.Add(cyberIdParam);
                cmd.CommandText = SQLStatements.RMFNISTCyberSelectXREF;

                returnValue = cmd.ExecuteScalar();

                if (returnValue == null)
                {

                    cmd.CommandText = SQLStatements.RMFNISTCyberInsertXREF;
                    cmd.ExecuteNonQuery();

                }
            }
        }

        private void saveNISTCyberMappingWithCategory(int rmfId, string catId)
        {
            int cyberId = 0;
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = this.conn;
            MySqlParameter catIdParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "catid", catId);
            cmd.Parameters.Add(catIdParam);
            cmd.CommandText = SQLStatements.NISTCyberSelectByCategoryID;

            MySqlDataReader reader = cmd.ExecuteReader();
            
            while (reader.Read())
            {
                cyberId = (int)reader["id"];
                //cmd.Parameters.Remove(catIdParam);
                MySqlCommand xrefCmd = new MySqlCommand();
                MySqlConnection newConn = new MySqlConnection(this.connStr);
                newConn.Open();
                xrefCmd.Connection = newConn;
                
                MySqlParameter rmfIdParam = getSqlParameter(System.Data.DbType.Int32, System.Data.ParameterDirection.Input, "rmfid", rmfId);
                MySqlParameter cyberIdParam = getSqlParameter(System.Data.DbType.Int32, System.Data.ParameterDirection.Input, "cyberid", cyberId);

                xrefCmd.Parameters.Add(rmfIdParam);
                xrefCmd.Parameters.Add(cyberIdParam);
                xrefCmd.CommandText = SQLStatements.RMFNISTCyberSelectXREF;

                object returnValue = xrefCmd.ExecuteScalar();

                if (returnValue == null)
                {

                    xrefCmd.CommandText = SQLStatements.RMFNISTCyberInsertXREF;
                    xrefCmd.ExecuteNonQuery();

                }

                xrefCmd.Dispose();
                newConn.Close();
            }

            reader.Close();
            cmd.Dispose();
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
