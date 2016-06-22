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
                     * User
                     * Organization
                     * Customer
                     * Candidate
                     * HiringActivity (tbJobFlow table in MPLOY)
                     * ContactLog (tbContactLog table in MPLOY)
                     * Skills
                     **/

                    //UserMigration.UserImport();

                    //OrganizationMigration.OrganizationImport();

                    //CustomerMigration.ContactImport();

                    //CandidateMigration.TestUpdate();

                    //RequirementMigration.JobImport();
                    
                    HiringActivityMigration.ImportHiringActivity();

                    //ContactLogMigration.ImportContactLog();

                    //SkillsMigration.SkillsImport();

                    //PersonSkillMigration.PersonSkillsImport();
                    //Adds a record to the PersonTypicalRole table after the PersonSkill table is populated:
                    //PersonTypicalRole.AddPersonTypicalRoleRecord();
                
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
