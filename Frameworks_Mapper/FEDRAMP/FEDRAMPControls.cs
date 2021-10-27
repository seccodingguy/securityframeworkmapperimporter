using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.IO;
using CsvHelper;

namespace Frameworks_Mapper
{
    public class FEDRAMPControls
    {
        private List<FEDRAMPControl> fedrampControlCollection = null;
        private string connStr = "";
        private MySqlConnection conn = null;

        public IEnumerable<FEDRAMPControl> NISTControlsCollection { get { return this.fedrampControlCollection; } }

        public void AddFEDRAMPControl(FEDRAMPControl item)
        {
            if (this.fedrampControlCollection == null)
            {
                this.fedrampControlCollection = new List<FEDRAMPControl>();
            }

            this.fedrampControlCollection.Add(item);

        }

        public string ConnectionString { set { this.connStr = value; } }

        public void SaveFEDRAMPControls(string csvFileName, string fedRampLevel)
        {
            StreamReader fedrampControlFile = File.OpenText(csvFileName);
            CsvReader fedrampControlReader = new CsvReader(fedrampControlFile, System.Globalization.CultureInfo.CurrentCulture);
            //FEDRAMPControls fedrampControlList = new FEDRAMPControls();

            string sortid = "";
            string nistcontrolid = "";
            string nistdescr = "";
            string controlname = "";
            string objective = "";

            int counter = 0;

            while (fedrampControlReader.Read())
            {
                FEDRAMPControl currentControl = new FEDRAMPControl();
                currentControl.FEDRAMPLevel = fedRampLevel;
                currentControl.NISTRevisionNumber = 4;
                sortid = fedrampControlReader.GetField(1).Trim();

                if (counter > 0 && sortid != "")
                {


                    nistcontrolid = fedrampControlReader.GetField(3).Trim();
                    controlname = fedrampControlReader.GetField(4).Trim();

                    string[] descrsplitval = { "The organization" };
                    string[] descrval = fedrampControlReader.GetField(5).Split(descrsplitval, StringSplitOptions.RemoveEmptyEntries);

                    if (descrval.Length > 0)
                    {
                        string[] reqsplitval = { "Supplemental Guidance" };
                        string[] requirementval = descrval[0].Split(reqsplitval, StringSplitOptions.RemoveEmptyEntries);

                        currentControl.FEDRAMPDescription = requirementval[0].Replace(':', ' ').Trim();

                        if (requirementval.Length > 1)
                        {
                            currentControl.FEDRAMPSupplementalGuidance = requirementval[1].Replace(':', ' ').Trim();
                        }

                    }
                    else
                    {

                        currentControl.FEDRAMPDescription = descrval[0].Replace(':', ' ').Trim();
                    }

                    currentControl.FEDRAMPSortID = sortid.Trim();

                    char[] splitNistID = { '(', ')' };
                    string[] nistidParsing = nistcontrolid.Trim().Split(splitNistID, StringSplitOptions.RemoveEmptyEntries);

                    if (nistidParsing.Length > 0)
                    {
                        currentControl.NISTControlID = nistidParsing[0].Trim();
                    }
                    else
                    {
                        currentControl.NISTControlID = nistcontrolid;
                    }

                    currentControl.FEDRAMPControlName = controlname;

                    if (fedrampControlReader.GetField(6) != null && fedrampControlReader.GetField(6) != "")
                    {
                        currentControl.AddControlParameters(this.getControlParameters(fedrampControlReader.GetField(6), currentControl.NISTControlID));
                    }

                    if (fedrampControlReader.GetField(7) != null && fedrampControlReader.GetField(7) != "")
                    {
                        string rowVal = fedrampControlReader.GetField(7);

                        if (rowVal != "" && rowVal != " ")
                        {
                            currentControl.AddControlRequirements(this.getControlRequirements(rowVal, currentControl.NISTControlID));
                        }
                    }

                    this.AddFEDRAMPControl(currentControl);
                    //fedrampControlList.AddFEDRAMPControl(currentControl);
                }

                counter++;

            }

            this.ConnectionString = "SERVER = localhost; DATABASE = frameworks; UID = mwireman; PASSWORD = shinobu5";
            this.SaveFEDRAMPControls();
        }

