using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ConsoleApplication1;
using ConsoleApplication1.Helper;
using Microsoft.Practices.EnterpriseLibrary.Logging;


namespace ConsoleApplication1
{
    public class Program
    {        

        static Entities _db;

        //static List<KeyValuePair<int, string>> sourceList = new List<KeyValuePair<int, string>>();

        static void Main(string[] args)
        {
            using (_db = new Entities())
            {
                try
                {
                    /**
                     * The order of the migration:
                     * Run from the top down in the order they are listed
                     **/

                    //UserMigration.UserImport();

                    //OrganizationMigration.OrganizationImport();

                    //CustomerMigration.ContactImport();

                    //CandidateMigration.CandidateImport();

                    //RequirementMigration.JobImport();
                    
                    
                    
                    //Reads from the mploy tbJobFlow table and inserts into the Intersect WorkflowStateLog table for each workflow process:
                    //WorkflowLogMigration.ImportHiringActivity();
                    

                    //SkillsMigration.SkillsImport();

                    //PersonSkillMigration.PersonSkillsImport();
                    
                    //Adds a record to the PersonTypicalRole table after the PersonSkill table is populated:
                    //PersonTypicalRole.AddPersonTypicalRoleRecord();


                    //Inserts into the WorkflowStateLog table for each requirement in the Mploy.tbJob table so each Requirement has a workflow state.
                    //RequirementProcessWorkflow.ImportRequirementProcessWorkflow();

                    //ACTIVITIES MIGRATION:
                    //HiringActivitiesMigration.ImportHiringActivity();

                    //ContactLogMigration.ImportContactLog();

                }
                catch
                (Exception ex)
                {                 
                    new LogWriterFactory().Create().Write(ex.Expand("Error occured with migration process in the Program.cs file."));
                }
            }
            
        }
    }
}
