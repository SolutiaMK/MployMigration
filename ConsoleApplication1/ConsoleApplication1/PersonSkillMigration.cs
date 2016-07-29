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
    class PersonSkillMigration
    {
        private static int _mploySkillId = -1;
        private static int _mployContactSkillId = -1;
        private static int _mployIdContact = -1;

        static Entities _db;

        public static void PersonSkillsImport()
        {
            using (_db = new Entities())
            {
                try
                {
                    //Read in the ContactSkill list:
                    var contactSkillData = _db.ReadContactSkill().ToList();

                    //For each ContactSkill in the list, insert a record into the PersonSkill table:
                    foreach (var contactSkillRecord in contactSkillData)
                    {
                        //'if' statement used to run sections of the contectSkillRecord if the sql connection keeps timing out:
                        if (contactSkillRecord.idContactSkill > 183621)
                        {
                            
                            _mploySkillId = contactSkillRecord.idSkill;
                            _mployIdContact = contactSkillRecord.idContact;
                            _mployContactSkillId = contactSkillRecord.idContactSkill;

                            //Read the record and insert data into the PersonSkill table:
                            var insertedPersonSkillRecord = _db.InsertPersonSkill(contactSkillRecord.idContact, contactSkillRecord.idSkill, 3, null, null);

                            var personSkillId = insertedPersonSkillRecord.Select(item => item.Id);                           


                            Debug.WriteLine("\n" + "Skill imported: " + _mployContactSkillId + " : " + " SkillId: " + _mploySkillId);
                        }
                    }
                }
                catch
                    (Exception ex)
                {
                    new LogWriterFactory().Create().Write(ex.Expand("Error occured with ContactSkillId: " + _mployContactSkillId + " and SkillId: " + _mploySkillId));
                }
            }
        }

    }
}