        private List<ControlRequirement> getControlRequirements(string rowVal, string nistControlId)
        {
            List<ControlRequirement> retVal = new List<ControlRequirement>();

            string[] requirementSplitVal = { "Requirement" };
            string[] requirementVals = rowVal.Split(requirementSplitVal, StringSplitOptions.RemoveEmptyEntries);

            if (requirementVals.Length > 1)
            {
                if (requirementVals.Length > 2)
                {
                    for (int j = 1; j < requirementVals.Length; j++)
                    {
                        ControlRequirement cr = new ControlRequirement();
                        cr.RequirementText = requirementVals[j];
                        cr.NISTControlID = nistControlId;

                        retVal.Add(cr);
                    }
                }
                else
                {
                    ControlRequirement cr = new ControlRequirement();
                    cr.RequirementText = requirementVals[requirementVals.Length - 1];
                    cr.NISTControlID = nistControlId;
                }
            }


            return retVal;
        }

        private List<ControlParameter> getControlParameters(string rowVal, string nistControlId)
        {
            List<ControlParameter> retVal = new List<ControlParameter>();
            char[] splitVals = { '[', ']' };

            string[] selectionVals = rowVal.Split(splitVals, StringSplitOptions.RemoveEmptyEntries);

            if (selectionVals.Length > 1)
            {
                if (selectionVals.Length > 2)
                {
                    for (int i = 1; i < selectionVals.Length; i = i + 2)
                    {
                        ControlParameter cp = new ControlParameter();
                        cp.ParameterText = selectionVals[i];
                        cp.NISTControlID = nistControlId;
                        retVal.Add(cp);
                    }
                }
                else
                {
                    ControlParameter cp = new ControlParameter();
                    cp.ParameterText = selectionVals[1];
                    cp.NISTControlID = nistControlId;
                    retVal.Add(cp);
                }

            }

            return retVal;
        }

        public void SaveFEDRAMPControls()
        {
            this.conn = new MySqlConnection(this.connStr);
            this.conn.Open();

            foreach(FEDRAMPControl item in this.fedrampControlCollection)
            {
                Console.WriteLine("Saving FedRAMP ID {0:S}", item.FEDRAMPSortID);
                saveControl(item);
            }

            this.conn.Close();
        }

        private void saveControl(FEDRAMPControl item)
        {
            int fedrampId = 0;

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = this.conn;

            MySqlParameter nistControlParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "nistcontrol", item.NISTControlID);
            MySqlParameter nistRevParam = getSqlParameter(System.Data.DbType.Int32, System.Data.ParameterDirection.Input, "revision", item.NISTRevisionNumber);
            
            cmd.Parameters.Add(nistControlParam);
            cmd.Parameters.Add(nistRevParam);
            cmd.CommandText = SQLStatements.NISTSelectControl;

            object returnValue = cmd.ExecuteScalar();

            int nistid = (int)returnValue;

            cmd.Parameters.Remove(nistControlParam);
            cmd.Parameters.Remove(nistRevParam);

            MySqlParameter sortidParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "sortid", item.FEDRAMPSortID);
            cmd.Parameters.Add(sortidParam);
            cmd.CommandText = SQLStatements.FEDRAMPSelectControl;

            returnValue = cmd.ExecuteScalar();
            
