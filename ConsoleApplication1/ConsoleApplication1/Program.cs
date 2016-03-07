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

        /********************
         * This is the main program to inport the data from the temp tables to the DB.
         * The temp files are created by running the SSIS program 'BasicsLocalTransferFromMploy'. It will read in the Mploy CSV files and import the data into a temp table in the DB
         * Then that temp data is imported to the correct tables by running this.
         * 
         * I have commented each section below. When I was running and testing this I would run one section at a time. (These all start in the Main.)
         * 
         * For example:
         * Uncomment the 'Candidate Insert' section to import the data from the Mploy file 'Candidate'. 
         * I tried to name each section according to the Mploy file name it was comming from.
         * It is also in the order I imported everything in from top down to deal with dependencies. (starting in the main)
        *********************/

        static Entities _db;

        static Guid _globalEntityId;
        static int _personId = 0;
        static int _mailingAddressId = 0;
        static int _candidateId = 0;
        static List<KeyValuePair<int, string>> sourceList = new List<KeyValuePair<int, string>>();

        static int _companyId = 0;
        static int _companyMailingAddressId = 0;

        private static int _contactId = 0;




        //Insert into GlobalEntity:
        static GlobalEntity InsertGlobalEntity(Model.Person record, int entityType)
        {
            //Insert the EntityTypeId of the new record
            GlobalEntity insertGlobalEntity = new GlobalEntity();
            insertGlobalEntity.EntityTypeId = entityType;            
            //Save the record to the DB
            _db.GlobalEntities.Add(insertGlobalEntity);
            _db.SaveChanges();
            //Get the generated GlobalEntityId and save it to be used later
            _globalEntityId = insertGlobalEntity.GlobalEntityId;
            return insertGlobalEntity;
        }



        //Insert Methods:
        static Person InsertPerson(Model.Person record)
        {
            //Insert into person table by creating an instance of the Person object
            Person insertPerson = new Person();

            insertPerson.FirstName = record.FirstName;
            insertPerson.LastName = record.LastName;

            Debug.WriteLine("\n" + record.FirstName + " " + record.LastName + "");

            //Check to see if the First and Last names are ever null

            if (record.FirstName == null)
            {
                insertPerson.FirstName = "Not Provided";
            }
            else if (record.LastName == null)
            {
                insertPerson.LastName = "Not Provided";
            }
            //else if (record.FirstName == null && record.LastName == null)
            //{
            //    continue;
            //}//add else to handle the normal case (they have both names)

            insertPerson.MPLOY_ContactId = record.IdContact;

            //Insert the CreateDate from the Candidate file but if null, then use the system time.
            //insertPerson.CreateDate = record.CreatedDate != null ? record.CreatedDate : DateTime.Now;
            //insertPerson.CreateDate = System.DateTime.Now;
            insertPerson.CreateDate = Convert.ToDateTime(record.CreatedDate);

            //Check the Status column from the Candidate file.  If Status == 'Active', then IsActive = true, else IsActive = false if status == Inactive or Not Being Considered.
            //insertPerson.IsActive = (record.Status == "Active") ? true : false;
            //switch (record.Status)
            //{
            //    case "Active":
            //        insertPerson.IsActive = true;
            //        break;
            //    case "Inactive":
            //        insertPerson.IsActive = false;
            //        break;
            //    case "Not Being Considered":
            //        insertPerson.IsActive = false;
            //        break;
            //}

            //Check Gender and assign the correct type. Possibly bring the GenderTypeId table in and assign that way instead of hard coding...                      
            //Calls the FindGenderType(), takes in the current rows gender column, validates type, returns the assigned gender type Id.
            //insertPerson.GenderTypeId = LookupValue.FindGenderType(record.Gender, insertPerson);

            insertPerson.LastUpdated = DateTime.Now;

            //add to the person table and save the changes
            _db.People.Add(insertPerson);
            _db.SaveChanges();

            //Get the person's Id
            _personId = insertPerson.Id;
            return insertPerson;
        }

//        static void InsertContactInformation(ConsoleApplication1.Model.Person record)
//        {
//            PersonContactInformation insertContactInformation = new PersonContactInformation();
//            //ContactInfoType contactInfoType = new ContactInfoType();

//            insertContactInformation.CreateDate = DateTime.Now;
//            insertContactInformation.LastUpdated = DateTime.Now;

///*            if (record.handle3type.Contains(",") && record.handle3type != null)
//            {
//                record.handle3type = record.handle3type.Replace(",", "");
//            }
// */

//            //Call AssignHandleTextType() for each Mploy handleType column (0-3)                                        
//            //0:
//            insertContactInformation.PersonId = _personId;
//            //Calls a method to find the ContactTypeId of the handleText from the Candidate file, and inserts it into the ContactInformation table in the DB.
//            var contactInfoTypeId = LookupValue.AssignHandleTextType(record.handle0type, record.handle0text);
//            insertContactInformation.ContactInformationTypeId = contactInfoTypeId;
//            if (record.handle0type != "0" && !string.IsNullOrEmpty(record.handle0type) && record.handle0type != "3")
//            {
//                insertContactInformation.Description = record.handle0text;
//                _db.PersonContactInformations.Add(insertContactInformation);
//                _db.SaveChanges();
//            }
//            //insertContactInformation.IsPreferred = true;
//            //insertContactInformation.IsActive = true;

