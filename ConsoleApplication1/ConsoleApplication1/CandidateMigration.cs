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

        //private static int _mailingAddressId = -1;
        //private static int _personId = -1;
        //private static int _candidateId = -1;
        private static int _mployContactId = -1;

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
                            //Call method to Insert the data as a person and candidate:
                            //if (candidateRecord.FirstName != "Unknown" && candidateRecord.LastName != "Unknown")
                            //{
                                
                            
                                _mployContactId = candidateRecord.IdContact;

                                //Get the user id:
                                var idUserVar = ImportHelperMethods.GetUserId(candidateRecord.IdUser);

                                //Get the GenderTypeId of the person being inserted:
                                var genderTypeId = ImportHelperMethods.GetGenderTypeId(candidateRecord.Gender);

                                //Insert the contact as a Person first:
                                //Returns the PersonId
                                var personId = -1;
                                
                                var personResult = _db.InsertPerson(null, candidateRecord.FirstName, candidateRecord.LastName, genderTypeId, 0, candidateRecord.Created, candidateRecord.IdContact, idUserVar).ToList();
                                personId = Convert.ToInt32(personResult[0]);

                                InsertCandidate(candidateRecord, personId);

                                //Update the Person table 'Preferred' columns for email, phone, and home address:
                                _db.UpdatePersonPreferredData(candidateRecord.IdContact);

                            //}
                        }                                          
                    }
                    //Import the contacts that are customers with hiring activities:

                }
                catch
                (Exception ex)
                {
                    new LogWriterFactory().Create().Write(ex.Expand("Error occured with Contact Id: " + _mployContactId));
                }

            }
        }

        public static void InsertCandidate(Model.Person candidateRecord, int personId)
        {          

            //Get the source type for the candidate:
            var sourceTypeIdForCandidate = ImportHelperMethods.GetSourceTypeId(candidateRecord.Source);

            //Get the candidate type id:
            var candidateStatusTypeId = GetCandidateStatusTypeId(candidateRecord.IdStatus);

            Candidate candidateData = new Candidate();

            if (!string.IsNullOrEmpty(candidateRecord.xdata))
            {
                candidateData = LookupValue.ParseXML(candidateRecord.xdata, candidateData);
            }

            //Get the org id:
            var orgId = ImportHelperMethods.GetIdOrganization(candidateRecord.IdOrganization);

            var candidateId = -1;

            //Insert as a Candidate
            //Returns the Candidate record
            var candidateResult = _db.InsertCandidate(personId, candidateStatusTypeId, null, null, null, sourceTypeIdForCandidate, candidateData.MaxTravelTypeId, null, null, candidateData.CurrentSalary, candidateData.DesiredSalary, candidateData.CurrentRate, candidateData.DesiredRate, null, candidateData.IsOpenToRelocation, 0, candidateRecord.Created, orgId).ToList();
            foreach (var item in candidateResult)
            {
                candidateId = item.Id;
            }


            //Insert the Candidate's Mailing Address and Contact Information and Notes:
            InsertPersonContactInfoAndMailAddress(candidateRecord, personId, candidateId);



            Debug.WriteLine("\n" + "Person imported: " + personId + " " + candidateRecord.FirstName + " " + candidateRecord.LastName + " Customer Id: " + candidateId);

        }

        static void InsertPersonContactInfoAndMailAddress(Model.Person candidateRecord, int personId, int candidateId)
        {
            //Insert the Person's email:
            if (!string.IsNullOrEmpty(candidateRecord.Email))
            {
                var personEmail = _db.InsertPersonContactInformation(personId, 2, candidateRecord.Email, null, true, 0).ToList();
            }

            //Insert Candidate Information
            //Returns the PersonContactInformation.Id
            var personContactInformationId = -1;
            var mailingAddressId = -1;
            //0:                        
            personContactInformationId = InsertContactInformation(personId, candidateRecord.handle0text, candidateRecord.handle0type);

            //1:
            personContactInformationId = InsertContactInformation(personId, candidateRecord.handle1text, candidateRecord.handle1type);

            //2:
            personContactInformationId = InsertContactInformation(personId, candidateRecord.handle2text, candidateRecord.handle2type);

            //3:
            personContactInformationId = InsertContactInformation(personId, candidateRecord.handle3text, candidateRecord.handle3type);

            //Insert Mailing Address & PersonMailAddress
            //Inserts into the MailAddress table and the PersonMailAddress table
            if (!string.IsNullOrEmpty(candidateRecord.Address) && !string.IsNullOrEmpty(candidateRecord.City) && !string.IsNullOrEmpty(candidateRecord.State) && !string.IsNullOrEmpty(candidateRecord.Zip))
            {
                //Returns the PersonMailAddress.Id
                var personMailAddressResult = _db.InsertPersonMailingAddress(personId, candidateRecord.Address, candidateRecord.City, candidateRecord.State, candidateRecord.Zip, 0).ToList();
                mailingAddressId = Convert.ToInt32(personMailAddressResult[0]);
            }

            //Insert Candidate Notes
            //Inserts into the CandidateNote table
            if (!string.IsNullOrEmpty(candidateRecord.Notes))
            {
                //Returns the CustomerNote.Id
                var customerNoteResult = _db.InsertCandidateNote(candidateId, candidateRecord.Notes, candidateRecord.Created, 0).ToList();
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
        static int InsertContactInformation(int personId, string contactData, int contactDataType)
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
                            result = _db.InsertPersonContactInformation(personId, contactInfoTypeId, contactData, true, false, 0).ToList();
                        }
                        break;
                    case 2:
                        {
                            result = _db.InsertPersonContactInformation(personId, contactInfoTypeId, contactData, false, true, 0).ToList();
                        }
                        break;
                    default:
                        result = _db.InsertPersonContactInformation(personId, contactInfoTypeId, contactData, false, false, 0).ToList();
                        break;
                }

                personContactInformationId = Convert.ToInt32(result[0]);

            }

            return personContactInformationId;
        }
    }
}
