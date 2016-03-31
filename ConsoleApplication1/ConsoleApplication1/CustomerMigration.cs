using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using ConsoleApplication1;
using ConsoleApplication1.Helper;
using Microsoft.Practices.EnterpriseLibrary.Logging;


namespace ConsoleApplication1
{
    class CustomerMigration
    {
        static Entities _db;
        private static int _customerId = -1;
        private static int _personId = -1;
        private static int _personMailAddressId = -1;

        public static void ContactImport()
        {
            using (_db = new Entities())
            {
                try
                {
                    //Read in the temp Contact table from the DB:
                    var contactData = _db.ReadContact().ToList(x => new ConsoleApplication1.Model.Person
                    {
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        MiddleName = x.MiddleName,
                        Gender = x.Gender,
                        Address = x.Address,
                        City = x.City,
                        State = x.State,
                        Zip = x.Zip,
                        Email = x.eMail,
                        handle0type = Convert.ToInt32(x.handle0type),
                        handle0text = x.handle0text,
                        handle1type = Convert.ToInt32(x.handle1type),
                        handle1text = x.handle1text,
                        handle2type = Convert.ToInt32(x.handle2type),
                        handle2text = x.handle2text,
                        handle3type = Convert.ToInt32(x.handle3type),
                        handle3text = x.handle3text,
                        IdOrganization = Convert.ToInt32(x.idOrganization),
                        Title = x.Title,
                        Created = Convert.ToDateTime(x.Created),
                        IdContact = Convert.ToInt32(x.idContact),
                        IdContactType = Convert.ToInt32(x.idContactType),
                        IdUser = Convert.ToInt32((x.idUserOwner)),
                        Source = Convert.ToInt32(x.IdSource),
                        IdStatus = x.IdStatus
                    });


                    foreach (var contactRecord in contactData)
                    {
                        //Check if the contact record's IdContact is in the Candidate file.
                        //Returns a list of the contact records found in the Candidate file.
                        //var checkForDuplicateRecords = _db.CheckCustomerAgainstCandidateData(contactRecord.IdContact).ToList();
                        ////If the contact file has any records that are also in the Candidate file, then do not import the contact.
                        //if (!checkForDuplicateRecords.Any())
                        //{

                        //If the contact record is a contact (contactTypeId of 1) AND the orgainization record id is not null or 4 (the id of the blank organization record):
                        if (contactRecord.IdContactType == 1 && contactRecord.IdOrganization != 0 &&
                            contactRecord.IdOrganization != 4)
                        {
                            //if (contactRecord.IdContact == 20551)
                            //{
                            //    var name = contactRecord.IdContactType;
                            //}

                            //Get the Contacts with the typeId = 1 that have hiring activities:

                            //Add a catch to skip those records, they are added as a Candidate instead.

                            //Get the user id:
                            var idUserVar = ImportHelperMethods.GetUserId(contactRecord.IdUser);

                            //Get the Organization Id:
                            var orgId = ImportHelperMethods.GetIdOrganization(contactRecord.IdOrganization);

                            var genderTypeId = ImportHelperMethods.GetGenderTypeId(contactRecord.Gender);

                            //Insert the Contact as a Person record first:
                            //Returns the PersonId
                            var personResult =
                                _db.InsertPerson(null, contactRecord.FirstName, contactRecord.LastName, genderTypeId, 0,
                                    contactRecord.Created, contactRecord.IdContact, idUserVar).ToList();
                            _personId = Convert.ToInt32(personResult[0]);


                            //Get the source type:
                            //var sourceTypeList = _db.ReadSourceType().ToList();
                            var sourceTypeIdForContact = ImportHelperMethods.GetSourceTypeId(contactRecord.Source);

                            //Get the customers associated org id:
                            var customerOrgId = _db.FindCustomerOrganizationId(_personId,
                                Convert.ToInt32(contactRecord.IdOrganization));
                            foreach (var org in customerOrgId)
                            {
                                orgId = org;
                            }

                            //Get the customer type id:
                            var customerTypeId = GetCustomerTypeId(contactRecord.IdStatus);


                            //Insert as a Customer
                            //Returns the Customer record
                            var customerResult =
                                _db.InsertCustomer(orgId, _personId, sourceTypeIdForContact, customerTypeId,
                                    contactRecord.Title, null, null, contactRecord.Created, 0, orgId).ToList();
                            foreach (var item in customerResult)
                            {
                                _customerId = item.Id;
                            }



                            //Insert Contact Information
                            //Returns the PersonContactInformation.Id
                            var personContactInformationId = -1;

                            //Insert Email address:
                            var personEmail =
                                _db.InsertPersonContactInformation(_personId, 2, contactRecord.Email, null, true, 0)
                                    .ToList();

                            //0:                        
                            personContactInformationId = InsertContactInformation(contactRecord.handle0text,
                                contactRecord.handle0type);

                            //1:
                            personContactInformationId = InsertContactInformation(contactRecord.handle1text,
                                contactRecord.handle1type);

                            //2:
                            personContactInformationId = InsertContactInformation(contactRecord.handle2text,
                                contactRecord.handle2type);

                            //3:
                            personContactInformationId = InsertContactInformation(contactRecord.handle3text,
                                contactRecord.handle3type);


                            //Insert Mailing Address & PersonMailAddress
                            //Inserts into the MailAddress table and the PersonMailAddress table
                            if (!string.IsNullOrEmpty(contactRecord.Address) &&
                                !string.IsNullOrEmpty(contactRecord.City) && !string.IsNullOrEmpty(contactRecord.State) &&
                                !string.IsNullOrEmpty(contactRecord.Zip))
                            {
                                //Returns the PersonMailAddress.Id
                                var personMailAddressResult =
                                    _db.InsertPersonMailingAddress(_personId, contactRecord.Address, contactRecord.City,
                                        contactRecord.State, contactRecord.Zip, 0).ToList();
                                _personMailAddressId = Convert.ToInt32(personMailAddressResult[0]);
                            }


                            //Insert Customer Notes
                            //Inserts into the CustomerNote table
                            if (!string.IsNullOrEmpty(contactRecord.Notes))
                            {
                                //Returns the CustomerNote.Id
                                var customerNoteResult =
                                    _db.InsertCustomerNote(_customerId, contactRecord.Notes, contactRecord.CreatedDate,
                                        0).ToList();
                            }

                            Debug.WriteLine("\n" + "Person imported: " + _personId + " " + contactRecord.FirstName + " " +
                                            contactRecord.LastName + " Customer Id: " + _customerId);

                        }
                        else
                        {
                            //Write out the skipped records here:
                        }
                        
                    //}
                    }

                }
                catch
                (Exception ex)
                {
                    new LogWriterFactory().Create().Write(ex.Expand("Error occured with Customer Id: " + _customerId + " and Person Id: " + _personId));
                }

            }
        }
        //Helper Methods:



        static int GetCustomerTypeId(int contactTypeId)
        {
            int customerTypeId = -1;

            switch (contactTypeId)
            {
                case 1:
                    customerTypeId = 1;
                    break;
                case 2:
                    customerTypeId = 2;
                    break;
                case 4:
                    customerTypeId = 6;
                    break;
                default:
                    customerTypeId = 9;
                    break;
            }
            return customerTypeId;
        }

       


        static int InsertContactInformation(string contactData, int contactDataType )
        {

            //Email is inserted with the Person
            
            var contactInfoTypeId = 0;
            var personContactInformationId = 0;
            //Call AssignHandleTextType() for each Mploy handleType column (0-3)                                        
            //0:
            if (contactDataType != 0  && contactDataType != 3)
            {
                //Calls a method to find the ContactTypeId of the handleText from the Candidate file, and inserts it into the ContactInformation table in the DB.
                contactInfoTypeId = LookupValue.AssignHandleTextType(contactDataType, contactData);
                List<int?> result;
                switch (contactInfoTypeId)
                {
                    case 1:
                    case 5:
                    case 6:
                    case 14:
                    {
                        result = _db.InsertPersonContactInformation(_personId, contactInfoTypeId, contactData, true, false, 0).ToList();
                    }
                        break;
                    case 2:
                    {
                        result = _db.InsertPersonContactInformation(_personId, contactInfoTypeId, contactData, false, true, 0).ToList();
                    }
                        break;
                    default:
                        result = _db.InsertPersonContactInformation(_personId, contactInfoTypeId, contactData, false, false, 0).ToList();
                        break;
                }

                personContactInformationId = Convert.ToInt32(result[0]);

            }

            return personContactInformationId;
        }

        


    }
}
