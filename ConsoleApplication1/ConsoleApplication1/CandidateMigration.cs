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
    internal class CandidateMigration
    {

        private static int _mailingAddressId = -1;
        private static int _personId = -1;
        private static int _candidateId = -1;

        static Entities _db;
        static List<KeyValuePair<int, string>> _sourceList = new List<KeyValuePair<int, string>>();


        public static void TestUpdate()
        {
            using (_db = new Entities())
            {
                try
                {

                    //Reads in the Candidate record from the Temp table in the DB and uses our custom model.
                    var data = _db.ReadPerson().ToList(x => new Model.Person
                    {
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        MiddleName = x.MiddleName,
                        Gender = x.Gender,
                        //Status = x.Status,
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
                        xdata = x.Xdata,
                        Notes = x.Notes,
                        ResumeText = x.ResumeText,
                        //Source = x.Source,
                        IdContact = Convert.ToInt32(x.idContact),
                        IdContactType = Convert.ToInt32(x.idContactType),
                        IdUser = Convert.ToInt32((x.idUserOwner)),
                        Source = Convert.ToInt32(x.IdSource),
                        IdStatus = x.IdStatus,
                        Created = Convert.ToDateTime(x.Created)
                    });



                    //For each person record from the Candidate file table:
                    foreach (var candidateRecord in data)
                    {
                        
                        //Check that the incoming contact record is of the Candidate type:
                        if (candidateRecord.IdContactType == 2 || candidateRecord.IdContactType == 5)
                        {

                            //Get the user id:
                            var idUserVar = ImportHelperMethods.GetUserId(candidateRecord.IdUser);

                            //Get the org id:
                            var orgId = ImportHelperMethods.GetIdOrganization(candidateRecord.IdOrganization);

                            //Get the GenderTypeId of the person being inserted:
                            var genderTypeId = ImportHelperMethods.GetGenderTypeId(candidateRecord.Gender);  

                            //Insert the contact as a Person first:
                            //Returns the PersonId
                            var personResult = _db.InsertPerson(null, candidateRecord.FirstName, candidateRecord.LastName, genderTypeId, 0, candidateRecord.Created, candidateRecord.IdContact, idUserVar).ToList();
                            _personId = Convert.ToInt32(personResult[0]);

                            //Get the source type for the candidate:
                            var sourceTypeIdForCandidate = ImportHelperMethods.GetSourceTypeId(candidateRecord.Source);

                            //Get the candidate type id:
                            var candidateStatusTypeId = GetCandidateStatusTypeId(candidateRecord.IdStatus);

                            Candidate candidateData = new Candidate();

                            if(!string.IsNullOrEmpty(candidateRecord.xdata))
                            {
                                candidateData = LookupValue.ParseXML(candidateRecord.xdata, candidateData);
                            }
                            

                            //Insert as a Candidate
                            //Returns the Candidate record
                            var candidateResult = _db.InsertCandidate(_personId, candidateStatusTypeId, null, null, null, sourceTypeIdForCandidate, candidateData.MaxTravelTypeId, null, null, candidateData.CurrentSalary, candidateData.DesiredSalary, candidateData.CurrentRate, candidateData.DesiredRate, null, null, candidateData.IsOpenToRelocation, 0, candidateRecord.Created, orgId).ToList();
                            foreach (var item in candidateResult)
                            {
                                _candidateId = item.Id;
                            }


                            //Insert the Person's email:
                            if (!string.IsNullOrEmpty(candidateRecord.Email))
                            {
                                var personEmail = _db.InsertPersonContactInformation(_personId, 2, candidateRecord.Email, null, null, 0).ToList(); 
                            }
                            
                            //Insert Candidate Information
                            //Returns the PersonContactInformation.Id
                            var personContactInformationId = -1;
                            //0:                        
                            personContactInformationId = InsertContactInformation(candidateRecord.handle0text, candidateRecord.handle0type);

                            //1:
                            personContactInformationId = InsertContactInformation(candidateRecord.handle1text, candidateRecord.handle1type);

                            //2:
                            personContactInformationId = InsertContactInformation(candidateRecord.handle2text, candidateRecord.handle2type);

                            //3:
                            personContactInformationId = InsertContactInformation(candidateRecord.handle3text, candidateRecord.handle3type);

                            //Insert Mailing Address & PersonMailAddress
                            //Inserts into the MailAddress table and the PersonMailAddress table
                            if (!string.IsNullOrEmpty(candidateRecord.Address) && !string.IsNullOrEmpty(candidateRecord.City) && !string.IsNullOrEmpty(candidateRecord.State) && !string.IsNullOrEmpty(candidateRecord.Zip))
                            {
                                //Returns the PersonMailAddress.Id
                                var personMailAddressResult = _db.InsertPersonMailingAddress(_personId, candidateRecord.Address, candidateRecord.City, candidateRecord.State, candidateRecord.Zip, 0).ToList();
                                _mailingAddressId = Convert.ToInt32(personMailAddressResult[0]);
                            }

                            //Insert Candidate Notes
                            //Inserts into the CandidateNote table
                            if (!string.IsNullOrEmpty(candidateRecord.Notes))
                            {
                                //Returns the CustomerNote.Id
                                var customerNoteResult = _db.InsertCandidateNote(_candidateId, candidateRecord.Notes, candidateRecord.Created, 0).ToList();
                            }

                            Debug.WriteLine("\n" + "Person imported: " + _personId + " " + candidateRecord.FirstName + " " + candidateRecord.LastName + " Customer Id: " + _candidateId);
                        }                                          
                    }

                }
                catch
                (Exception ex)
                {
                    new LogWriterFactory().Create().Write(ex.Expand("Error occured with PersonId: " + _personId));
                }

            }
        }

        public static int GetCandidateStatusTypeId(int statusId)
        {
            int candidateStatusTypeId = -1;

            switch (statusId)
            {
                case 1:
                    candidateStatusTypeId = 1;
                    break;
                case 2:
                    candidateStatusTypeId = 4;
                    break;
                case 4:
                    candidateStatusTypeId = 2;
                    break;
                default:
                    candidateStatusTypeId = 3;
                    break;
            }
            return candidateStatusTypeId;
        }

        //Inserts the ContactInformation for the given personId and contact info:
        static int InsertContactInformation(string contactData, int contactDataType)
        {

            //Email is inserted with the Person

            var contactInfoTypeId = 0;
            var personContactInformationId = 0;
            //Call AssignHandleTextType() for each Mploy handleType column (0-3)                                        
            //0:
            if (contactDataType != 0 && contactDataType != 3)
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



        static void InsertMailingAddress(ConsoleApplication1.Model.Person record)
        {
            //I changed the CreateDate & LastUpdated columns to be of type DateTime2. I need to look into this...
            //Do same for the MailingAddress table
            MailingAddress insertMailingAddress = new MailingAddress();

            //Check the Status column from the Candidate file.  If Status == 'Active', then IsActive = true, else IsActive = false.
            //insertPerson.IsActive = (record.Status == "Active") ? true : false;

            insertMailingAddress.Line1 = (string.IsNullOrEmpty(record.Address))
                ? "Not Available"
                : record.Address;
            //record.idcontact.ToString();


            insertMailingAddress.City = (string.IsNullOrEmpty(record.City))
                ? "Not Available"
                : record.City;
            insertMailingAddress.State = (string.IsNullOrEmpty(record.State))
                ? "NA"
                : record.State;
            insertMailingAddress.Zip = (string.IsNullOrEmpty(record.Zip))
                ? "Not Available"
                : record.Zip;


            insertMailingAddress.CreateDate = System.DateTime.Now;
            insertMailingAddress.LastUpdated = System.DateTime.Now;

            //Add to the MailingAddress table
            _db.MailingAddresses.Add(insertMailingAddress);

            _db.SaveChanges();
            //Get the mailingAddress's Id
            _mailingAddressId = insertMailingAddress.Id;
        }

        static void InsertPersonMailAddress(ConsoleApplication1.Model.Person record)
        {
            //Take the ids and insert into the PersonMailAddress table
            PersonMailAddress insertPersonMailAddress = new PersonMailAddress();
            insertPersonMailAddress.PersonId = _personId;
            insertPersonMailAddress.MailingAddressId = _mailingAddressId;

            //PersonMailAddress -> MailAddressType: 1 = home, 2 = office
            //Not null field, I am defaulting it to 1 for now.
            insertPersonMailAddress.MailingAddressTypeId = 1;
            insertPersonMailAddress.CreateDate = DateTime.Now;
            insertPersonMailAddress.LastUpdated = DateTime.Now;

            //Add the new PersonMailAddress to the db and save the changes
            _db.PersonMailAddresses.Add(insertPersonMailAddress);
            _db.SaveChanges();
        }
    }
}