            if(returnValue == null)
            {
                MySqlParameter levelParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "level", item.FEDRAMPLevel);
                MySqlParameter controlNameParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "controlname", item.FEDRAMPControlName);
                MySqlParameter controlDescrParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "controldescr", item.FEDRAMPDescription);
                MySqlParameter supplementalDescrParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "guidance", item.FEDRAMPSupplementalGuidance);

                cmd.Parameters.Add(levelParam);
                cmd.Parameters.Add(controlDescrParam);
                cmd.Parameters.Add(controlNameParam);
                cmd.Parameters.Add(supplementalDescrParam);
                
                cmd.CommandText = SQLStatements.FEDRAMPInsertControl;
                cmd.ExecuteNonQuery();

                cmd.Parameters.Remove(levelParam);

                cmd.CommandText = SQLStatements.FEDRAMPSelectControl;

                fedrampId = (int)cmd.ExecuteScalar();
            }
            else
            {
                fedrampId = (int)returnValue;
            }

            saveFedRampNISTXREF(fedrampId, nistid);

            if(item.FEDRAMPRequirements != null)
            {
                saveControlRequirements(item.FEDRAMPRequirements, fedrampId, nistid);
            }

            if(item.FEDRAMPSelectionParams != null)
            {
                saveControlParameters(item.FEDRAMPSelectionParams, fedrampId, nistid);
            }
        }

        private void saveFedRampNISTXREF(int fedrampId, int nistId)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = this.conn;

            MySqlParameter fedrampIdParam = getSqlParameter(System.Data.DbType.Int32, System.Data.ParameterDirection.Input, "fedrampid", fedrampId);
            MySqlParameter nistIdParam = getSqlParameter(System.Data.DbType.Int32, System.Data.ParameterDirection.Input, "nistid", nistId);
            cmd.Parameters.Add(fedrampIdParam);
            cmd.Parameters.Add(nistIdParam);

            cmd.CommandText = SQLStatements.FEDRAMPSelectNISTXREF;

            object returnValue = cmd.ExecuteScalar();

            if(returnValue == null)
            {
                cmd.CommandText = SQLStatements.FEDRAMPSaveNISTXREF;
                cmd.ExecuteNonQuery();
            }
        }

        private void saveControlRequirements(List<ControlRequirement> requirements, int fedrampId, int nistId)
        {
            foreach(ControlRequirement item in requirements)
            {
                string requirementText = item.RequirementText;
                int requirementid = 0;

                MySqlParameter controlParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "requirement", requirementText);
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = this.conn;
                cmd.Parameters.Add(controlParam);
                cmd.CommandText = SQLStatements.FEDRAMPSelectControlRequirement;

                object returnValue = cmd.ExecuteScalar();
                
                if(returnValue == null)
                {
                    cmd.CommandText = SQLStatements.FEDRAMPSaveControlRequirement;
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = SQLStatements.FEDRAMPSelectControlRequirement;
                    returnValue = cmd.ExecuteScalar();
                    requirementid = (int)returnValue;

                }
                else
                {
                    cmd.CommandText = SQLStatements.FEDRAMPSelectControlRequirement;
                    returnValue = cmd.ExecuteScalar();
                    requirementid = (int)returnValue;
                }

                cmd.Parameters.Remove(controlParam);

                MySqlParameter nistIdParam = getSqlParameter(System.Data.DbType.Int32, System.Data.ParameterDirection.Input, "nistid", nistId);
                MySqlParameter requirementIdParam = getSqlParameter(System.Data.DbType.Int32, System.Data.ParameterDirection.Input, "requirementid", requirementid);
                MySqlParameter fedrampIdParam = getSqlParameter(System.Data.DbType.Int32, System.Data.ParameterDirection.Input, "fedrampid", fedrampId);
                cmd.Parameters.Add(nistIdParam);
                cmd.Parameters.Add(requirementIdParam);
                cmd.Parameters.Add(fedrampIdParam);
                cmd.CommandText = SQLStatements.FEDRAMPSelectRequirementXREF;

                returnValue = cmd.ExecuteScalar();

                if(returnValue == null)
                {
                    cmd.CommandText = SQLStatements.FEDRAMPSaveRequirementXREF;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void saveControlParameters(List<ControlParameter> parameters, int fedrampId, int nistId)
        {
            foreach (ControlParameter item in parameters)
            {
                string parameterText = item.ParameterText;
                int parameterid = 0;

                MySqlParameter controlParam = getSqlParameter(System.Data.DbType.String, System.Data.ParameterDirection.Input, "parameter", parameterText);
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = this.conn;
                cmd.Parameters.Add(controlParam);
                cmd.CommandText = SQLStatements.FEDRAMPSelectControlParameter;

                object returnValue = cmd.ExecuteScalar();

                if (returnValue == null)
                {
                    cmd.CommandText = SQLStatements.FEDRAMPSaveControlParameter;
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = SQLStatements.FEDRAMPSelectControlParameter;
                    returnValue = cmd.ExecuteScalar();
                    parameterid = (int)returnValue;

                }
                else
                {
                    cmd.CommandText = SQLStatements.FEDRAMPSelectControlParameter;
                    returnValue = cmd.ExecuteScalar();
                    parameterid = (int)returnValue;
                }

                cmd.Parameters.Remove(controlParam);

                MySqlParameter nistIdParam = getSqlParameter(System.Data.DbType.Int32, System.Data.ParameterDirection.Input, "nistid", nistId);
                MySqlParameter requirementIdParam = getSqlParameter(System.Data.DbType.Int32, System.Data.ParameterDirection.Input, "parameterid", parameterid);
                MySqlParameter fedrampIdParam = getSqlParameter(System.Data.DbType.Int32, System.Data.ParameterDirection.Input, "fedrampid", fedrampId);
                cmd.Parameters.Add(nistIdParam);
                cmd.Parameters.Add(requirementIdParam);
                cmd.Parameters.Add(fedrampIdParam);
                cmd.CommandText = SQLStatements.FEDRAMPSelectParameterXREF;

                returnValue = cmd.ExecuteScalar();

                if (returnValue == null)
                {
                    cmd.CommandText = SQLStatements.FEDRAMPSaveParameterXREF;
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
