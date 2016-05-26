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
        private static int _candidateId = -1;
        private static int _customerId = -1;

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

                        //var requirementCity = String.Empty;
                        //var requirementState = String.Empty;
                        //if (!string.IsNullOrEmpty(requirementRecord.City))
                        //{
                            
                        //}

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
                            , requirementRecord.City
                            , requirementRecord.State
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


                        //*********************** Create the RequirementCustomer records ****************
                        //The idContact on the Requirement should be associated to a customer. Create that requirementCustomer association here.
                        //It will be updated during the HiringActivity import if there is a newer association from teh hiring activities.

                        var returnedIds = _db.GetRequirementContactAssociation(requirementRecord.idJob,
                            requirementRecord.idContactManager);

                        //Find the customeid in intersect that matches the mploy contact                   
                        var errorString = string.Empty;
                        var personId = -1;
                        foreach (var result in returnedIds)
                        {
                            _requirementId = Convert.ToInt32(result.jobId);
                            _candidateId = Convert.ToInt32(result.candidateId);
                            _customerId = Convert.ToInt32(result.customerId);
                            personId = Convert.ToInt32(result.personId);
                            errorString = Convert.ToString(result.errorString);
                        }
                        //***** This RequirementCustomer Relationship is populated during the Requirement import process *****
                        //Insert the new record into the RequirementCustomer table if the _customerId is not null:
                        if (_customerId != 0)
                        {
                            var requirementCustomer = _db.InsertRequirementCustomer(_requirementId, _customerId, true, 0, requirementRecord.Created, requirementRecord.iduser);
                        }


                        Debug.WriteLine("\n" + "Requirement imported: " + MPLOYIdJob + " : " + Name + " CustomerId: " + _customerId);
                    }
                }
                catch
                (Exception ex)
                {
                    new LogWriterFactory().Create().Write(ex.Expand("Error occured with RequirementId: " + _requirementId));
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
}
