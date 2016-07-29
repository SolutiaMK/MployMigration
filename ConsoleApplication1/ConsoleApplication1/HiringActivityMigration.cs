using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Schema;
using ConsoleApplication1;
using ConsoleApplication1.Helper;
using Microsoft.Practices.EnterpriseLibrary.Logging;

/****
 * The WorkflowStateLog table has a trigger to automatically update the end time stamp when an entity is inserted that already has records in the table.
 * This trigger needs to be turned off while this section of the migration runs so that the historical dates can be entered appropriately.
 ****/
namespace ConsoleApplication1
{
    class HiringActivityMigration
    {

        static Entities _db;
        private static int _candidateId = -1;
        private static int _requirementId = -1;
        private static int _customerId = -1;
        private static int _personId = -1;

        public static void ImportHiringActivity()
        {
            using (_db = new Entities())
            {
                 try
                {
                    //Read in the Hiring Aatvity records from the Mploy tables:
                    var hiringActivityData = _db.ReadtbJobFlow().ToList();

                    //For each record, transform the data and insert into our DB:
                    foreach (var activityRecord in hiringActivityData)//.Where(x => x.idJob == 3092)
                    {
                        //TESTING:
                        //if (activityRecord.idJobFlow == 1008)
                        //{
                            //Get the Candidate and Requirement Id for the incoming MPLOY Contact and Job Id:
                            //Set the _candidateId and _requirementId here:
                            var returnedIds = _db.GetHiringActivityAssociation(activityRecord.idJob,
                                activityRecord.IdContact);

                            //If the Customer Id returned is not null, then create a record in the Person table and the Candidate table for that MPLOY_contactId
                            //Skip Gloria Sharp: MPLOY_ContactId = 4226                        
                            var errorString = string.Empty;
                            foreach (var result in returnedIds)
                            {
                                _requirementId = Convert.ToInt32(result.jobId);
                                _candidateId = Convert.ToInt32(result.candidateId);
                                _customerId = Convert.ToInt32(result.customerId);
                                _personId = Convert.ToInt32(result.personId);
                                errorString = Convert.ToString(result.errorString);
                            }

                            //If the error string is null or empty, then continue. Else, there is an orphaned record:
                            if (!string.IsNullOrEmpty(errorString))
                            {
                                //Print out orphaned record info:
                                Debug.WriteLine("\n" + errorString + "\n" + "***** MPLOY JobId: " + activityRecord.idJob +
                                                " MPLOY ContactId: " + activityRecord.IdContact + " MPLOY JobFlowId: " +
                                                activityRecord.idJobFlow + " *****");
                            }                        
                            else 
                            {
                                //Continue like normal:                                               

                                //***** Find the ids needed to insert into the WorkflowStateLog and RequirementCandidate tables, and to update the Candidate and Requirement tables *****

                                //***** Insert into the WorkflowStateLog and RequirementCandidate tables *****

                                var activityNote = activityRecord.Note;
                                if (string.IsNullOrEmpty(activityRecord.Note))
                                {
                                    activityNote = " ";
                                }

                                //********** Update or create the RequirementCandidate/Customer relationships based on the incoming hiring activity  *****

                                //UPDATE: find the workflow type for all the different options and then run a seperate insert for each one.  That way I can do one pass, but add all of the needed workflowStateLog entries I need.

                                //Insert a new record in the RequirementCandidate table for the current requirement and Candidate
                                if (_candidateId != 0)
                                {
                                    var requirementCandidateId = _db.InsertRequirementCandidate(_requirementId, _candidateId, 0, activityRecord.Created, activityRecord.idUser);
                                }

                                //***** This RequirementCustomer Relationship is populated during the Requirement import process *****
                                //Insert the new record into the RequirementCustomer table if the _customerId if not null:
                                if (_customerId != 0)
                                {
                                    var requirementCustomer = _db.InsertRequirementCustomer(_requirementId, _customerId, true, 0, activityRecord.Created, activityRecord.idUser);
                                    Debug.WriteLine("\n" + "Customer data:" + "\n" + "***** MPLOY JobId: " + activityRecord.idJob +
                                        " MPLOY ContactId: " + activityRecord.IdContact + " MPLOY JobFlowId: " +
                                        activityRecord.idJobFlow + " *****");
                                }

                                //Get the Candidate Process Type Id from the Mploy eventType
                                var workflowTypeIdCandidateProcessId =
                                    GetCandidateProcessTypeId(activityRecord.idEventType);
                                //Get the Requirement Process Type Id from the Mploy eventType
                                var workflowTypeIdReqProcessId = GetRequirementProcessWorkflowTypeId(activityRecord.idEventType);
                                //Get the Requirement Fulfillment Process Id from the Mploy eventType
                                var workflowTypeIdReqFulfillProcessId =
                                    GetRequirementFulfillmentProcessWorkflowTypwId(activityRecord.idEventType);
                                
                                //Get the Workflow state id based on the above process given the mploy event type id:
                                var candidateWorkflowStateId = GetStateId(activityRecord.idEventType, workflowTypeIdCandidateProcessId);

                                var requirementWorkflowStateId = GetStateId(activityRecord.idEventType, workflowTypeIdReqProcessId);

                                var reqFulfillmentStateId = GetStateId(activityRecord.idEventType, workflowTypeIdReqFulfillProcessId);

                                //State Sub Id:
                                var candidateSubId = GetWorkflowStateSubId(candidateWorkflowStateId, activityRecord.idEventType);
                                
                                var requirementSubId = GetWorkflowStateSubId(requirementWorkflowStateId,
                                    activityRecord.idEventType);
                               
                                var reqFulfillmentSubId = GetWorkflowStateSubId(reqFulfillmentStateId,
                                    activityRecord.idEventType);

                                int? reasonCode = null;

                                //Insert the workflow for each process if it exists for the record:


                                //Get the WorkflowState and SubState ids:
                                //var workflowStateId = GetWorkflowStateId(activityRecord.idEventType);
                                //var workflowStateSubId = GetWorkflowStateSubId(workflowStateId, activityRecord.idEventType);
                                
                                //var workflowTypeId = GetWorkflowTypeId(workflowStateId);

                                    //Insert the WorkflowStateLog record: -> this is only dealing with the Candidates, Requirements, and RequirementCandidates.
                                if (_candidateId != 0)
                                {

                                    if (workflowTypeIdCandidateProcessId != null)
                                    {
                                        var workflowStateLogCandidate =
                                            _db.InsertWorkflowStateLog(workflowTypeIdCandidateProcessId,
                                                candidateWorkflowStateId, candidateSubId, reasonCode,
                                                activityRecord.Created, null, 0, activityRecord.idJobFlow, _candidateId,
                                                _requirementId, activityRecord.idUser);
                                        Debug.WriteLine("\n" + "Requirement Id: " + _requirementId + " Mploy Job Id: " + activityRecord.idJob + " Mploy idJobFlow: " + activityRecord.idJobFlow);
                                    }

                                    if (workflowTypeIdReqProcessId != null)
                                    {
                                        var workflowStateLogRequirement =
                                            _db.InsertWorkflowStateLog(workflowTypeIdReqProcessId,
                                                requirementWorkflowStateId, requirementSubId, reasonCode,
                                                activityRecord.Created, null, 0, activityRecord.idJobFlow, _candidateId,
                                                _requirementId, activityRecord.idUser);
                                        Debug.WriteLine("\n" + "Requirement Id: " + _requirementId + " Mploy Job Id: " + activityRecord.idJob + " Mploy idJobFlow: " + activityRecord.idJobFlow);
                                    }

                                    if (workflowTypeIdReqFulfillProcessId != null)
                                    {
                                        var workflowStateLogReqFulfillment =
                                            _db.InsertWorkflowStateLog(workflowTypeIdReqFulfillProcessId,
                                                reqFulfillmentStateId, reqFulfillmentSubId, reasonCode,
                                                activityRecord.Created, null, 0, activityRecord.idJobFlow, _candidateId,
                                                _requirementId, activityRecord.idUser);
                                        Debug.WriteLine("\n" + "Requirement Id: " + _requirementId + " Mploy Job Id: " + activityRecord.idJob + " Mploy idJobFlow: " + activityRecord.idJobFlow);
                                    }
                                    //var workflowStateLog = _db.InsertWorkflowStateLog(workflowTypeId, workflowStateId, workflowStateSubId, reasonCode, activityRecord.Created, null, 0, activityRecord.idJobFlow, _candidateId, _requirementId, activityRecord.idUser);
                                    //Debug.WriteLine("\n" + "Requirement Id: " + _requirementId + " Mploy Job Id: " + activityRecord.idJob + " Mploy idJobFlow: " + activityRecord.idJobFlow);
                                }                                                                    
                           }                            
                    }
                }
                 catch
                (Exception ex)
                 {
                     new LogWriterFactory().Create().Write(ex.Expand("Error occured with entry: " + _candidateId + ", " + _requirementId ));
                 }
             
            }
        }

