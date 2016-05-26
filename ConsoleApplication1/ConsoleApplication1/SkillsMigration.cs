using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Diagnostics;
using System.Linq;
using System.Xml.Schema;
using ConsoleApplication1;
using ConsoleApplication1.Helper;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace ConsoleApplication1
{
    class SkillsMigration
    {
        private static string _skillName = String.Empty;
        private static int _skillId = -1;

        static Entities _db;

        public static void SkillsImport()
        {
            using (_db = new Entities())
            {
                try
                {
                    var skillData = _db.ReadSkill().ToList();

                    foreach (var skillRecord in skillData)
                    {
                     //For each skill record, get the data we need and insert it into the Skills table in Intersect.

                        _skillName = skillRecord.Skill;

                        //Insert the current skill:
                        var insertedSkillRecord = _db.InsertSkill(skillRecord.Skill, skillRecord.created, null, 0, skillRecord.idSkill).ToList();

                        //Get our skill Category id for the incoming skill based on the Mploy category:
                        var skillCategoryId = GetSkillCategoryId(skillRecord.idCategory);
                        
                        foreach (var item in insertedSkillRecord)
                        {
                            _skillId = item.Id;
                        }

                        //If the skill category is not -1, then insert the into the skillCategoryMapping table:
                        if (skillCategoryId != -1)
                        {
                            //Insert a record into the SkillCategoryMapping table for the skill:
                            var skillCategoryMappingId = _db.InsertSkillCategoryMapping(_skillId, skillCategoryId, 0, skillRecord.created, null); 
                        }

                        Debug.WriteLine("\n" + "Skill imported: " + _skillName + " : " + " SkillId: " + _skillId);
                    }
                }
                catch
                    (Exception ex)
                {
                    new LogWriterFactory().Create().Write(ex.Expand("Error occured with Skill: " + _skillName + " and SkillId: " + _skillId));
                }
            }
        }

        //Returns our SkillCategory id that matches the incoming mploy category id for the skill
        static int GetSkillCategoryId(int mployCategoryId)
        {
            /** Mploy Category Ids:
                2	Business Analyst
                3	Project Management
                4	QA
                5	Developer
                6	Industries
                7	Packages
                8	Modules
                9	Communications
                10	Job Status 
             * 
             * Intersect Category Ids:
                1	Business Analyst Skills
                2	Project Manager Skills
                3	Developer Skills
                4	Quality Assurance Analyst Skills
                5	Packages
                8	Industries
                9	Departments
            **/
            var skillCategoryId = -1;

            switch (mployCategoryId)
            {
                case 2:
                    skillCategoryId = 1;
                    break;
                case 3:
                    skillCategoryId = 2;
                    break;
                case 4:
                    skillCategoryId = 4;
                    break;
                case 5:
                    skillCategoryId = 3;
                    break;
                case 6:
                    skillCategoryId = 8;
                    break;
                case 7:
                    skillCategoryId = 5;
                    break;
                //case 8:
                //case 9:
                //case 10:
                //default:
                //    skillCategoryId = ;
                //    break;
            }

            return skillCategoryId;
        }
    }
}
