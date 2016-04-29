using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Diagnostics;
using System.Linq;
using System.Xml.Schema;
using ConsoleApplication1;
using ConsoleApplication1.Helper;
using Microsoft.Practices.EnterpriseLibrary.Common.Utility;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace ConsoleApplication1
{
    class PersonTypicalRole
    {

        static Entities _db;

        public static void AddPersonTypicalRoleRecord()
        {
            using (_db = new Entities())
            {
                try
                {
                    //Read in the a list from Intersect that contains data from the SkillCategoryMapping, PersonSkill, and SkillCategory tables:
                    var contactSkillData = _db.ReadIntersectSkillCategoryMapping();

                    foreach (var contactSkillRecord in contactSkillData)
                    {
                       
                        //Add a record to the PersonTypicalRole table:
                        //Based on the SkillCategory (only for Ids 1-4) for each skill associated to the current person
                        //Add a record to the PersonTypicalRole table (only once per role per person)
                        //A person can have multiple typical roles, but only one record per role


                        var typicalRoleId = GetTypicalRoleId(contactSkillRecord.SkillCategoryId);

                        if (typicalRoleId != -1)
                        {
                            //If there is a typical role id, then insert a record into the PersonTypicalRole table:
                            var personTypicalRoleRecord = _db.InsertPersonTypicalRole(contactSkillRecord.PersonId, typicalRoleId, 1, null);
                            var personTypicalRoleId = personTypicalRoleRecord.Select(d => d.Id).ToList();
                            Debug.WriteLine("\n" + "PersonTypicalRoleId: " + personTypicalRoleId[0] + "  PersonId: " + contactSkillRecord.PersonId + "  " + " SkillId: " + contactSkillRecord.SkillId + "  " + "TypicalRoleId: " + typicalRoleId);
                        }


                    }
                }
                catch
                    (Exception ex)
                {
                    new LogWriterFactory().Create().Write(ex.Expand("Error occured with PersonTypicalRole entry: "));
                }
            }
        }

        private static int GetTypicalRoleId(int categoryId)
        {
            var typicalRoleId = -1;

            switch (categoryId)
            {
                case 1:
                    typicalRoleId = 3;
                    break;
                case 2:
                    typicalRoleId = 1;
                    break;
                case 3:
                    typicalRoleId = 12;
                    break;
                case 4:
                    typicalRoleId = 9;
                    break;
            }

            return typicalRoleId;
        }
    }

}