//            //1: 
//            insertContactInformation.PersonId = _personId;
//            contactInfoTypeId = LookupValue.AssignHandleTextType(record.handle1type, record.handle1text);
//            insertContactInformation.ContactInformationTypeId = contactInfoTypeId;
//            if (record.handle1type != "0" && !string.IsNullOrEmpty(record.handle1type) && record.handle1type != "3")
//            {
//                insertContactInformation.Description = record.handle1text;
//                _db.PersonContactInformations.Add(insertContactInformation);
//                _db.SaveChanges();
//            }
//            //2:
//            insertContactInformation.PersonId = _personId;
//            contactInfoTypeId = LookupValue.AssignHandleTextType(record.handle2type, record.handle2text);
//            insertContactInformation.ContactInformationTypeId = contactInfoTypeId;
//            if (record.handle2type != "0" && !string.IsNullOrEmpty(record.handle2type) && record.handle2type != "3")
//            {
//                insertContactInformation.Description = record.handle2text;
//                _db.PersonContactInformations.Add(insertContactInformation);
//                _db.SaveChanges();
//            }

//            //3:
//            insertContactInformation.PersonId = _personId;
//            contactInfoTypeId = LookupValue.AssignHandleTextType(record.handle3type, record.handle3text);
//            insertContactInformation.ContactInformationTypeId = contactInfoTypeId;
//            if (record.handle3type != "0" && !string.IsNullOrEmpty(record.handle3type) && record.handle3type != "3")
//            {
//                insertContactInformation.Description = record.handle3text;
//                _db.PersonContactInformations.Add(insertContactInformation);
//                _db.SaveChanges();
//            }

