using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using ConsoleApplication1;
using ConsoleApplication1.Helper;
using Microsoft.Practices.EnterpriseLibrary.Logging;

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
                    foreach (var activityRecord in hiringActivityData)
                    {
                        //Get the Candidate and Requirement Id for the incoming MPLOY Contact and Job Id:
                        //Set the _candidateId and _requirementId here:
                        //[0] holds the JobId
                        //[1] holds the CanddateId or CustomerId
                        var returnedIds = _db.GetHiringActivityAssociation(activityRecord.idJob, activityRecord.IdContact);
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
                            Debug.WriteLine("\n" + errorString + "\n" + "***** MPLOY JobId: " + activityRecord.idJob + " MPLOY ContactId: " + activityRecord.IdContact + " MPLOY JobFlowId: " + activityRecord.idJobFlow + " *****");                        
                        }
                        else if (_candidateId == 0) //If the CandidateId is null:
                        {
                            //Handle adding the idContact from this record as a new person and Candidate first.
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

                            var mployContactResult = data.Where(d => d.IdContact == activityRecord.IdContact).ToList();
                            foreach (var item in mployContactResult)
                            {
                                //Skip Gloria Sharp:
                                if (item.IdContact != 4226)
                                {
                                    CandidateMigration.InsertAsPersonAndCandidate(item);
                                }
                                
                            }
                            
                        }
                       //else 
                        //{
                            //Continue like normal:

                            //Update the Candidate and Requirement tables:


                            //Add a record to the SalesRecruitingActivityLog and RequirementCandidate tables here:                    

                            //Get the SalesRecruitingActivityLog.SalesRecruitingWorkflowId:
                            var workflowId = GetSalesRecruitingWorkflowId(activityRecord.idEventType);
                            //Find the RequirementCandidateStatusTypeId for the RequirementCandidate table
                            var requirementCandidateStatusTypeId = GetRequirementCandidateStatusTypeId();
                            //Find the RequirementStatusTypeId for the Requirement table
                            var requirementStatusTypeId = GetRequirementCandidateStatusTypeId();
                            //Find the updated CandidateStatusTypeId to update the CandidateStatusTypeId in the Candidate table 
                            var candidateStatusTypeId = GetRequirementCandidateStatusTypeId();
                           
                            //Insert into the SalesRecruitingActivityLog:
                            
                            //Insert a new record in the RequirementCandidate table for the current requirement and Candidate
                            
                            //Update the Requirement.RequirementStatusTypeId:
                            
                            //Update the Candidate.CandidateStatusTypeId:

                        //}
                        Debug.WriteLine("\n" + "Requirement Id: " + _requirementId + " Mploy Job Id: " + activityRecord.idJob);                    
                    }
                }
                 catch
                (Exception ex)
                 {
                     new LogWriterFactory().Create().Write(ex.Expand("Error occured with entry: " + _candidateId + ", " + _requirementId ));
                 }
             
            }
        }

        static int GetSalesRecruitingWorkflowId(int eventTypeId)
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
            var workflowId = -1;

            switch (eventTypeId)
            {
                case 70:
                    //Intersect - Recruiter Interview
                    workflowId = 19;
                    break;
                case 80:
                    //Manager Interview
                    workflowId = 20;
                    break;
                case 60:
                    //Candidate Submission
                    workflowId = 32;
                    break;
                case 100:
                    //Employment Offer Accepted
                    workflowId = 25; 
                    break;
                case 110:
                    //Requirement Resolved
                    workflowId = 43;
                    break;
                case 120:
                    //Requirement Resolved
                    workflowId = 43;
                    break;
                case 50:
                    //Recruiter Interview                    
                    workflowId = 19;
                    break;
                case 40:
                    //Conversion
                    workflowId = 44;
                    break;
                case 55:
                    //Tech Interview
                    workflowId = 21;
                    break;
                case 90:
                    //Referrence Check
                    workflowId = 22;
                    break;
                case 35:
                    //Applied For Job
                    workflowId = 17;
                    break;
                case 140:
                    //Requirement Resolved
                    workflowId = 43;
                    break;
                case 85:
                    //Conversion
                    workflowId = 44;
                    break;
                case 102:
                    //Approved Independent Contract
                    workflowId = 23;
                    break;
                case 108:
                    //Future Candidate
                    workflowId = 26;
                    break;
                case 51:
                    //Manager Interview
                    workflowId = 20;
                    break;
                case 71:
                    //Recruiter Interview 
                    workflowId = 19;
                    break;
                case 52:
                    //Client In-Person Interview
                    workflowId = 35;
                    break;
                case 72:
                    //Client Phone Interview
                    workflowId = 34;
                    break;
                case 20:
                case 30:
                case 10:
                case 95:
                case 105:
                case 130:
                    {
                        workflowId = 44;
                    }
                    break;
                default:
                    //Conversion
                    workflowId = 44;
                    break;
            }

            return workflowId;
        }
    }
}
