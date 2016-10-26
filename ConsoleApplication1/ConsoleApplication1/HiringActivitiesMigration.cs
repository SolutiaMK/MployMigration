using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Schema;
using ConsoleApplication1;
using ConsoleApplication1.Helper;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace ConsoleApplication1
{
    internal class HiringActivitiesMigration
    {
        private static Entities _db;
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

                    //ALL JobFlow Activities are being considered RequirementCandidate entities!

                    //Read in the Hiring Aatvity records from the Mploy tables:
                    var hiringActivityData = _db.ReadtbJobFlow().ToList();

                    //For each record, transform the data and insert into our DB:
                    foreach (var activityRecord in hiringActivityData) //.Where(x => x.idJob == 3092)
                    {
                        //Do Work:

                        var returnedIds = _db.GetHiringActivityAssociation(activityRecord.idJob,
                                activityRecord.IdContact);

                        //Get the contact's Intersect Ids:                   
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

                            //If the current record is dealing with a Candidate:
                            if (_candidateId != 0 && _requirementId != 0)
                            {
                                //Entity type, RequirementCandidate = 5:
                                var entityTypeId = 5;
                                //Get the activity id for the job flow record based on the event type id on the mploy table:
                                var activityId = GetActivityId(activityRecord.idEventType, entityTypeId);                           

                                var getGlobalEntityIdResult = _db.GetGlobalEntityIdForEntity(_requirementId, _candidateId, null).ToList();

                                Guid? globalEntityId = getGlobalEntityIdResult[0];

                                //Only append the "Note:" & "Outcome:" when that filed has data.
                                //If the Note and Outcome fields are both blank, do not insert.
                                //if (!string.IsNullOrEmpty(activityRecord.Note) || !string.IsNullOrEmpty(activityRecord.Outcome))
                                //{
                                //    //Create the note to insert.  Take the Note and outcome fields from mploy and append them to create the whole note to insert into intersect:
                                //    var noteAndOutcomeString = "Note: " + activityRecord.Note + " Outcome: " +
                                //                               activityRecord.Outcome;

                                    //If the returned activity type id is NOT -1, then insert into the activityLog table. If it IS -1, then don't insert because something is wrong.
                                    if (activityId != -1)
                                    {
                                        //Insert the ActivityLog record here:
                                        var insertedActivityLogId = _db.InsertActivityLog(globalEntityId, activityId, activityRecord.Created,
                                            null, null, activityRecord.idUser);

                                        Debug.WriteLine("\n" + "Requirement Id: " + _requirementId + " Candidate Id: " + _candidateId + " Global Entity Id: " + globalEntityId);
                                    }
                                    else
                                    {
                                        Debug.WriteLine("\n" + "***** Following activity was not inserted *****" + "\n" +"  -> Job FLow Id: " + activityRecord.idJobFlow + " Id Event Type: " + activityRecord.idEventType);
                                    }                                  
                                //}
                            }                           
                        }

                    }
                }
                catch
                    (Exception ex)
                {
                    new LogWriterFactory().Create()
                        .Write(ex.Expand("Error occured with entry: " + _candidateId + ", " + _requirementId));
                }

            }
        }

        //Get the activity type id for the activity log table based on the eventType from the Mploy.tbJobFlow table.
        static int GetActivityId(int eventTypeId, int entityTypeId)
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
            int activityTypeId = -1;

            switch (eventTypeId)
            {
                case 70:
                    activityTypeId = 50;
                    break;
                case 80:
                    activityTypeId = 51;
                    break;
                case 60:
                    activityTypeId = 7;
                    break;
                case 100:
                    activityTypeId = 52;
                    break;
                case 110:
                    activityTypeId = 53;
                    break;
                case 120:
                    activityTypeId = 54;
                    break;
                case 50:
                    activityTypeId = 50;
                    break;
                case 40:
                    activityTypeId = 4;
                    break;
                case 55:
                    activityTypeId = 55;
                    break;
                case 90:
                    activityTypeId = 9;
                    break;
                //case 20:
                //case 30:
                //case 10:
                case 35:
                    activityTypeId = 56;
                    break;
                case 95:
                    activityTypeId = 52;
                    break;
                case 105:
                    activityTypeId = 57;
                    break;
                //case 130:
                case 140:
                    activityTypeId = 58;
                    break;
                case 85:
                    activityTypeId = 59;
                    break;
                //case 102:
                case 108:
                    activityTypeId = 60;
                    break;
                case 51:
                    activityTypeId = 51;
                    break;
                case 71:
                    activityTypeId = 50;
                    break;
                case 52:
                    activityTypeId = 10;
                    break;
                case 72:
                    activityTypeId = 61;
                    break;
            }

            //Depeding on the entityTypeId, return the associated activityTypeId:
            //switch (entityTypeId)
            //{
            //    case 1:
            //        //Find the activity types for the entity type 1:
            //        switch (eventTypeId)
            //        {
            //            case 70:
            //            case 80:
            //            case 60:
            //            case 100:
            //            case 110:
            //            case 120:
            //            case 50:
            //            case 40:
            //            case 55:
            //            case 90:
            //            case 20:
            //            case 30:
            //            case 10:
            //            case 35:
            //            case 95:
            //            case 105:
            //            case 130:
            //            case 140:
            //            case 85:
            //            case 102:
            //            case 108:
            //            case 51:
            //            case 71:
            //            case 52:
            //            case 72:
            //                break;
            //        }
            //        break;
            //    case 2:
            //        //Find the activity types for the entity type 2
            //        switch (eventTypeId)
            //        {
            //            case 70:
            //            case 80:
            //            case 60:
            //            case 100:
            //            case 110:
            //            case 120:
            //            case 50:
            //            case 40:
            //            case 55:
            //            case 90:
            //            case 20:
            //            case 30:
            //            case 10:
            //            case 35:
            //            case 95:
            //            case 105:
            //            case 130:
            //            case 140:
            //            case 85:
            //            case 102:
            //            case 108:
            //            case 51:
            //            case 71:
            //            case 52:
            //            case 72:
            //                break;
            //        }
            //        break;
            //}           
            //Return the activity type id so it can be used to insert the new ActivityLog entry.
            return activityTypeId;
        }
    }
}