        static int? GetCandidateProcessTypeId(int eventTypeId)
        {
            /*****
             *      Id  EventType -> FROM MPLOY!
                    70	Phone Interview
                    80	Manager Interview
                    60	Submittal
                    100	Offer Accepted
                    110	Placement
                    120	Contract Placement
                    50	Recruiter Interview
                    40	Candidate
                    55	Tech Interview 
                    90	Reference Check
                    20	Contact Created
                    30	Job Created
                    10	Organization Created
                    35	Applied For Job
                    95	FTE Offer
                    105	Offer Declined
                    130	Job Closed
                    140	Contract Extended
                    85	Pass
                    102	Approved I.C.
                    108	Future Candidate
                    51	Solutia Manager Interview
                    71	Solutia Phone Interview
                    52	Client Manager Interview
                    72	Client Phone Interview
            *****/
            int? workflowTypeId = null;


            switch (eventTypeId)
            {
                case 70:
                case 80:
                case 60:
                case 100:
                case 110:
                case 120:
                case 50:
                case 40:
                case 55:
                case 90:
                case 35:
                case 95:
                case 105:
                case 140:
                case 85:
                case 102:
                case 108:
                case 51:
                case 71:
                case 52:
                case 72:
                case 20:
                {
                    workflowTypeId = 2;
                    break;
                }
            }

            return workflowTypeId;
        }

