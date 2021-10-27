using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frameworks_Mapper
{
    public static class SQLStatements
    {
        public static string NISTSelectFamily =  "SELECT id FROM nistfamily WHERE nistfamilyname=@familyname;";
        public static string NISTInsertFamily = "INSERT INTO nistfamily(nistfamilyname) VALUES(@familyname);";
        public static string NISTSelectClass = "SELECT id FROM nistclass WHERE classname=@classname;";
        public static string NISTInsertClass = "INSERT INTO nistclass (classname) VALUES(@classname);";
        public static string NISTSelectControl = "SELECT id FROM nistcontrol WHERE number=@nistcontrol AND revisionnumber=@revision;";
        public static string NISTInsertControl = "INSERT INTO nistcontrol (number,title,impact,priority,descr,guidance,classId,familyId,revisionnumber) VALUES(@number,@title,@impact,@priority,@descr,@guidance,@classid,@familyid,@revision);";
        public static string NISTSelectRevision = "SELECT id from nistrevisions WHERE revisionnumber=@revision;";
        public static string NISTInsertRevision = "INSERT INTO nistrevisions (revisionnumber) VALUES (@revision);";
        public static string NISTUpdateDescription = "UPDATE nistcontrol SET descr = @descr WHERE number=@number;";
        
        public static string CCMSelectDomain = "SELECT id FROM cloudcontroldomains WHERE domainname=@domainname;";
        public static string CCMInsertDomain = "INSERT INTO cloudcontroldomains(domainname) VALUES(@domainname);";
        public static string CCMSelectNISTXREF = "SELECT cloudcontrolid FROM cloudcontrols_nist_xref WHERE cloudcontrolid=@cloudid AND nistcontrolid=@nistid;";
        public static string CCMInserNISTXREF = "INSERT INTO cloudcontrols_nist_xref (cloudcontrolid,nistcontrolid) VALUES(@cloudid,@nistid);";
        public static string CCMSelectControl = "SELECT id FROM cloudcontrols WHERE controlid=@ccmcontrolid;";
        public static string CCMInsertControl = "INSERT INTO cloudcontrols (controlid,specification,domainid) VALUES(@ccmcontrolid,@specification,@domainid);";

        public static string FEDRAMPSelectControl = "SELECT id FROM fedrampcontrols WHERE sortid=@sortid;";
        public static string FEDRAMPInsertControl = "INSERT INTO fedrampcontrols (sortid,fedramplevel,controlname,controldescr,supplementguidance) VALUES (@sortid,@level,@controlname,@controldescr,@guidance);";
        public static string FEDRAMPSelectNISTXREF = "SELECT fedrampid FROM fedramp_nist_xref WHERE fedrampid=@fedrampid AND nistid=@nistid;";
        public static string FEDRAMPSaveNISTXREF = "INSERT INTO fedramp_nist_xref (fedrampid,nistid) VALUES (@fedrampid,@nistid);";
        public static string FEDRAMPSelectControlRequirement = "SELECT id from controlrequirements WHERE requirementext=@requirement;";
        public static string FEDRAMPSaveControlRequirement = "INSERT INTO controlrequirements (requirementext) VALUES (@requirement);";
        public static string FEDRAMPSelectControlParameter = "SELECT id from controlparameters WHERE parametertext=@parameter;";
        public static string FEDRAMPSaveControlParameter = "INSERT INTO controlparameters (parametertext) VALUES (@parameter);";
        public static string FEDRAMPSelectRequirementXREF = "SELECT fedrampid FROM fedramp_controlrequirements_xref WHERE fedrampid=@fedrampid AND requirementid=@requirementid AND nistid=@nistid;";
        public static string FEDRAMPSaveRequirementXREF = "INSERT INTO fedramp_controlrequirements_xref (fedrampid,requirementid,nistid) VALUES (@fedrampid,@requirementid,@nistid);";
        public static string FEDRAMPSelectParameterXREF = "SELECT fedrampid FROM fedramp_controlparameters_xref WHERE fedrampid=@fedrampid AND controlparameterid=@parameterid AND nistid=@nistid;";
        public static string FEDRAMPSaveParameterXREF = "INSERT INTO fedramp_controlparameters_xref (fedrampid,controlparameterid,nistid) VALUES (@fedrampid,@parameterid,@nistid);";

        public static string CCISelectItem = "SELECT id from ccis WHERE cciidno=@cciid;";
        public static string CCIInsertItem = "INSERT INTO ccis (cciidno,definition,type) VALUES (@cciid,@definition,@type);";
        public static string CCISelectNISTXREF = "SELECT cciid FROM ccis_nistcontrols_xref WHERE cciid=@cciid AND nistcontrolid=@nistid;";
        public static string CCIInsertNISTXREF = "INSERT INTO ccis_nistcontrols_xref (cciid,nistcontrolid) VALUES (@cciid,@nistid);";

        public static string STIGSelectItem = "SELECT id FROM stigs WHERE groupidno=@groupno;";
        public static string STIGInsertItem = "INSERT INTO stigs (groupidno,descr,title,target) VALUES (@groupno,@descr,@title,@target);";
        public static string STIGRuleSelect = "SELECT id from stig_rules WHERE ruleidno=@ruleid;";
        public static string STIGRuleInsert = "INSERT INTO stig_rules (ruleidno,severity,weight,version,descr) VALUES (@ruleid,@severity,@weight,@version,@descr);";
        
        public static string STIGRuleFixSelect = "SELECT id FROM stig_rules_fixes WHERE fixidno=@fixid;";
        public static string STIGRuleFixInsert = "INSERT INTO stig_rules_fixes (uifixtext,clifixtext,descr,fixidno) VALUES (@uifix,@clifix,@descr,@fixid);";
        
        public static string STIGRuleCheckSelect = "SELECT id FROM stig_rules_checks WHERE checksystemno=@systemno;";
        public static string STIGRuleCheckInsert = "INSERT INTO stig_rules_checks (uichecktext,clichecktext,descr,checksystemno) VALUES (@uicheck,@clicheck,@descr,@systemno);";
        
        public static string STIGCCISelectXREF = "SELECT stigid FROM stigs_ccis_xref WHERE stigid=@stigid AND cciid=@cciid;";
        public static string STIGCCIInsertXREF = "INSERT INTO stigs_ccis_xref (stigid,cciid) VALUES (@stigid,@cciid);";
        
        public static string STIGRuleSelectXREF = "SELECT stigid FROM stigs_rules_xref WHERE stigid=@stigid AND ruleid=@ruleid;";
        public static string STIGRuleInsertXREF = "INSERT INTO stigs_rules_xref (stigid,ruleid) VALUES (@stigid,@ruleid);";
        
        public static string STIGRuleCheckSelectXREF = "SELECT ruleid FROM stig_rules_check_xref WHERE ruleid=@ruleid AND checkid=@checkid;";
        public static string STIGRuleCheckInsertXREF = "INSERT INTO stig_rules_check_xref (ruleid,checkid) VALUES (@ruleid,@checkid);";
        
        public static string STIGRuleFixSelectXREF = "SELECT ruleid FROM stig_rules_fix_xref WHERE ruleid=@ruleid AND fixid=@fixid;";
        public static string STIGRuleFixInsertXREF = "INSERT INTO stig_rules_fix_xref (ruleid,fixid) VALUES (@ruleid,@fixid);";

        public static string NISTCyberSelect = "SELECT id FROM nistcybersecurity WHERE subcategoryid=@subcatid;";
        public static string NISTCyberInsert = "INSERT INTO nistcybersecurity (functionname,categoryname,categorynameid,categorynamedescr,subcategoryid,subcategorydescr) VALUES (@functionname,@catname,@catnameid,@catnamedescr,@subcatid,@subcatdescr);";
        public static string NISTCyberSelectByCategoryID = "SELECT id FROM nistcybersecurity WHERE categorynameid=@catid;";
        public static string NISTCyberNISTControlsSelectXREF = "SELECT nistcyberid FROM nistcybersecurity_nistcontrols_xref WHERE nistcyberid=@cyberid AND nistcontrolid=@nistid;";
        public static string NISTCyberNISTControlsInsertXREF = "INSERT INTO nistcybersecurity_nistcontrols_xref (nistcyberid,nistcontrolid) VALUES (@cyberid,@nistid);";

        public static string RMFSelect = "SELECT id from rmf WHERE taskid=@taskid;";
        public static string RMFSelectWithTaskDescr = "SELECT id from rmf WHERE taskid=@taskid and taskdescr=@taskdescr;";
        public static string RMFInsert = "INSERT INTO rmf (step,taskid,taskname,taskdescr) VALUES (@step,@taskid,@taskname,@descr);";
        
        public static string RMFNISTCyberSelectXREF = "SELECT rmfid FROM rmf_nistcybersecurity_xref WHERE rmfid=@rmfid AND nistcyberid=@cyberid;";
        public static string RMFNISTCyberInsertXREF = "INSERT INTO rmf_nistcybersecurity_xref (rmfid,nistcyberid) VALUES (@rmfid,@cyberid);";

    }
}