//            //Email: Mploy has email as a seperate column, so I am inserting it seperate from the Handletext/type fields
//            insertContactInformation.PersonId = _personId;
//            insertContactInformation.ContactInformationTypeId = 2;
//            insertContactInformation.Description = record.Email;
//            _db.PersonContactInformations.Add(insertContactInformation);
//            _db.SaveChanges();
//        }

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



        //Takes in the current Candidate that is being added and the Candidate's associated person record
        //Inserts the candidate record and returns the inserted candidate so it can be used later.
        //public static Candidate InsertCandidateRecord(ConsoleApplication1.Model.Person candidateRecord, Person insertPerson)
        //{
        //    var insertedCandidate = new Candidate();


        //    Candidate insertCandidate = new Candidate();
        //    //Insert the current personId so the data is associated to the right person.
        //    insertCandidate.PersonId = _personId;

        //    ///////////////////////////  Parse XML  ////////////////////////////////////                        
        //    //Calls a method to parse through the xdata column as long as it is not null.
        //    if (candidateRecord.xdata != null)
        //    {
        //        insertCandidate = LookupValue.ParseXML(candidateRecord.xdata, _db, insertCandidate,
        //            insertPerson);
        //        //_db.People.Attach(insertPerson);
        //        //_db.SaveChanges();
        //    }
        //    switch (candidateRecord.Status)
        //    {
        //        case "Active":
        //            insertCandidate.CandidateStatusTypeId = 1;
        //            break;
        //        case "Inactive":
        //            insertCandidate.CandidateStatusTypeId = 0;
        //            break;
        //        case "Not Being Considered":
        //            insertCandidate.CandidateStatusTypeId = 2;
        //            break;
        //    }

        //    //Method to lookup the SourceType from the Source column in the "Candidate" file to the corresponding SourceTypeId from the SoureType table in the DB
        //    insertCandidate = LookupValue.FindSourceType(insertCandidate, candidateRecord.Source, sourceList);
        //    insertCandidate.CreateDate = DateTime.Now;
        //    insertCandidate.LastUpdated = DateTime.Now;

        //    insertCandidate.ResumeSummary = candidateRecord.ResumeText;

        //    //insertCandidate.IsActive = true;
        //    insertCandidate.ModifiedById = 0;
        //    insertCandidate.CreatedById = 0;


        //    insertCandidate.GlobalEntityId = _globalEntityId;

        //    return insertedCandidate;
        //}

        static void Main(string[] args)
        {
            using (_db = new Entities())
            {
                try
                {

                    ////Reads in the Candidate record from the Temp table in the DB and uses our custom model.
                    //var data = _db.ReadPerson().ToList(x => new Model.Person { FirstName = x.FirstName, LastName = x.LastName, MiddleName = x.MiddleName, Gender = x.Gender, Status = x.Status, Address = x.Address, City = x.City, 
                    //    State = x.State, Zip = x.Zip, Email = x.Email, handle0type = x.handle0type, handle0text = x.handle0text, handle1type = x.handle1type, handle1text = x.handle1text, 
                    //    handle2type = x.handle2type, handle2text = x.handle2text, handle3type = x.handle3type, handle3text = x.handle3text, xdata = x.xdata, Notes = x.Notes, ResumeText = x.ResumeText, Source = x.Source, IdContact = Convert.ToInt32(x.idcontact)});

                    ////Read in and store the temp Organzation table from the DB:
                    ////var organizationData = _db.ReadOrganization().ToList();
                    
                    ////Read in the temp Contact table from the DB:
                    //var contactData = _db.ReadContact().ToList(x => new ConsoleApplication1.Model.Person { FirstName = x.FirstName, LastName = x.LastName, MiddleName = x.MiddleName, Gender = x.Gender, Address = x.Address, City = x.City, 
                    //    State = x.State, Zip = x.Zip, Email = x.Email, handle0type = x.handle0type, handle0text = x.handle0text, handle1type = x.handle1type, handle1text = x.handle1text, 
                    //    handle2type = x.handle2type, handle2text = x.handle2text, handle3type = x.handle3type, handle3text = x.handle3text, IdOrganization = Convert.ToInt32(x.IdOrganization), Title = x.Title, Created = Convert.ToDateTime(x.Created), IdContact = Convert.ToInt32(x.IdContact)});

                    ////var jobData = _db.ReadJob().ToList();

                    //var hiringActivityData = _db.ReadHiringActivity().ToList();

                    //var contactLogData = _db.ReadContactLog().ToList();

                    //Calls the GetSourceTypeList method to generate the list I use to find the source for each person
                    sourceList = LookupValue.GetTypeIdList(sourceList);


                    //UserMigration.UserImport();

                    //OrganizationMigration.OrganizationImport();

                    //CustomerMigration.ContactImport();

                    CandidateMigration.TestUpdate();
                    
                    












                    

                    //////////////// Candidate Insert ////////////////
                    //Read each person from the data
                    //foreach (var record in data){
                        //This 'if' is for testing:
                        //if (record.LastName == "Larson" && record.FirstName == "Adam")
                        //{
                            

                    //    ///////////////////////////  Person  ////////////////////////////////////
                    //    //Call the insert Person method, return the insertPerson var to be used later.
                    //    Person insertPerson = InsertPerson(record);


                    //    ///////////////////////////////  ContactInformation  ////////////////////////////////////
                    //    InsertContactInformation(record);


                    //    ///////////////////////////////  MailingAddress  ////////////////////////////////////
                    //    InsertMailingAddress(record);


                    //    //If the mailingAddressId & the personId > 0, then insert into PersonMailAddress & Candidate:
                    //    if (_mailingAddressId > 0 && _personId > 0)
                    //    {

                    //        ///////////////////////////  PersonMailAddress  ////////////////////////////////////
                    //        InsertPersonMailAddress(record);


                    //        //Global Entity
                    //        //Candidate = EntityTypeId of 2
                    //        int entityType = 2;
                    //        GlobalEntity insertGlobalEntity = InsertGlobalEntity(record, entityType);


                    //        ///////////////////////////  Candidate  ////////////////////////////////////   

                    //        //Calls the InsertCandidate function with the current person and candidate info.
                    //        //Returns the candidate record to be saved to the database.
                    //        var insertCandidate = InsertCandidateRecord(record, insertPerson);
                                
                   
                    //        //MOVED TO THE INSERTCANDIDATERECORD FUNCTION!!!!!!!!!
                    //        //Candidate insertCandidate = new Candidate();
                    //        ////Insert the current personId so the data is associated to the right person.
                    //        //insertCandidate.PersonId = personId;

                    //        /////////////////////////////  Parse XML  ////////////////////////////////////                        
                    //        ////Calls a method to parse through the xdata column as long as it is not null.
                    //        //if (record.xdata != null)
                    //        //{
                    //        //    insertCandidate = LookupValue.ParseXML(record.xdata, _db, insertCandidate,
                    //        //        insertPerson);
                    //        //    //_db.People.Attach(insertPerson);
                    //        //    //_db.SaveChanges();
                    //        //}
                    //        //switch (record.Status)
                    //        //{
                    //        //    case "Active":
                    //        //        insertCandidate.CandidateStatusTypeId = 1;
                    //        //        break;
                    //        //    case "Inactive":
                    //        //        insertCandidate.CandidateStatusTypeId = 0;
                    //        //        break; 
                    //        //    case "Not Being Considered":
                    //        //        insertCandidate.CandidateStatusTypeId = 2;
                    //        //        break;
                    //        //}

                    //        ////Method to lookup the SourceType from the Source column in the "Candidate" file to the corresponding SourceTypeId from the SoureType table in the DB
                    //        //insertCandidate = LookupValue.FindSourceType(insertCandidate, record.Source, sourceList);
                    //        //insertCandidate.CreateDate = DateTime.Now;
                    //        //insertCandidate.LastUpdated = DateTime.Now;

                    //        //insertCandidate.ResumeSummary = record.ResumeText;

                    //        ////insertCandidate.IsActive = true;
                    //        //insertCandidate.ModifiedById = 0;
                    //        //insertCandidate.CreatedById = 0;
                    //        ////////////////////////////////////////////// Moved to the InsertCandidateRecord function - END
                    //        _db.Candidates.Add(insertCandidate);
                    //        _db.SaveChanges();

                    //        _candidateId = insertCandidate.Id;
                    //    }

                    //    ///////////////////////////  CandidateNote  ////////////////////////////////////
                    //    //Needs a type from the CandidateNoteType: 1 == Summary, 2 == Comments
                    //    //Connects to Candidate from the CandidateId


                    //    CandidateNote insertCandidateNote = new CandidateNote();
                    //    insertCandidateNote.CandidateId = _candidateId;
                    //    //Insert the Notes associated with current Candidate. If null, insert "None Provided"
                    //    insertCandidateNote.Note = record.Notes ?? "None Provided";


                    //    //insertCandidateNote.CommentDate = DateTime.Now; //(DateTime) record.CreatedDate;
                    //    //Default the notes column to have the CandidateNoteTypeId to 2 because all the notes from Mploy should be comments.
                    //    //insertCandidateNote.CandidateNoteTypeId = 2;
                    //    insertCandidateNote.CreateDate = DateTime.Now;
                    //    insertCandidateNote.LastUpdated = DateTime.Now;
                            

                    //    _db.CandidateNotes.Add(insertCandidateNote);
                    //    _db.SaveChanges();


                    //        //NOTE:
                    //        //The only column I have not imported yet is the Title column.  
                    //        //It should belong in JobHistory, but I sould import the Orginization file before inserting into the JobHistory table.
                    //        //This will need to be accessed at some point, so I will need to decide how to save it or how to import files so that they can reference eachother.
                         

                    //    _mailingAddressId = 0;
                    //    _personId = 0;
                    //    _candidateId = 0;
                    ////}//End of the name catch
                    //} 
                    
 //End of the record foreach
    //End of the Candidate insert

/*

                    ///////////////////////////  Organization File Inserts ////////////////////////////////////
                    
                    /****************** Start the Organization (i.e. Company) insert **************************#1#
                    foreach (var orgRecord in organizationData)
                    {
                        //if (orgRecord.Organization == "Glazer's Wholesale Distributors")
                        //{

                            ///////////////////////////////  Organization (i.e. Company) ////////////////////////////////////

                            // ADD IN: Company.CompanyNotes, Company.MPLOY_OrganizationId

                            Company insertCompany = new Company();
                            insertCompany.Description = orgRecord.Organization;

                            Debug.WriteLine("\n" + orgRecord.Organization);

                            insertCompany.LastUpdated = System.DateTime.Now;
                            insertCompany.CreateDate = System.DateTime.Now;
                            //insertCompany.CreatedById = 0;
                            //insertCompany.CompanyNotes = orgRecord.InternalNote;
                            insertCompany.MPLOY_OrganizationId = Convert.ToInt32(orgRecord.idorganization);

                            _db.Companies.Add(insertCompany);
                            _db.SaveChanges();
                            companyId = insertCompany.Id;

                            ///////////////////////////////  MailingAddress for a Company ////////////////////////////////////
                            MailingAddress insertCompanyMailingAddress = new MailingAddress();

                            insertCompanyMailingAddress.Line1 = (string.IsNullOrEmpty(orgRecord.Address))
                                ? "Not Available"
                                : orgRecord.Address;

                            insertCompanyMailingAddress.City = (string.IsNullOrEmpty(orgRecord.City))
                                ? "Not Available"
                                : orgRecord.City;
                            insertCompanyMailingAddress.State = (string.IsNullOrEmpty(orgRecord.State))
                                ? "NA"
                                : orgRecord.State;
                            insertCompanyMailingAddress.Zip = (string.IsNullOrEmpty(orgRecord.Zip))
                                ? "Not Available"
                                : orgRecord.Zip;

                            insertCompanyMailingAddress.CreateDate = DateTime.Now;
                            insertCompanyMailingAddress.LastUpdated = DateTime.Now;

                            //Add to the MailingAddress table
                            _db.MailingAddresses.Add(insertCompanyMailingAddress);
                            _db.SaveChanges();
                            //Get the mailingAddress's Id
                            companyMailingAddressId = insertCompanyMailingAddress.Id;

                            ///////////////////////////////  CompanyContactInformation ////////////////////////////////////
                            CompanyContactInformation insertCompanyContactInformation = new CompanyContactInformation();
                            insertCompanyContactInformation.CompanyId = companyId;
                            //insertCompanyContactInformation.Description = orgRecord.Phone;
                            //insertCompanyContactInformation.Description 

                            //Need to insert 3 times into CompanyContactInformation if not null (Phone, fax, url)
                            if (!string.IsNullOrEmpty(orgRecord.Phone))
                            {
                                insertCompanyContactInformation.Description = orgRecord.Phone;
                                insertCompanyContactInformation.ContactInformationTypeId = 5;
                                insertCompanyContactInformation.CompanyId = companyId;
                                insertCompanyContactInformation.LastUpdated = DateTime.Now;
                                insertCompanyContactInformation.CreateDate = DateTime.Now;
                                _db.CompanyContactInformations.Add(insertCompanyContactInformation);
                                _db.SaveChanges();
                            }

                            if (!string.IsNullOrEmpty(orgRecord.Fax))
                            {
                                insertCompanyContactInformation.Description = orgRecord.Fax;
                                insertCompanyContactInformation.ContactInformationTypeId = 13;
                                insertCompanyContactInformation.CompanyId = companyId;
                                insertCompanyContactInformation.LastUpdated = DateTime.Now;
                                insertCompanyContactInformation.CreateDate = DateTime.Now;
                                _db.CompanyContactInformations.Add(insertCompanyContactInformation);
                                _db.SaveChanges();
                            }

                            if (!string.IsNullOrEmpty(orgRecord.Url))
                            {
                                insertCompanyContactInformation.Description = orgRecord.Url;
                                insertCompanyContactInformation.ContactInformationTypeId = 11;
                                insertCompanyContactInformation.CompanyId = companyId;
                                insertCompanyContactInformation.LastUpdated = DateTime.Now;
                                insertCompanyContactInformation.CreateDate = DateTime.Now;
                                _db.CompanyContactInformations.Add(insertCompanyContactInformation);
                                _db.SaveChanges();
                            }

                            ///////////////////////////////  CompanyMailAddress ////////////////////////////////////
                            CompanyMailAddress insertCompanyMailAddress = new CompanyMailAddress();
                            insertCompanyMailAddress.CompanyId = companyId;
                            insertCompanyMailAddress.MailingAddressId = companyMailingAddressId;
                            insertCompanyMailAddress.LastUpdated = DateTime.Now;
                            insertCompanyMailAddress.CreateDate = DateTime.Now;
                            _db.CompanyMailAddresses.Add(insertCompanyMailAddress);
                            _db.SaveChanges();
                        }


//End of the Company inserts

                    //}//Best Buy catch

                    /****************Ending of the Company insert***************#1#
*/

/*


                    ///////////////////////////  Contact File Inserts ////////////////////////////////////

                    /****************** Start the Contact insert **************************#1#
                    foreach (var contactRecord in contactData)
                    {
                        //if (contactRecord.LastName == "Etzell"){
                            // ADD IN: Customer.MPLOY_OrganizationId

                            ///// Insert the Contact as a Person first /////
                            Person insertPerson = InsertPerson(contactRecord);

                            ///////////////////////////////  ContactInformation  ////////////////////////////////////
                            InsertContactInformation(contactRecord);


                            ///////////////////////////////  MailingAddress  ////////////////////////////////////
                            InsertMailingAddress(contactRecord);


                            //If the mailingAddressId & the personId > 0, then insert into PersonMailAddress & Candidate:
                            if (mailingAddressId > 0 && personId > 0)
                            {

                                ///////////////////////////  PersonMailAddress  ////////////////////////////////////
                                InsertPersonMailAddress(contactRecord);

                            }

                            ///// Then insert the Contact as a Customer /////


                            //Use the new PersonId to insert into the Contact.PersonId column
                            Customer insertCustomer = new Customer();
                            insertCustomer.MPLOY_OrganizationId = contactRecord.IdOrganization;

                            if (!contactRecord.IdOrganization.Equals(null) && contactRecord.IdOrganization != 0)
                            {
                                //ObjectParameter returnedOrgId = null;                           
                                var returnedOrgId = _db.GetCompanyContactAssociation(contactRecord.IdOrganization).ToList();
                                insertCustomer.CompanyId = Convert.ToInt32(returnedOrgId[0]);
                            }



                            insertCustomer.CreateDate = Convert.ToDateTime(contactRecord.Created);
                            //insertCustomer.CreateDate = DateTime.Now;
                            insertCustomer.LastUpdated = DateTime.Now;

                            insertCustomer.PersonId = personId;
                            insertCustomer.Title = contactRecord.Title;
                            //Defaulting the SourceTypeId to 19 (Mploy) because there is no source from Mploy for the Contact file
                            insertCustomer.SourceTypeId = 19;
                            insertCustomer.ModifiedById = 0;
                            insertCustomer.CreatedById = 0;

                            _db.Customers.Add(insertCustomer);
                            _db.SaveChanges();
                        }
                    //}//END of the LastName "if" statement above

                    /****************** End Contact insert **************************#1#
                    //}//Ending bracket for the name "if" statement catch up above.

*/


/*

                    ///////////////////////////  HiringActivity File Inserts ////////////////////////////////////
                    /****************** Start the HiringActivity insert **************************#1#
                    foreach (var hiringActivityRecord in hiringActivityData)
                    {
                        //RequirementActivityLogTypeId
                        //RequirementId | DONE
                        //ActivityLogDate | DONE
                        //ActivityLogNotes | DONE
                        //IsActive - defaulted to 1
                        //LastUpdated - defaulted to getdate()
                        //ModifiedById | DONE
                        //CreateDate - defaulted to getdate()
                        //CreatedById |vDONE

                        //if (hiringActivityRecord.IdJob == 1005 && hiringActivityRecord.IdContact == 4236)
                        //{
                            //HiringActivity can only be associated to a Candidate.
                            RequirementActivityLog insertRequirementActivityLog = new RequirementActivityLog();

                            //SP that takes in the MPLOY JobId and ContactId and returns our DBS id for that requirement and candidate connected to the current hiringActivity
                            var returnedJobAndContactId = _db.GetHiringActivityAssociation(hiringActivityRecord.IdJob, hiringActivityRecord.IdContact).ToList();
                            //[0] holds the JobId
                            //[1] holds the CanddateId
                            insertRequirementActivityLog.RequirementId = Convert.ToInt32(returnedJobAndContactId[0].jobId);
                            insertRequirementActivityLog.CandidateId = Convert.ToInt32(returnedJobAndContactId[0].contactId);

                            Debug.WriteLine("\n" + "Current HiringActivity RequirementId: " + insertRequirementActivityLog.RequirementId + " & CandidateId: " + insertRequirementActivityLog.CandidateId);

                            //If Notes are not null from teh hiringActivityRecord, insert into ActivityLogNotes.  Else, insert "None Available":
                            insertRequirementActivityLog.ActivityLogNotes = hiringActivityRecord.Notes ?? "None Available";

                            insertRequirementActivityLog.ActivityLogDate = Convert.ToDateTime(hiringActivityRecord.CreatedDate);
                            insertRequirementActivityLog.CreateDate = Convert.ToDateTime(hiringActivityRecord.CreatedDate);
                            insertRequirementActivityLog.LastUpdated = DateTime.Now;

                            insertRequirementActivityLog.ModifiedById = 0;
                            insertRequirementActivityLog.CreatedById = 0;

                            insertRequirementActivityLog.Detail = hiringActivityRecord.Outcome;

                            //Handle the RequirementActivityLogTypeId:


                            //Handle Xdata:
                            //insertRequirementActivityLog = LookupValue.ParseHiringActivityXML(hiringActivityRecord.Xdata, insertRequirementActivityLog);
                            /*
                            1	Account Manager Candidate Approval
                            2	Account Manager Candidate Decline
                            3	Account Manager Client Submittal
                            4	Client Phone Interview
                            5	Client Manager Interview
                            6	Client Offer
                            7	Client Decline
                            8	Client Follow Up
                            9	Candidate Follow Up
                            10	Engagement Approved/Accepted
                            11	Permanent Placement
                            12	Pass
                            13	Close
                            #1#
                            switch (hiringActivityRecord.EventType.ToLower())
                            {
                                case "submittal":
                                    insertRequirementActivityLog.RequirementActivityLogTypeId = 3;
                                    break;  
                                case "contract placement":
                                    insertRequirementActivityLog.RequirementActivityLogTypeId = 10;
                                    break;
                                case "fte offer":
                                    insertRequirementActivityLog.RequirementActivityLogTypeId = 17;
                                    break;
                                case "manager interview":
                                    insertRequirementActivityLog.RequirementActivityLogTypeId = 5;
                                    break;
                                case "phone interview":
                                    insertRequirementActivityLog.RequirementActivityLogTypeId = 4;
                                    break;
                                case "tech interview":
                                    insertRequirementActivityLog.RequirementActivityLogTypeId = 15;
                                    break;
                                case "solutia manager interview":
                                    insertRequirementActivityLog.RequirementActivityLogTypeId = 16;
                                    break;
                                case "applied for job":
                                    insertRequirementActivityLog.RequirementActivityLogTypeId = 17;
                                    break;
                                case "candidate":
                                    insertRequirementActivityLog.RequirementActivityLogTypeId = 18;
                                    break;
                                case "recruiter interview":
                                    insertRequirementActivityLog.RequirementActivityLogTypeId = 19;
                                    break;
                                case "client manager interview":
                                    insertRequirementActivityLog.RequirementActivityLogTypeId = 5;
                                    break;
                                case "solutia phone interview":
                                    insertRequirementActivityLog.RequirementActivityLogTypeId = 20;
                                    break;
                                case "client phone interview":
                                    insertRequirementActivityLog.RequirementActivityLogTypeId = 4;
                                    break;
                                case "pass":
                                    insertRequirementActivityLog.RequirementActivityLogTypeId = 12;
                                    break;
                                case "reference check":
                                    insertRequirementActivityLog.RequirementActivityLogTypeId = 21;
                                    break;
                                case "offer accepted":
                                    insertRequirementActivityLog.RequirementActivityLogTypeId = 22;
                                    break;
                                case "future candidate":
                                    insertRequirementActivityLog.RequirementActivityLogTypeId = 18;
                                    break;
                                case "placement":
                                    insertRequirementActivityLog.RequirementActivityLogTypeId = 11;
                                    break;
                                //Offer declined?
                            }
                        

                            _db.RequirementActivityLogs.Add(insertRequirementActivityLog);
                            _db.SaveChanges();
                        //}
                        
                    }
*/

/*
                    /****************** End the HiringActivity insert **************************#1#


                                ///////////////////////////  Job File Inserts ////////////////////////////////////
                                /****************** Start the Job insert **************************#1#
                                foreach (var jobRecord in jobData)
                                {
                                    Requirement insertRequirement = new Requirement();
                                    //insertRequirement.CreateDate = Convert.ToDateTime(jobRecord.CreatedDate);

                      
                     
                                    //Used the SP GetJobPersonId to find the Customers/Candidates Id in our DB based on the MPLOY_ContactID
                                    var returnedPersonId = _db.GetJobPersonId(jobRecord.IdContactManager).ToList();
                                    insertRequirement.CustomerId = Convert.ToInt32(returnedPersonId[0]);
                                    //insertRequirement.CustomerId = Convert.ToInt32(returnedPersonId[0]);
                      
                      
                     
                     
                                    //Only insert the Jobs that have a contact associated to them (needs to include both Candidate and Customer tables)
                                    //the Person table needs to be updated with the ContactId to fix this.
                                    if (Convert.ToInt32(returnedPersonId[0]) == 0)
                                    {
                                        Debug.WriteLine("\n" + "IdContactManager: " + jobRecord.IdContactManager + " & JobId: " + jobRecord.IdJob + "");
                                        continue;
                                    }

                                    //Because travelRequired can't be null, I am defaulting it to false, and then changing it later as needed.
                                    insertRequirement.TravelRequired = false;
                                    //Get the OrganizationId associated to each Job:
                                    var returnedOrgId = _db.GetCompanyJobAssociation(jobRecord.MPLOY_CompanyId).ToList();
                                    
                                    //Add in a catch here for if returnedOrgId == 0...
                                    if (Convert.ToInt32(returnedOrgId[0]) != 0)
                                    {
                                        insertRequirement.CompanyId = Convert.ToInt32(returnedOrgId[0]);
                                    }
                                    

                                    insertRequirement.MPLOY_JobId = jobRecord.IdJob;
                                    insertRequirement.MPLOY_ContactId = jobRecord.IdContactManager;
                                    insertRequirement.Title = jobRecord.PositionTitle;

                                    insertRequirement.VMSField = jobRecord.RequirementNumber;
                        
                                    //Handle the Address info:
                                    //I am not sure what this address should be associated to
                                    //I looked into it and it seems that the addresses are already stored in the MailingAddress table under the Companies Id.


                                    if (jobRecord.StartDate == "ASAP" || jobRecord.StartDate == "asap" || jobRecord.StartDate == "Immediate")
                                    {
                                        insertRequirement.DesiredStartDate = Convert.ToDateTime(jobRecord.CreatedDate);
                                    } 
                                    //else if (jobRecord.StartDate == "TBD")
                                    //{
                            
                                    //}
                                    //else if (jobRecord.StartDate != null)
                                    //{
                                    //    insertRequirement.DesiredStartDate = Convert.ToDateTime(jobRecord.StartDate);
                                    //}
                        
                       

                                    if (jobRecord.Description != null)
                                    {

                                        insertRequirement.Description = jobRecord.Description;
                                    }
                                    else
                                    {
                                        insertRequirement.Description = "None Available";
                                    }
                        
                                    //insertRequirement.RequirementProjectTypeId = //Get the id from the RequirementType table

                                    //Handle Xdata:
                                    insertRequirement = LookupValue.ParseJobXML(jobRecord.Xdata, insertRequirement);

                                    insertRequirement.CreateDate = jobRecord.CreatedDate != null ? Convert.ToDateTime(jobRecord.CreatedDate) : DateTime.Now;
                                    insertRequirement.LastUpdated = DateTime.Now;
                                    insertRequirement.PostingDate = Convert.ToDateTime(jobRecord.CreatedDate);

                                    switch (jobRecord.Priority)
                                    {
                                        case "1":
                                            insertRequirement.RequirementPriorityId = 1;
                                            break;
                                        case "2":
                                            insertRequirement.RequirementPriorityId = 2;
                                            break;
                                        case "3":
                                        case "4":
                                        case "5":
                                            insertRequirement.RequirementPriorityId = 3;
                                            break;
                                        default:
                                            insertRequirement.RequirementPriorityId = 0;
                                            break;
                                    }

                                    switch (jobRecord.JobType)
                                    {
                                        case "Permanent":
                                            insertRequirement.RequirementTypeId = 1;
                                            break;
                                        case "Contract":
                                            insertRequirement.RequirementTypeId = 2;
                                            break;
                                        case "Contract to Hire":
                                            insertRequirement.RequirementTypeId = 3;
                                            break;
                                        case "Contract or Permanent":
                                            insertRequirement.RequirementTypeId = 4;
                                            break;
                                        default:
                                            insertRequirement.RequirementTypeId = 0;
                                            break;
                                    }
                                    insertRequirement.IsActive = true;
                                    //FIX LATER:
                                    //insertRequirement.OwnerId = 22269; //Local version
                                    insertRequirement.OwnerId = 19495; //Dev site
                        
                                    _db.Requirements.Add(insertRequirement);
                                    _db.SaveChanges();
                                }
                                /****************** End the Job insert **************************#1#
*/




                            ///////////////////////////  ContactLog File Inserts ////////////////////////////////////
                            /****************** Start the ContactLog insert ***************************/
                            //foreach (var contactLogRecord in contactLogData)
                            //{
                            //    //if (contactLogRecord.IdContact == 53)
                            //    //{
                            //    ActivityLog insertActivityLog = new ActivityLog();

                            //    var returnedId = _db.GetContactLogAssociation(contactLogRecord.IdContact).ToList();
                            //    insertActivityLog.PersonId = Convert.ToInt32(returnedId[0]);
                            //    //Skip the contacts from Mploy that are cadidates for now. Candidate records do not have the needed Mploy Ids associaed to them yet in our db.
                            //    //I need to rerun the Candidate insert to do this and I a waiting b/c it takes so long.
                            //    if (insertActivityLog.PersonId == 0)
                            //    {
                            //        continue;                          
                            //    }
                            //    //insertActivityLog.ActivityLogTypeId =;
                            //    insertActivityLog.ActivityLogDate = Convert.ToDateTime(contactLogRecord.CreatedDate);
                            //    insertActivityLog.CreateDate = Convert.ToDateTime(contactLogRecord.CreatedDate);
                            //    insertActivityLog.LastUpdated = DateTime.Now;
                            //    //Insert the contactLog notes and if the notes column is null, then insert "None Available"
                            //    insertActivityLog.Notes = contactLogRecord.Note ?? "None Available";

                            //    /*
                            //     * ActivityLogType Id & Description:
                            //    1.	Sent Email
                            //    2.	Received Email 
                            //    3.	Spoke with
                            //    4.	Sent Linkedin InMail
                            //    5.	Received LinkedIn InMail
                            //    6.	Left VoiceMail
                            //    7.	Status Changed
                            //    8.	Received Voicemail   
                            //    9	Schedule Call
                            //    10.	Other Log Entry
                            //    11.	Met With
                            //    12.	Sent InMail
                            //    */

                            //    switch (contactLogRecord.LogType)
                            //    {
                            //        case "Sent Email":
                            //            insertActivityLog.ActivityLogTypeId = 1;
                            //            break;
                            //        case "Left Voicemail":
                            //            insertActivityLog.ActivityLogTypeId = 6;
                            //            break;
                            //        case "Received Email":
                            //            insertActivityLog.ActivityLogTypeId = 2;
                            //            break;
                            //        case "Schedule Call":
                            //            insertActivityLog.ActivityLogTypeId = 9;
                            //            break;
                            //        case "Spoke With":
                            //            insertActivityLog.ActivityLogTypeId = 3;
                            //            break;
                            //        case "Status Changed":
                            //            insertActivityLog.ActivityLogTypeId = 7;
                            //            break;
                            //        case "Other Log Entry":
                            //            insertActivityLog.ActivityLogTypeId = 10;
                            //            break;
                            //        case "Met With":
                            //            insertActivityLog.ActivityLogTypeId = 11;
                            //            break;
                            //        case "Sent InMail":
                            //            insertActivityLog.ActivityLogTypeId = 12;
                            //            break;
                            //        default:
                            //            insertActivityLog.ActivityLogTypeId = 10;
                            //            break;
                            //    }

                            //   /* switch (contactLogRecord.IdUser)
                            //    {
                            //        case 5:
                            //            insertActivityLog.ModifiedById = 24;
                            //            break;
                            //        case 6:
                            //            insertActivityLog.ModifiedById = 

                            //    }*/
                            //    insertActivityLog.ModifiedById = 1;

                            //    insertActivityLog.IsActive = true;

                            //    _db.ActivityLogs.Add(insertActivityLog);
                            //    _db.SaveChanges();
                            //    //}

                            //}
                            /****************** End the ContactLog insert ***************************/


                  
                }
                catch
                (Exception ex)
                {                 
                    new LogWriterFactory().Create().Write(ex.Expand("Error occured with PersonId: " + _personId + ", and MailingAddressId: " + _mailingAddressId));
                }
            }
            
        }
    }
}