        static int? GetRequirementProcessWorkflowTypeId(int eventTypeId)
        {
            int? workflowTypeId = null;

            switch (eventTypeId)
            {

                case 110:
                    workflowTypeId = 1;
                    break;
                case 30:
                    workflowTypeId = 1;
                    break;
            }

            return workflowTypeId;
        }

        static int? GetRequirementFulfillmentProcessWorkflowTypwId(int eventTypeId)
        {
            int? workflowTypeId = null;

            switch (eventTypeId)
            {
                case 70:
                case 80:
                case 60:
                case 110:
                case 120:
                case 50:
                case 40:
                case 55:
                case 90:
                case 35:
                case 95:
                case 105:
                case 130:
                case 140:
                case 85:
                case 108:
                case 51:
                case 71:
                case 52:
                case 72:
                {
                    workflowTypeId = 3;
                    break;
                }
            }

            return workflowTypeId;
        }

        static int? GetStateId(int eventTypeId, int? workflowTypeId)
        {
            //WorkflowTypeId 1 = Requirement Process, EntityTypeId 3
            //WorkflowTypeId 2 = Candidate Process, EntityTypeId 2
            //WorkflowTypeId 3 = Requirement Fulfillment Process, EntityTypeId 5
            int? workflowStateId = null;

            switch (eventTypeId)
            {
                case 70:
                case 80:
                case 50:
                case 90:
                case 51:
                case 71:
                {
                    switch (workflowTypeId)
                    {
                        case 2:
                            workflowStateId = 8;
                            break;
                        case 3:
                            workflowStateId = 15;
                            break;
                    }
                    break;
                }
                case 60:
                    switch (workflowTypeId)
                    {
                        case 2:
                            workflowStateId = 9;
                            break;
                        case 3:
                            workflowStateId = 18;
                            break;
                    }
                    break;
                case 100:
                    switch (workflowTypeId)
                    {
                        case 2:
                            workflowStateId = 13;
                            break;
                    }
                    break;
                case 110:
                    switch (workflowTypeId)
                    {
                        case 1:
                            workflowStateId = 24;
                            break;
                        case 2:
                            workflowStateId = 9;
                            break;
                        case 3:
                            workflowStateId = 31;
                            break;
                    }
                    break;
                case 120:
                    switch (workflowTypeId)
                    {
                        case 2:
                            workflowStateId = 9;
                            break;
                        case 3:
                            workflowStateId = 31;
                            break;
                    }
                    break;
                case 40:
                case 35:
                {
                    switch (workflowTypeId)
                    {
                        case 2:
                            workflowStateId = 7;
                            break;
                        case 3:
                            workflowStateId = 15;
                            break;
                    }
                    break;
                }
                case 55:
                    switch (workflowTypeId)
                    {
                        case 2:
                            workflowStateId = 8;
                            break;
                        case 3:
                            workflowStateId = 16;
                            break;
                    }
                    break;
                case 140:
                case 85:
                case 108:
                {
                    switch (workflowTypeId)
                    {
                        case 2:
                            workflowStateId = 9;
                            break;
                        case 3:
                            workflowStateId = 31;
                            break;
                    }
                    break;
                }
                case 102:
                    switch (workflowTypeId)
                    {
                        case 2:
                            workflowStateId = 9;
                            break;
                    }
                    break;

                case 52:
                case 72:
                {
                    switch (workflowTypeId)
                    {
                        case 2:
                            workflowStateId = 9;
                            break;
                        case 3:
                            workflowStateId = 19;
                            break;
                    }
                    break;
                }
                case 20:
                    switch (workflowTypeId)
                    {
                        case 2:
                            workflowStateId = 7;
                            break;
                    }
                    break;
                case 30:
                    switch (workflowTypeId)
                    {
                        case 1:
                            workflowStateId = 1;
                            break;
                    }
                    break;
                //case 10: //None with the eventType of 10 currently.  Need to revist mapping.
                case 95:
                    switch (workflowTypeId)
                    {
                        case 2:
                            workflowStateId = 12;
                            break;
                        case 3:
                            workflowStateId = 19;
                            break;
                    }
                    break;
                case 105:
                    switch (workflowTypeId)
                    {
                        case 2:
                            workflowStateId = 12;
                            break;
                        case 3:
                            workflowStateId = 31;
                            break;
                    }
                    break;
                case 130:
                    switch (workflowTypeId)
                    {
                        case 3:
                            workflowStateId = 31;
                            break;
                    }
                    break;
            }
            return workflowStateId;
        }



