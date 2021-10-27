using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using System.IO;
using System.Xml;
using System.IO.Compression;

namespace Frameworks_Mapper
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Saving NIST and CCM Controls");
            Console.WriteLine("----------------------------");
            Console.WriteLine("");
            Console.WriteLine("Saving NIST Rev 3");
            Console.WriteLine("");
            SaveNISTControlsR3(@"C:\Users\mark_\source\repos\frameworks\ControlsContent\800-53-controls.csv");
            Console.WriteLine("");

            Console.WriteLine("");
            Console.WriteLine("Saving NIST Rev 4");
            Console.WriteLine("");
            SaveNISTControlsR4(@"C:\Users\mark_\source\repos\frameworks\ControlsContent\800-53-r4-controls.xml");
            Console.WriteLine("");

            Console.WriteLine("Saving CCM Version 3");
            Console.WriteLine("");
            SaveCCMControls(@"C:\Users\mark_\source\repos\frameworks\ControlsContent\cloud-controls-matrix-v3.0.1-080319.csv");

            Console.WriteLine("");
            Console.WriteLine("Saving FedRAMP Baseline Low");
            Console.WriteLine("");
            SaveFEDRAMPControls(@"C:\Users\mark_\source\repos\frameworks\ControlsContent\FedRAMP_Security_Controls_Baseline_Low.csv", "Low");

            Console.WriteLine("");
            Console.WriteLine("Saving FedRAMP Baseline Moderate");
            Console.WriteLine("");
            SaveFEDRAMPControls(@"C:\Users\mark_\source\repos\frameworks\ControlsContent\FedRAMP_Security_Controls_Baseline_Moderate.csv", "Moderate");

            Console.WriteLine("");
            Console.WriteLine("Saving FedRAMP Baseline High");
            Console.WriteLine("");
            SaveFEDRAMPControls(@"C:\Users\mark_\source\repos\frameworks\ControlsContent\FedRAMP_Security_Controls_Baseline_High.csv", "High");

            Console.WriteLine("");
            Console.WriteLine("Saving CCI List");
            Console.WriteLine("");
            SaveCCIList(@"C:\Users\mark_\source\repos\frameworks\ControlsContent\U_CCI_List.xml");

            Console.WriteLine("");
            Console.WriteLine("Saving NIST Cyber Risk Framework");
            Console.WriteLine("");
            SaveNISTCyberSecurity(@"C:\Users\mark_\source\repos\frameworks\ControlsContent\2018-04-16_framework_v1.1_core1.csv");

            Console.WriteLine("");
            Console.WriteLine("Saving NIST Rev 5");
            Console.WriteLine("");
            SaveNISTControlsR5(@"C:\Users\mark_\OneDrive\repos\frameworks\ControlsContent\sp800-53r5-control-catalog.csv");
            Console.WriteLine("");

            Console.WriteLine("");
            Console.WriteLine("Saving NIST CRF to NIST 800-53 Rev 5 Mapping");
            Console.WriteLine("");
            SaveNISTCRFToNIST80053Rev5Mapping(@"C:\Users\mark_\OneDrive\repos\frameworks\ControlsContent\csf-to-sp800-53r5-mappings.csv");
            Console.WriteLine("");

            Console.WriteLine("");
            Console.WriteLine("Saving Risk Management Framework");
            Console.WriteLine("");
            SaveRMF(@"C:\Users\mark_\OneDrive\repos\frameworks\ControlsContent\rmf_mapping.csv");

            unzipSTIGS(@"E:\temp\U_SRG-STIG_Library_2021_01v2\", @"E:\temp\U_SRG-STIG_Library_2021_01v2\unzipped");
            Console.WriteLine("Done!\nPress any key to exit.");
            Console.ReadLine();
        }

        static void unzipSTIGS(string stigZIPFileDir, string stigExtractToDir)
        {
            string[] zipFiles = Directory.GetFiles(stigZIPFileDir, "*.zip", SearchOption.TopDirectoryOnly);

            foreach(string zipFile in zipFiles)
            {
                Console.WriteLine("Extracting STIG File {0:S}.", zipFile);
                try
                {
                    ZipFile.ExtractToDirectory(zipFile, stigExtractToDir);
                }
                catch(Exception errorInfo)
                {
                    Console.WriteLine("Error with file {0:S} - {1:S}.", zipFile, errorInfo.Message);
                }
            }

            string[] xmlFiles = Directory.GetFiles(stigExtractToDir, "*.xml", SearchOption.AllDirectories);

            

            foreach(string xmlFile in xmlFiles)
            {
                Console.WriteLine("Saving STIG File {0:S}.", xmlFile);
                
                SaveSTIGs(xmlFile);
            }
        }

        static void SaveNISTControlsR3(string csvFileName)
        {
            StreamReader nistControlFile = File.OpenText(csvFileName);
            CsvReader nistControlReader = new CsvReader(nistControlFile, System.Globalization.CultureInfo.CurrentCulture);
            NISTControls nistControlList = new NISTControls();

            string nistclass = "";
            string nistnumber = "";
            string nistfamily = "";
            string nisttitle = "";
            string nistpriority = "";
            string nistdescription = "";
            string nistimpact = "";
            string nistguidance = "";
            
            int counter = 0;

            while (nistControlReader.Read())
            {
                if (counter > 0)
                {
                    NISTControl newControl = new NISTControl();

                    nistclass = nistControlReader.GetField(0);
                    nistfamily = nistControlReader.GetField(1);
                    nistnumber = nistControlReader.GetField(2);
                    nisttitle = nistControlReader.GetField(4);
                    nistimpact = nistControlReader.GetField(5);
                    nistpriority = nistControlReader.GetField(6);
                    nistdescription = nistControlReader.GetField(7);
                    nistguidance = nistControlReader.GetField(8);

                    newControl.NISTClassName = nistclass;
                    newControl.NISTFamilyName = nistfamily;
                    newControl.NISTNumber = nistnumber;
                    newControl.NISTTitle = nisttitle;
                    newControl.NISTImpact = nistimpact;
                    newControl.NISTPriority = nistpriority;
                    newControl.NISTDescription = nistdescription;
                    newControl.NISTGuidance = nistguidance;
                    newControl.NISTRevisionNumber = 3;

                    nistControlList.AddNISTControl(newControl);

                }

                counter++;

            }

            nistControlList.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"]; // "SERVER = localhost; DATABASE = frameworks; UID = mwireman; PASSWORD = shinobu5";
            nistControlList.SaveNISTControls();
        }

        static void SaveNISTControlsR4(string xmlFileName)
        {
            NISTControls nistControlsList = new NISTControls();

            XmlDocument nistR4Xml = new XmlDocument();
            nistR4Xml.Load(xmlFileName);
            NameTable nt = new NameTable();
            
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(nistR4Xml.NameTable);
            nsmgr.AddNamespace(String.Empty, "http://scap.nist.gov/schema/sp800-53/feed/2.0/sp800-53-feed_2.0.xsd");
            nsmgr.AddNamespace("controls", "http://scap.nist.gov/schema/sp800-53/feed/2.0");
            
            XmlNodeList controls = nistR4Xml.SelectNodes("//controls:control",nsmgr);

            foreach(XmlNode item in controls)
            {
                NISTControl parentControl = new NISTControl();

                if (item["withdrawn"] == null)
                {
                    string family = getElementValue(item["family"]); //.InnerText;
                    string number = getElementValue(item["number"]); //.InnerText;
                    string title = getElementValue(item["title"]); //.InnerText;
                    string priority = getElementValue(item["priority"]); //.InnerText;
                    string guidance = getElementValue(item["supplemental-guidance"]["description"]); //.InnerText;
                    parentControl.NISTRevisionNumber = 4;

                    string impact = "";

                    foreach (XmlNode impactNode in item)
                    {
                        if (impactNode.Name == "baseline-impact")
                        {
                            impact = impact + "," + impactNode.InnerText;
                        }
                    }

                    if (impact != "")
                    {
                        impact = impact.Substring(1);
                    }

                    parentControl.NISTFamilyName = family;
                    parentControl.NISTNumber = number;
                    parentControl.NISTTitle = title;
                    parentControl.NISTPriority = priority;
                    parentControl.NISTImpact = impact;
                    parentControl.NISTGuidance = guidance;

                    nistControlsList.AddNISTControl(parentControl);

                    XmlNodeList statements = item["statement"].GetElementsByTagName("statement");

                    foreach (XmlNode childNode in statements)
                    {
                        NISTControl childControl = new NISTControl();

                        number = getElementValue(childNode["number"]); //.InnerText;
                        string descr = getElementValue(childNode["description"]); //.InnerText;

                        childControl.NISTFamilyName = family;
                        childControl.NISTNumber = number;
                        childControl.NISTTitle = title;
                        childControl.NISTPriority = priority;
                        childControl.NISTImpact = impact;
                        childControl.NISTGuidance = guidance;
                        childControl.NISTDescription = descr;
                        childControl.NISTRevisionNumber = 4;

                        nistControlsList.AddNISTControl(childControl);
                    }
                }
                
            }

            nistControlsList.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"]; //"SERVER = localhost; DATABASE = frameworks; UID = mwireman; PASSWORD = shinobu5";
            nistControlsList.SaveNISTControls();
        }

        static void SaveNISTControlsR5(string csvFileName)
        {
            StreamReader nistControlFile = File.OpenText(csvFileName);
            CsvReader nistControlReader = new CsvReader(nistControlFile, System.Globalization.CultureInfo.CurrentCulture);
            NISTControls nistControlList = new NISTControls();

            string nistclass = "";
            string nistnumber = "";
            string nistfamily = "";
            string nisttitle = "";
            string nistpriority = "";
            string nistdescription = "";
            string nistimpact = "";
            string nistguidance = "";
            
            int counter = 0;

            while(nistControlReader.Read())
            {
                if (counter > 0)
                {
                    NISTControl control = new NISTControl();
                    nistnumber = nistControlReader.GetField(0).Trim();
                    nistfamily = nistControlReader.GetField(1).Trim();
                    nistdescription = nistControlReader.GetField(2).Trim();
                    nistguidance = nistControlReader.GetField(3).Trim();

                    control.NISTNumber = nistnumber;
                    control.NISTFamilyName = nistfamily;
                    control.NISTDescription = nistdescription;
                    control.NISTGuidance = nistguidance;
                    control.NISTRevisionNumber = 5;

                    nistControlList.AddNISTControl(control);

                    
                }

                counter++;
            }

            nistControlList.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"]; //"SERVER = localhost; DATABASE = frameworks; UID = mwireman; PASSWORD = shinobu5";
            nistControlList.SaveNISTControls();

        }

        static void SaveNISTCRFToNIST80053Rev5Mapping(string csvFileName)
        {
            StreamReader nistControlFile = File.OpenText(csvFileName);
            CsvReader nistControlReader = new CsvReader(nistControlFile, System.Globalization.CultureInfo.CurrentCulture);
            NISTCyberSecItems nistCyberList = new NISTCyberSecItems();

            while (nistControlReader.Read())
            {
                NISTCyberSec cyberSecControl = new NISTCyberSec();

                string[] splitSubCatVal = nistControlReader.GetField(2).Split(':');
                
                cyberSecControl.SubCategoryID = splitSubCatVal[0].Trim();
                
                string[] splitNISTControlVal = nistControlReader.GetField(3).Split(',');

                foreach(string val in splitNISTControlVal)
                {
                    cyberSecControl.AddNIST80053Rev5Control(val.Trim());
                }

                nistCyberList.AddNISTCyberSecurityItem(cyberSecControl);
            }

            nistCyberList.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"]; //"SERVER = localhost; DATABASE = frameworks; UID = mwireman; PASSWORD = shinobu5";
            nistCyberList.SaveNISTCyberNIST80053Mapping(5);
        }

        static void SaveCCMControls(string csvFileName)
        {
            StreamReader ccmControlFile = File.OpenText(csvFileName);
            CsvReader ccmControlReader = new CsvReader(ccmControlFile, System.Globalization.CultureInfo.CurrentCulture);
            CCMControls ccmControlList = new CCMControls();

            string ccmDomain = "";
            string ccmID = "";
            string ccmSpecification = "";
            string nistControls = "";

            int counter = 0;

            while (ccmControlReader.Read())
            {
                if (counter > 0)
                {
                    CCMControl currentControl = new CCMControl();
                    ccmDomain = ccmControlReader.GetField(0);
                    ccmID = ccmControlReader.GetField(1);
                    ccmSpecification = ccmControlReader.GetField(2);
                    nistControls = ccmControlReader.GetField(47);

                    currentControl.CCMDomain = ccmDomain;
                    currentControl.CCMNumber = ccmID;
                    currentControl.CCMSpecification = ccmSpecification;

                    string[] nistControlsList = nistControls.Split('\n');

                    foreach (string control in nistControlsList)
                    {
                        NISTControl mappedNISTControl = new NISTControl();
                        mappedNISTControl.NISTNumber = control;
                        mappedNISTControl.NISTRevisionNumber = 3;
                        currentControl.AddNISTControl(mappedNISTControl);
                    }

                    ccmControlList.AddCCMControl(currentControl);
                }

                counter++;
            }

            ccmControlList.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"]; // "SERVER = localhost; DATABASE = frameworks; UID = mwireman; PASSWORD = shinobu5";
            ccmControlList.SaveCCMControls();
        }


        static void SaveFEDRAMPControls(string csvFileName, string fedRampLevel)
        {
            StreamReader fedrampControlFile = File.OpenText(csvFileName);
            CsvReader fedrampControlReader = new CsvReader(fedrampControlFile, System.Globalization.CultureInfo.CurrentCulture);
            FEDRAMPControls fedrampControlList = new FEDRAMPControls();
            
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

                    string[] descrsplitval = { "The organization"};
                    string[] descrval = fedrampControlReader.GetField(5).Split(descrsplitval, StringSplitOptions.RemoveEmptyEntries);

                    if (descrval.Length > 0)
                    {
                        string[] reqsplitval = { "Supplemental Guidance" };
                        string[] requirementval = descrval[0].Split(reqsplitval, StringSplitOptions.RemoveEmptyEntries);

                        currentControl.FEDRAMPDescription = requirementval[0].Replace(':',' ').Trim();

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
                    
                    if(fedrampControlReader.GetField(6) != null && fedrampControlReader.GetField(6) != "")
                    {
                        currentControl.AddControlParameters(getControlParameters(fedrampControlReader.GetField(6), currentControl.NISTControlID)); 
                    }

                    if(fedrampControlReader.GetField(7) != null && fedrampControlReader.GetField(7) != "")
                    {
                        string rowVal = fedrampControlReader.GetField(7);

                        if (rowVal != "" && rowVal != " ")
                        {
                            currentControl.AddControlRequirements(getControlRequirements(rowVal, currentControl.NISTControlID));
                        }
                    }

                    fedrampControlList.AddFEDRAMPControl(currentControl);
                }

                counter++;

            }

            fedrampControlList.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"]; // "SERVER = localhost; DATABASE = frameworks; UID = mwireman; PASSWORD = shinobu5";
            fedrampControlList.SaveFEDRAMPControls();
        }

        static List<ControlRequirement> getControlRequirements(string rowVal, string nistControlId)
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

        static List<ControlParameter> getControlParameters(string rowVal, string nistControlId)
        {
            List<ControlParameter> retVal = new List<ControlParameter>();
            char[] splitVals = { '[', ']'};

            string[] selectionVals = rowVal.Split(splitVals, StringSplitOptions.RemoveEmptyEntries);

            if (selectionVals.Length > 1)
            {
                if (selectionVals.Length > 2)
                {
                    for (int i = 1; i < selectionVals.Length; i = i + 2)
                    {
                        ControlParameter cp = new ControlParameter();
                        cp.ParameterText = selectionVals[i];
                        //char[] splitNistID = { '(', ')'};
                        //string[] nistidParsing = selectionVals[i - 1].Trim().Split(splitNistID, StringSplitOptions.RemoveEmptyEntries);

                        //if (nistidParsing.Length > 0)
                        //{
                        //    string nistidval = nistidParsing[0].Trim();

                        //    for(int m = 1; m < nistidParsing.Length; m++)
                        //    {
                        //        if(nistidParsing[m] != "" && nistidParsing[m] != " " && !nistidParsing[m].Contains("-"))
                        //        {
                        //            nistidval += nistidParsing[m].Trim() + "."; 
                        //        }
                        //    }

                        //    cp.NISTControlID = nistidval;
                        //}
                        //else
                        //{
                        //    cp.NISTControlID = selectionVals[i - 1].Trim();
                        //}
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


        static void SaveCCIList(string xmlFile)
        {
            XmlDocument cciXmlDoc = new XmlDocument();
            cciXmlDoc.Load(xmlFile);

            NameTable nt = new NameTable();

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(cciXmlDoc.NameTable);
            nsmgr.AddNamespace(String.Empty, "http://iase.disa.mil/cci/cci.xsd");
            nsmgr.AddNamespace("ns", "http://iase.disa.mil/cci");
            XmlNodeList cciItemNodes = cciXmlDoc.SelectNodes("//ns:cci_item",nsmgr);

            CCILists cciCollection = new CCILists();
            cciCollection.NISTRevisionNumber = 4;

            string cciid = "";
            string ccidefinition = "";
            string ccitype = "";
            
            foreach(XmlNode cceItemnode in cciItemNodes)
            {
                CCIList newCCI = new CCIList();
                
                cciid = cceItemnode.Attributes["id"].Value;
                ccidefinition = cceItemnode.SelectSingleNode("ns:definition",nsmgr).InnerText;
                ccitype = cceItemnode.SelectSingleNode("ns:type",nsmgr).InnerText;

                newCCI.CCIIDNo = cciid;
                newCCI.CCIDescription = ccidefinition;
                newCCI.CCIType = ccitype;

                XmlNodeList referenceNodes = cceItemnode.SelectNodes("ns:references/ns:reference",nsmgr);

                foreach(XmlNode referenceNode in referenceNodes)
                {
                    if(referenceNode.Attributes["creator"].Value == "NIST" && referenceNode.Attributes["title"].Value == "NIST SP 800-53 Revision 4" && referenceNode.Attributes["version"].Value == "4")
                    {
                        string nistControl = referenceNode.Attributes["index"].Value.Trim();
                        string[] nistvalsplit = nistControl.Split(' ');
                        string[] nistparamsplit = nistControl.Split('(');
                        string sVal = "";

                       
                        if (nistControl.Contains('(') && nistControl.Contains(')'))
                        {
                            sVal = nistparamsplit[0];
                        }
                        else
                        {
                            if (nistvalsplit.Length > 2)
                            {
                                sVal = nistvalsplit[0] + nistvalsplit[1] + ".";

                                for (int i = 2; i < nistvalsplit.Length; i++)
                                {
                                    sVal = sVal + nistvalsplit[i] + ".";
                                }
                            }
                            else if (nistvalsplit.Length == 2)
                            {
                                sVal = nistvalsplit[0] + nistvalsplit[1] + ".";
                            }
                            else if (nistvalsplit.Length == 1)
                            {
                                sVal = nistControl;
                            }
                        }

                        //nistControls.Add(sVal);
                        newCCI.AddNISTReference(sVal);

                    }

                    cciCollection.AddCCEList(newCCI);
                }
            }

            cciCollection.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"]; //"SERVER = localhost; DATABASE = frameworks; UID = mwireman; PASSWORD = shinobu5";
            cciCollection.SaveCCIListItems();
        }

        static void SaveSTIGs(string xmlFile)
        {
            XmlDocument stigXmlDoc = new XmlDocument();
            stigXmlDoc.Load(xmlFile);
            string targetSystem = "";

            STIGS stigCollection = new STIGS();
            
            NameTable nt = new NameTable();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(stigXmlDoc.NameTable);
            nsmgr.AddNamespace(String.Empty, "http://nvd.nist.gov/schema/xccdf-1.1.4.xsd");
            nsmgr.AddNamespace("ns", "http://checklists.nist.gov/xccdf/1.1");

            if (stigXmlDoc.SelectSingleNode("ns:Benchmark/ns:title", nsmgr) != null)
            {
                targetSystem = stigXmlDoc.SelectSingleNode("ns:Benchmark/ns:title", nsmgr).InnerText;
            }
                
            XmlNodeList stigItemNodes = stigXmlDoc.SelectNodes("//ns:Group", nsmgr);
            
            Console.WriteLine("Saving STIG Target {0:S}", targetSystem);

            foreach (XmlNode stigNode in stigItemNodes)
            {
                STIG currentSTIG = new STIG();

                List<string> cciReferences = new List<string>();

                string groupId = stigNode.Attributes["id"].Value;
                string title = stigNode.SelectSingleNode("ns:title", nsmgr).InnerText;
                string groupDescr = "";

                Console.WriteLine("\tParsing STIG {0:S}", groupId);

                currentSTIG.GroupIDNumber = groupId;
                currentSTIG.Title = title;
                currentSTIG.TargetSystem = targetSystem;
                
                XmlDocument groupDescrXml = new XmlDocument();
                groupDescrXml.LoadXml(stigNode.SelectSingleNode("ns:description",nsmgr).InnerText);
                if(groupDescrXml.InnerText != "")
                {
                    groupDescr = groupDescrXml.InnerText;
                }

                currentSTIG.Description = groupDescr;

                XmlNodeList ruleNodes = stigNode.SelectNodes("ns:Rule", nsmgr);

                foreach (XmlNode ruleNode in ruleNodes)
                {
                    STIGRule currentRule = new STIGRule();
                    string severity = "";

                    string ruleId = ruleNode.Attributes["id"].Value;
                    if (ruleNode.Attributes["severity"] != null)
                    {
                        severity = ruleNode.Attributes["severity"].Value;
                    }
                    string weight = ruleNode.Attributes["weight"].Value;
                    string version = ruleNode.SelectSingleNode("ns:version", nsmgr).InnerText;
                    string ruleDescr = "";
                    
                    string ruleDescrNodeVal = ruleNode.SelectSingleNode("ns:description", nsmgr).InnerText;
                    string[] ruleDescrParser = { "<VulnDiscussion>", "</VulnDiscussion>" };
                    string[] ruleDescrParsed = ruleDescrNodeVal.Split(ruleDescrParser, StringSplitOptions.RemoveEmptyEntries);
                    
                    if (ruleDescrParsed.Length > 1)
                    {
                        ruleDescr = ruleDescrParsed[0];
                    }

                    currentRule.RuleIDNumber = ruleId;
                    currentRule.Severity = severity;
                    currentRule.Weight = weight;
                    currentRule.Version = version;
                    currentRule.Description = ruleDescr;

                    XmlNodeList cciRefNodes = ruleNode.SelectNodes("ns:ident", nsmgr);

                    foreach(XmlNode cciRefNode in cciRefNodes)
                    {
                        //cciReferences.Add(cciRefNode.InnerText);
                        if (cciRefNode.InnerText.Contains("CCI"))
                        {
                            currentSTIG.AddCCIReference(cciRefNode.InnerText);
                        }
                    }

                    STIGFix fixDetail = new STIGFix();

                    XmlNode fixNode = ruleNode.SelectSingleNode("ns:fixtext", nsmgr);

                    string fixText = fixNode.InnerText;

                    string[] parseFixText = { "via UI:", "via CLI:" };
                    string[] parsedFixText = fixText.Split(parseFixText, StringSplitOptions.RemoveEmptyEntries);

                    string uiFixSteps = "";
                    string cliFixSteps = "";
                    string fixDescr = "";
                    string fixNo = fixNode.Attributes["fixref"].Value;

                    if (parsedFixText.Length > 2)
                    {
                        fixDescr = parsedFixText[0];
                        uiFixSteps = parsedFixText[parsedFixText.Length - 2];
                        cliFixSteps = parsedFixText[parsedFixText.Length - 1];
                    }
                    else
                    {
                        fixDescr = parsedFixText[0];
                    }

                    fixDetail.UIFixSteps = uiFixSteps;
                    fixDetail.CLIFixSteps = cliFixSteps;
                    fixDetail.Description = fixDescr;
                    fixDetail.FixIDNumber = fixNo;

                    currentRule.FixDetails = fixDetail;

                    STIGCheck checkDetail = new STIGCheck();

                    XmlNode checkNode = ruleNode.SelectSingleNode("ns:check/ns:check-content", nsmgr);

                    string[] parseCheckText = { "via UI:", "via CLI:" };
                    string[] parsedCheckText = fixText.Split(parseCheckText, StringSplitOptions.RemoveEmptyEntries);

                    string uiCheckSteps = "";
                    string cliCheckSteps = "";
                    string checkDescr = "";
                    string checkSystemNo = ruleNode.SelectSingleNode("ns:check", nsmgr).Attributes["system"].Value;

                    if (parsedCheckText.Length > 2)
                    {
                        checkDescr = parsedFixText[0];
                        uiCheckSteps = parsedFixText[parsedFixText.Length - 2];
                        cliCheckSteps = parsedFixText[parsedFixText.Length - 1];
                    }
                    else
                    {
                        checkDescr = parsedFixText[0];
                    }

                    checkDetail.CheckSystemNumber = checkSystemNo;
                    checkDetail.CLICheckSteps = cliCheckSteps;
                    checkDetail.UICheckSteps = uiCheckSteps;
                    checkDetail.Description = checkDescr;

                    currentRule.CheckDetails = checkDetail;

                    currentSTIG.AddRule(currentRule);

                }

                stigCollection.AddSTIG(currentSTIG);
            }

            stigCollection.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"]; //"SERVER = localhost; DATABASE = frameworks; UID = mwireman; PASSWORD = shinobu5";
            stigCollection.SaveSTIGS();
            int counter = stigItemNodes.Count;
        }


        static void SaveNISTCyberSecurity(string csvFileName)
        {
            StreamReader nistCyberFile = File.OpenText(csvFileName);
            CsvReader nistCyberReader = new CsvReader(nistCyberFile, System.Globalization.CultureInfo.CurrentCulture);
            
            string functionName = "";
            string categoryName = "";
            string categoryNameId = "";
            string categoryNameDescr = "";
            string subCatName = "";
            string subCatId = "";
            string subCatDescr = "";

            List<string> nistReferences = new List<string>();

            NISTCyberSecItems ncsi = new NISTCyberSecItems();

            int counter = 0;

            while (nistCyberReader.Read())
            {
                NISTCyberSec ncs = new NISTCyberSec();

                if (counter > 0)
                {
                    if (nistCyberReader.GetField(0).Trim() != String.Empty)
                    {
                        functionName = nistCyberReader.GetField(0).Trim();
                    }

                    if (nistCyberReader.GetField(1).Trim() != String.Empty)
                    {
                        categoryName = nistCyberReader.GetField(1).Trim();
                        char[] spltCatVals = { '(', ')', ':' };
                        string[] categoryNameSplit = categoryName.Split(spltCatVals, StringSplitOptions.RemoveEmptyEntries);

                        categoryName = categoryNameSplit[0];
                        categoryNameId = categoryNameSplit[1];
                        categoryNameDescr = categoryNameSplit[2];

                        int index = categoryNameSplit.Length;

                    }

                    if (nistCyberReader.GetField(2).Trim() != String.Empty)
                    {
                        subCatName = nistCyberReader.GetField(2).Trim();
                        char[] splitSubCatVals = { ':' };
                        string[] subCatNameSplit = subCatName.Split(splitSubCatVals);

                        subCatId = subCatNameSplit[0];
                        subCatDescr = subCatNameSplit[1];

                    }
                    else
                    {
                        if (nistCyberReader.GetField(3).Trim().Contains("NIST"))
                        {
                            Console.WriteLine("Parsing NIST Cyber Security {0:S}", subCatName);


                            string[] nistParser = { "NIST SP 800-53 Rev. 4", "," };
                            string[] parsedResult = nistCyberReader.GetField(3).Trim().Split(nistParser, StringSplitOptions.RemoveEmptyEntries);

                            ncs.Function = functionName;
                            ncs.Category = categoryName;
                            ncs.CategoryID = categoryNameId;
                            ncs.CategoryDescription = categoryNameDescr;
                            ncs.SubCategoryID = subCatId;
                            ncs.SubCategoryDescription = subCatDescr;

                            for (int i = 1; i < parsedResult.Length; i++)
                            {
                                //nistReferences.Add(parsedResult[i]);
                                ncs.AddNIST80053Rev4Control(parsedResult[i].Trim());
                            }

                            ncsi.AddNISTCyberSecurityItem(ncs);
                           
                        }
                    }
                }

                counter++;
            }

            ncsi.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"]; //"SERVER = localhost; DATABASE = frameworks; UID = mwireman; PASSWORD = shinobu5";
            ncsi.SaveNISTCyberItems();

        }

        static void SaveRMF(string csvFileName)
        {
            StreamReader rmfFile = File.OpenText(csvFileName);
            CsvReader rmfReader = new CsvReader(rmfFile, System.Globalization.CultureInfo.CurrentCulture);
            RMFItems rmfController = new RMFItems();

            string step = "";
            string taskid = "";
            string taskName = "";
            string taskDescr = "";

            int counter = 0;
                        
            while (rmfReader.Read())
            {
                if(counter > 0)
                {
                    RMF rmfItem = new RMF();

                    if(rmfReader.GetField(0) != String.Empty)
                    {
                        step = rmfReader.GetField(0).Trim();
                    }

                    if (rmfReader.GetField(1) != String.Empty)
                    {
                        taskid = rmfReader.GetField(1).Trim();
                    }

                    if (rmfReader.GetField(2) != String.Empty)
                    {
                        taskName = rmfReader.GetField(2).Trim();
                        
                    }

                    taskDescr = rmfReader.GetField(3).Trim();

                    if (rmfReader.GetField(4) != String.Empty)
                    {
                        List<NISTCyberSec> cyberItems = new List<NISTCyberSec>();

                        string[] splitVals = { "[Cybersecurity Framework:", ";", "]" };
                        string[] splitResult = rmfReader.GetField(4).Trim().Split(splitVals, StringSplitOptions.RemoveEmptyEntries);
                        
                        foreach (string cyberId in splitResult)
                        {
                            string[] splitSubResult = cyberId.Split('-');
                            NISTCyberSec cyberItem = new NISTCyberSec();
                            if(splitSubResult.Length > 1)
                            {
                                cyberItem.SubCategoryID = cyberId.Trim();
                            }
                            else
                            {
                                cyberItem.CategoryID = cyberId.Trim();
                            }
                            //cyberItem.CategoryID = splitSubResult[0].Trim();
                            cyberItems.Add(cyberItem);
                        }

                        rmfItem.NISTCyberRiskItems = cyberItems;
                    }

                    rmfItem.Step = step;
                    rmfItem.TaskID = taskid;
                    rmfItem.TaskName = taskName;
                    rmfItem.TaskDescription = taskDescr;

                    rmfController.AddRMFItem(rmfItem);
                    
                }
                counter++;
            }

            
            rmfController.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"]; //"SERVER = localhost; DATABASE = frameworks; UID = mwireman; PASSWORD = shinobu5";
            rmfController.SaveRMFItems();
            

        }

        static string getElementValue(XmlElement element)
        {
            if(element == null)
            {
                return "";
            }
            else
            {
                return element.InnerText;
            }
        }

        static void splitString(string val, string splitVal)
        {
            string[] splitOptions = { splitVal };

            string[] output = val.Split(splitOptions, StringSplitOptions.RemoveEmptyEntries);

        }
    }
}
