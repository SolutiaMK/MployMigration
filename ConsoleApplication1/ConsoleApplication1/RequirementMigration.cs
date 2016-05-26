using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Diagnostics;
using System.Linq;
using ConsoleApplication1;
using ConsoleApplication1.Helper;
using Microsoft.Practices.EnterpriseLibrary.Logging;


namespace ConsoleApplication1
{
    internal class RequirementMigration
    {

        //private static int _mailingAddressId = -1;
        //private static int _personId = -1;
        private static int _requirementId = -1;

        static Entities _db;
        //static List<KeyValuePair<int, string>> _sourceList = new List<KeyValuePair<int, string>>();


        public static void JobImport()
        {
            using (_db = new Entities())
            {
                try
                {

                    //Reads in the job record from tbJob MPLOY table:
                    var jobData = _db.ReadJob().ToList();

                    //For each job record from the job file table:
                    foreach (var requirementRecord in jobData)
                    {
                        // Sets requirement fields to job data coming from MPLOY
                        var Name = requirementRecord.PositionTitle;
                        var vMSField = requirementRecord.RequirementNumber;
                        var startDate = requirementRecord.StartDate;
                        var requirementDescription = requirementRecord.Description;
                        var requirementTypeId = requirementRecord.idJobType;
                        var requirementPriorityTypeId = requirementRecord.Priority;
                        var createDate = requirementRecord.Created;
                        var MPLOYIdUser = requirementRecord.iduser;
                        var MPLOYIdJob = requirementRecord.idJob;
                        var MPLOYIdUserRecruiter = requirementRecord.idUserRecruiter;
                        var MPLOYIdUserClosed = requirementRecord.idUserClosed;
                        var MPLOYIdUserFilled = requirementRecord.idUserFilled;

                        //These come from XData -->  Get RequirementStatusTypeId, Get IsTravelRequired, Get MinRate, Get MaxRate, Get RequirementDuration
                        Requirement requirementData = new Requirement();

                        //Set the needed variables to a default. If there is xdata for these fields, they will be reset in the ParseJobXML:
                        requirementData.IsBackgroundCheckRequired = false;
                        requirementData.IsDrugTestRequired = false;
                        requirementData.IsTravelRequired = false;
                        requirementData.IsWorkFromHome = false;

                        if (!string.IsNullOrEmpty(requirementRecord.XData))
                        {
                            requirementData = LookupValue.ParseJobXML(requirementRecord.XData, requirementData);
                        }

                        var priorityType = GetRequirementPriorityTypeId(Convert.ToInt32(requirementPriorityTypeId));

                        var reqStatusTypeId = GetRequirementStatusTypeId(requirementRecord.Closed);

                        var insertRequirement = _db.InsertRequirement(
                            Name
                            , requirementDescription
                            , vMSField
                            , requirementTypeId
                            , priorityType
                            , 3
                            , null
                            , null
                            , 3
                            , 11
                            , requirementData.PaymentTermTypeId
                            , requirementRecord.Created
                            , null
                            , null
                            , null
                            , requirementRecord.Closed
                            , requirementData.MinRate
                            , requirementData.MaxRate
                            , requirementData.Budget
                            , requirementData.Duration
                            , requirementData.ContractDetails
                            , requirementData.IsTravelRequired
                            , requirementData.IsDrugTestRequired
                            , requirementData.IsBackgroundCheckRequired
                            , requirementData.IsWorkFromHome
                            , createDate
                            , 0
                            , MPLOYIdJob
                            , requirementRecord.idContactManager
                            , MPLOYIdUserFilled
                            , MPLOYIdUserClosed
                            , MPLOYIdUserRecruiter
                            , MPLOYIdUser).ToList();

                        //Get the Requirement Info:
                        //var reqInfo = _db.FindRequirementInfo(requirementRecord.idJob).ToList();

                        foreach (var item in insertRequirement)
                        {
                            _requirementId = item.Id;
                        }


                        if (!string.IsNullOrEmpty(requirementRecord.InternalNote))
                        {
                            var requirementNoteId = _db.InsertRequirementNote(_requirementId, requirementRecord.InternalNote, createDate, 0);
                        }
                        //Store the Closed Reason from Mploy as a RequirementNote:
                        if (!string.IsNullOrEmpty(requirementRecord.ClosedReason))
                        {
                            //Add a new requirement note record for the closed reason, append "Closed Reason" to the front for clarity of where the note came from in the MPLOY db:
                            var reqClosedReasonNote = "Closed Reason: " + requirementRecord.ClosedReason;
                            var requirementNoteId = _db.InsertRequirementNote(_requirementId, reqClosedReasonNote, createDate, 0);
                        }


                        Debug.WriteLine("\n" + "Requirement imported: " + MPLOYIdJob + " : " + Name);
                    }
                }
                catch
                (Exception ex)
                {
                    new LogWriterFactory().Create().Write(ex.Expand("Error occured with PersonId: " + _requirementId));
                }

            }
        }

        static int GetRequirementStatusTypeId(DateTime? closedDate)
        {
            var statusId = -1;
            //If the Mploy closed date has a value and is not null, then set the status to closed
            if (closedDate != DateTime.MinValue && closedDate.HasValue)
            {
                statusId = 11;
            }
            else
            {
                statusId = 1; //Open
            }
            return statusId;
        }