        //Get the WorkflowTypeId based on the WorkflowStateTypeId
        static int GetWorkflowTypeId(int workflowStateId)
        {
            var workflowTypeId = -1;
            //WorkflowTypeId 1 = Requirement Process, EntityTypeId 3
            //WorkflowTypeId 2 = Candidate Process, EntityTypeId 2
            //WorkflowTypeId 3 = Requirement Fulfillment Process, EntityTypeId 5

            switch (workflowStateId)
            {
                case 1:
                    workflowTypeId = 1;
                    break;
                case 8:
                case 13:
                case 7:
                case 12:
                case 9:
                {
                    workflowTypeId = 2;
                    break;
                }
                case 18:
                case 31:
                case 15:
                case 19:
                {
                    workflowTypeId = 3;
                    break;
                }
            }

            return workflowTypeId;
        }

        //Gets the ReasonCode for the WorkflowStateLog record if it has one:
        static int? GetReasonCodeId()
        {
            int? reasonCodeId = null;

            return reasonCodeId;
        }

        //Takes in the Mploy eventType and returns the Intersect WorkflowState Id:
        static int GetWorkflowStateId(int eventTypeId)
        {
            /*****
             *      Id  EventType -> FROM MPLOY!
                    70	Phone Interview
                    80	Manager Interview
                    60	Submittal
                    100	Offer Accepted
                    110	Placement
                    120	Contract Placement
                    50	Recruiter Interview
                    40	Candidate
                    55	Tech Interview 
                    90	Reference Check
                    20	Contact Created
                    30	Job Created
                    10	Organization Created
                    35	Applied For Job
                    95	FTE Offer
                    105	Offer Declined
                    130	Job Closed
                    140	Contract Extended
                    85	Pass
                    102	Approved I.C.
                    108	Future Candidate
                    51	Solutia Manager Interview
                    71	Solutia Phone Interview
                    52	Client Manager Interview
                    72	Client Phone Interview
            *****/
            int workflowStateId = -1;


            switch (eventTypeId)
            {
                case 70:
                    workflowStateId = 8;
                    break;
                case 80:
                    workflowStateId = 8;
                    break;
                case 60:
                    workflowStateId = 18;
                    break;
                case 100:
                    workflowStateId = 13; 
                    break;
                case 110:
                    workflowStateId = 31;
                    break;
                case 120:
                    workflowStateId = 31;
                    break;
                case 50:
                    workflowStateId = 8;
                    break;
                case 40:
                    workflowStateId = 7;
                    break;
                case 55:
                    workflowStateId = 8;
                    break;
                case 90:
                    workflowStateId = 8;
                    break;
                case 35:
                    workflowStateId = 7;
                    break;
                case 140:
                    workflowStateId = 31;
                    break;
                case 85:
                    workflowStateId = 31;
                    break;
                case 102:
                    workflowStateId = 9;
                    break;
                case 108:
                    workflowStateId = 9;
                    break;
                case 51:
                    workflowStateId = 8;
                    break;
                case 71:
                    workflowStateId = 8;
                    break;
                case 52:
                    workflowStateId = 19;
                    break;
                case 72:
                    workflowStateId = 19;
                    break;
                case 20:
                    workflowStateId = 7;
                    break;
                case 30:
                    workflowStateId = 1;
                    break;
                //case 10: //None with the eventType of 10 currently.  Need to revist mapping.
                case 95:
                    workflowStateId = 12;
                    break;
                case 105:
                    workflowStateId = 12;
                    break;
                case 130:
                    workflowStateId = 31;
                    break;
            }

            return workflowStateId;
        }

