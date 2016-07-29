using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using ConsoleApplication1;
using ConsoleApplication1.Helper;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace ConsoleApplication1
{
    class UserMigration
    {

        static Entities _db;

        private static int _personId = -1;

        public static void UserImport()
        {
            using (_db = new Entities())
            {
                try
                {

                    //Read in the data from the TempUserFileTable:
                    var userData = _db.ReadUser().ToList();

                    foreach (var userRecord in userData)
                    {

                        //Read in the User file data stored in the TempUserFileTable table:
                        var returnedPersonId = _db.InsertUser(null, userRecord.FirstName, userRecord.LastName, userRecord.email, 0, Convert.ToInt32(userRecord.idUser)).ToList();
                        _personId = Convert.ToInt32(returnedPersonId[0]);

                        var person = new Person();

                        _db.UpdateUserMployId(_personId, Convert.ToInt32(userRecord.idUser));


                        //Email
                        //var emailResult = _db.InsertPersonContactInformation(_personId, 2, userRecord.Email, null, true, 0).ToList();

                        //Phone
                        var phoneResult = _db.InsertPersonContactInformation(_personId, 14, userRecord.Phone, true, null, 0).ToList();
                        //var insertedPersonContactInformationId =  result;

                        Debug.WriteLine("\n" + "Person imported: " + _personId);
                    }

                }
                catch
                (Exception ex)
                {
                    new LogWriterFactory().Create().Write(ex.Expand("Error occured with Person Id: " + _personId));
                }
            }
        }
        //Helper methods:


    }
}