        static int GetRequirementPriorityTypeId(int mployJobPriorityType)
        {
            int reqPriorityTypeId = -1;

            switch (mployJobPriorityType)
            {
                case 1:
                    reqPriorityTypeId = 3;
                    break;
                case 2:
                    reqPriorityTypeId = 2;
                    break;
                case 3:
                    reqPriorityTypeId = 1;
                    break;
                default:
                    reqPriorityTypeId = 2;
                    break;
            }
            return reqPriorityTypeId;
        }
    }


    //    public static int GetCandidateStatusTypeId(int statusId)
    //    {
    //        int candidateStatusTypeId = -1;

    //        switch (statusId)
    //        {
    //            case 1:
    //                candidateStatusTypeId = 1;
    //                break;
    //            case 2:
    //                candidateStatusTypeId = 4;
    //                break;
    //            case 4:
    //                candidateStatusTypeId = 2;
    //                break;
    //            default:
    //                candidateStatusTypeId = 3;
    //                break;
    //        }
    //        return candidateStatusTypeId;
    //    }

    //    //Inserts the ContactInformation for the given personId and contact info:
    //    static int InsertContactInformation(string contactData, int contactDataType)
    //    {

    //        //Email is inserted with the Person

    //        var contactInfoTypeId = 0;
    //        var personContactInformationId = 0;
    //        //Call AssignHandleTextType() for each Mploy handleType column (0-3)                                        
    //        //0:
    //        if (contactDataType != 0 && contactDataType != 3)
    //        {
    //            //Calls a method to find the ContactTypeId of the handleText from the Candidate file, and inserts it into the ContactInformation table in the DB.
    //            contactInfoTypeId = LookupValue.AssignHandleTextType(contactDataType, contactData);
    //            List<int?> result;
    //            switch (contactInfoTypeId)
    //            {
    //                case 1:
    //                case 5:
    //                case 6:
    //                case 14:
    //                    {
    //                        result = _db.InsertPersonContactInformation(_personId, contactInfoTypeId, contactData, true, false, 0).ToList();
    //                    }
    //                    break;
    //                case 2:
    //                    {
    //                        result = _db.InsertPersonContactInformation(_personId, contactInfoTypeId, contactData, false, true, 0).ToList();
    //                    }
    //                    break;
    //                default:
    //                    result = _db.InsertPersonContactInformation(_personId, contactInfoTypeId, contactData, false, false, 0).ToList();
    //                    break;
    //            }

    //            personContactInformationId = Convert.ToInt32(result[0]);

    //        }

    //        return personContactInformationId;
    //    }



    //    static void InsertMailingAddress(ConsoleApplication1.Model.Person record)
    //    {
    //        //I changed the CreateDate & LastUpdated columns to be of type DateTime2. I need to look into this...
    //        //Do same for the MailingAddress table
    //        MailingAddress insertMailingAddress = new MailingAddress();

    //        //Check the Status column from the Candidate file.  If Status == 'Active', then IsActive = true, else IsActive = false.
    //        //insertPerson.IsActive = (record.Status == "Active") ? true : false;

    //        insertMailingAddress.Line1 = (string.IsNullOrEmpty(record.Address))
    //            ? "Not Available"
    //            : record.Address;
    //        //record.idcontact.ToString();


    //        insertMailingAddress.City = (string.IsNullOrEmpty(record.City))
    //            ? "Not Available"
    //            : record.City;
    //        insertMailingAddress.State = (string.IsNullOrEmpty(record.State))
    //            ? "NA"
    //            : record.State;
    //        insertMailingAddress.Zip = (string.IsNullOrEmpty(record.Zip))
    //            ? "Not Available"
    //            : record.Zip;


    //        insertMailingAddress.CreateDate = System.DateTime.Now;
    //        insertMailingAddress.LastUpdated = System.DateTime.Now;

    //        //Add to the MailingAddress table
    //        _db.MailingAddresses.Add(insertMailingAddress);

    //        _db.SaveChanges();
    //        //Get the mailingAddress's Id
    //        _mailingAddressId = insertMailingAddress.Id;
    //    }

    //    static void InsertPersonMailAddress(ConsoleApplication1.Model.Person record)
    //    {
    //        //Take the ids and insert into the PersonMailAddress table
    //        PersonMailAddress insertPersonMailAddress = new PersonMailAddress();
    //        insertPersonMailAddress.PersonId = _personId;
    //        insertPersonMailAddress.MailingAddressId = _mailingAddressId;

    //        //PersonMailAddress -> MailAddressType: 1 = home, 2 = office
    //        //Not null field, I am defaulting it to 1 for now.
    //        insertPersonMailAddress.MailingAddressTypeId = 1;
    //        insertPersonMailAddress.CreateDate = DateTime.Now;
    //        insertPersonMailAddress.LastUpdated = DateTime.Now;

    //        //Add the new PersonMailAddress to the db and save the changes
    //        _db.PersonMailAddresses.Add(insertPersonMailAddress);
    //        _db.SaveChanges();
    //    }
    //}
}