        //Takes in the Intersect WorkflowStateId and returns the WorkflowStateSubId, if one exists:
        static int? GetWorkflowStateSubId(int? workflowStateId, int eventTypeId)
        {
            int? workflowStateSubId = null;
            //Check the eventType and then the workFlowStateId to find the correct subState Id:
            switch (eventTypeId)
            {
                case 60:
                    //Check for the sub state id:
                    if (workflowStateId == 9)
                    {
                        workflowStateSubId = 13;
                    }
                    break;
                case 110:
                    if (workflowStateId == 31)
                    {
                        workflowStateSubId = 12;
                    }
                    break;
                case 120:
                    switch (workflowStateId)
                    {
                        case 31:
                            workflowStateSubId = 12;
                            break;
                        case 9:
                            workflowStateSubId = 13;
                            break;
                    }
                    break;
                case 105:
                    if (workflowStateId == 12)
                    {
                        workflowStateSubId = 14;
                    }
                    break;
                case 140:
                case 102:
                {
                    if (workflowStateId == 9)
                    {
                        workflowStateSubId = 13;
                    }
                    break;
                }
                case 85:
                    switch (workflowStateId)
                    {
                        case 31:
                            workflowStateSubId = 8;
                            break;
                        case 9:
                            workflowStateSubId = 15;
                            break;
                    }
                    break;
                case 108:
                    switch (workflowStateId)
                    {
                        case 9:
                            workflowStateSubId = 14;
                            break;
                        case 31:
                            workflowStateSubId = 8;
                            break;
                    }
                    break;
            }

            return workflowStateSubId;
        }




        //**** OLD CODE!! ****                              

        //If the hiring activity has a canceled date, the the workflow and req status is closed:
        //if (activityRecord.Canceled.HasValue)
        //{
        //    //Conversion:
        //    workflowId = 44;
        //    //Closed:
        //    requirementStatusTypeId = 11;
        //}

        //Get the SalesRecruitingActivityLog PayRate and BillRate:                            
        //var insertActivityLog = new SalesRecruitingActivityLog();
        //if (!string.IsNullOrEmpty(activityRecord.XData))
        //{
        //    //Parse the XData for the Bill and Pay rate:

        //    insertActivityLog = LookupValue.ParseHiringActivityXML(activityRecord.XData, insertActivityLog);
        //}


        //Get the SalesRecruitingActivityLog.SalesRecruitingWorkflowId:
        //var workflowId = GetSalesRecruitingWorkflowId(activityRecord.idEventType);                      

        //Find the RequirementCandidateStatusTypeId for the RequirementCandidate table:
        //var requirementCandidateStatusTypeId = GetRequirementCandidateStatusTypeId(activityRecord.idEventType);

        //Find the RequirementStatusTypeId for the Requirement table
        //var requirementStatusTypeId = GetRequirementStatusTypeId(activityRecord.idEventType);

        //Find the updated CandidateStatusTypeId to update the CandidateStatusTypeId in the Candidate table 
        //var candidateStatusTypeId = GetCandidateStatusTypeId(activityRecord.idEventType);




        //Insert into the SalesRecruitingActivityLog table:
        //var saleRecruitingActivityLogId = _db.InsertSalesRecruitingActivityLog(workflowId, activityNote, activityRecord.Outcome, insertActivityLog.PayRate, insertActivityLog.BillRate, activityRecord.Scheduled, activityRecord.enddate, activityRecord.Created, activityRecord.Created, activityRecord.IdContact, activityRecord.idUser, activityRecord.idJob);





        //***** Update the Requirement and Candidate tables to have the appropriate types based off of the latest hiring activity associated to them *****

        //Update the Requirement.RequirementStatusTypeId:
        //if (requirementStatusTypeId != -1)
        //{
        //    //Same logic as CandidateStatusTypeId, only update if the requirementStatusType is actually reset:
        //    var updatedRequirementId = _db.UpdateRequirementStatusType(activityRecord.idJob, requirementStatusTypeId);
        //}

        ////Update the Candidate.CandidateStatusTypeId:
        ////Only update if the CandidateStatus is reset in the GetCandidateStatusTypeId function, if it returns a -1 then skip resetting the CandidateStatusType:
        //if (candidateStatusTypeId != -1)
        //{                          
        //    var updatedCandidateStatusTypeId = _db.UpdateCandidateStatusType(activityRecord.IdContact, candidateStatusTypeId);
        //}                                                 

        //}

        //************* Old Req. and Candidate Status functions ************************
        //static int GetRequirementStatusTypeId(int eventTypeId)
        //{
        //    var requirementStatusTypeId = -1;
        //    //RequirementStatusType table ids:
        //    switch (eventTypeId)
        //    {
        //        case 40:
        //            //In intersect - id of 11 = closed
        //            requirementStatusTypeId = 11;
        //            break;
        //        case 85:
        //            //In intersect - id of 11 = closed
        //            requirementStatusTypeId = 11;
        //            break;
        //        case 110:
        //            requirementStatusTypeId = 4;
        //            break;
        //        case 120:
        //            requirementStatusTypeId = 4;
        //            break;
        //        case 140:
        //            requirementStatusTypeId = 7;
        //            break;
        //    }

        //    return requirementStatusTypeId;
        //}

        //static int GetRequirementCandidateStatusTypeId(int eventTypeId)
        //{
        //    var reqCandidateStatusTypeId = -1;

        //    switch (eventTypeId)
        //    {
        //        case 85:
        //            //In intersect - id of 14 = Conversion
        //            reqCandidateStatusTypeId = 14;
        //            break;
        //        case 40:
        //            reqCandidateStatusTypeId = 14;
        //            break;
        //        case 110:
        //        case 120:
        //        case 140:
        //            {
        //                //Assigned
        //                reqCandidateStatusTypeId = 1;
        //                break;
        //            }
        //        default:
        //            reqCandidateStatusTypeId = 14;
        //            break;
        //    }

        //    return reqCandidateStatusTypeId;
        //}

        //static int GetCandidateStatusTypeId(int eventTypeId)
        //{
        //    //Takes in the hiring activity log event type, and resets the candidate status if it needs to
        //    var candidateStatusTypeId = -1;

        //    switch (eventTypeId)
        //    {
        //        case 40:
        //            candidateStatusTypeId = 6;
        //            break;
        //    }
        //    //if -1 is returned, then do NOT update the CandidateStatusType in the Candidate table.
        //    return candidateStatusTypeId;
        //}
    }
}
//After the If and before the else in the main section:
//else if (_candidateId == 0) //If the CandidateId is null:
//{
//    //Handle adding the idContact from this record as a new person and Candidate first.
//    var data = _db.ReadPerson().ToList(x => new Model.Person
//    {
//        FirstName = x.FirstName,
//        LastName = x.LastName,
//        MiddleName = x.MiddleName,
//        Gender = x.Gender,
//        //Status = x.Status,
//        Address = x.Address,
//        City = x.City,
//        State = x.State,
//        Zip = x.Zip,
//        Email = x.eMail,
//        handle0type = Convert.ToInt32(x.handle0type),
//        handle0text = x.handle0text,
//        handle1type = Convert.ToInt32(x.handle1type),
//        handle1text = x.handle1text,
//        handle2type = Convert.ToInt32(x.handle2type),
//        handle2text = x.handle2text,
//        handle3type = Convert.ToInt32(x.handle3type),
//        handle3text = x.handle3text,
//        xdata = x.Xdata,
//        Notes = x.Notes,
//        ResumeText = x.ResumeText,
//        //Source = x.Source,
//        IdContact = Convert.ToInt32(x.idContact),
//        IdContactType = Convert.ToInt32(x.idContactType),
//        IdUser = Convert.ToInt32((x.idUserOwner)),
//        Source = Convert.ToInt32(x.IdSource),
//        IdStatus = x.IdStatus,
//        Created = Convert.ToDateTime(x.Created)
//    });
//    //get the record for the contactId we are looking at:
//    var mployContactResult = data.Where(d => d.IdContact == activityRecord.IdContact).ToList();
//    foreach (var item in mployContactResult)
//    {
//        //Skip Gloria Sharp:
//        if (item.IdContact != 4226)
//        {
//            //Insert as person maybe?

//            //CandidateMigration.InsertCandidate(item);
//        }

//    }

//}
//else if (_candidateId == 0){  //If the CandidateId is null:
//    Debug.WriteLine("\n" + errorString + " - No CandidateId " + "\n" + "***** MPLOY JobId: " + activityRecord.idJob +
//                    " MPLOY ContactId: " + activityRecord.IdContact + " MPLOY JobFlowId: " +
//                    activityRecord.idJobFlow + " *****");
//}