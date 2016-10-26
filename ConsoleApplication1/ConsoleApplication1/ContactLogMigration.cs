using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using ConsoleApplication1;
using ConsoleApplication1.Helper;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace ConsoleApplication1
{
    class ContactLogMigration
    {

        static Entities _db;
        private static int _mployIdLog = -1;
        private static int _candidateId = -1;
        private static int _customerId = -1;
        private static int _personId = -1;

        public static void ImportContactLog()
        {
            using (_db = new Entities())
            {
                try
                {
                               
                    //Read in the Contact Log data from the MPLOY table:
                    var contactLogData = _db.ReadContactLog().ToList();

                    //For each record, transform the data and insert into our DB:
                    foreach (var contactLogRecord in contactLogData)
                    {
                        //For each contact log record, get the needed values and insert into the SalesRecruitingActivityLog table:

                        if (contactLogRecord.idLog >= 34631)
                        {
                                     
                            _mployIdLog = contactLogRecord.idLog;


                            var returnedIds = _db.GetCandidateOrCustomer(contactLogRecord.idContact);

                            //Get the contact's Intersect Ids:                   
                            var errorString = string.Empty;
                            foreach (var result in returnedIds)
                            {                          
                                _candidateId = Convert.ToInt32(result.candidateId);
                                _customerId = Convert.ToInt32(result.customerId);
                                _personId = Convert.ToInt32(result.personId);
                                errorString = Convert.ToString(result.errorString);
                            }

                            //If the error string is null or empty, then continue. Else, there is an orphaned record:
                            if (!string.IsNullOrEmpty(errorString))
                            {
                                //Print out orphaned record info:
                                Debug.WriteLine("\n" + errorString + "\n" + "*****" +
                                                " MPLOY ContactId: " + contactLogRecord.idContact + " MPLOY Log Id: " +
                                                contactLogRecord.idLog + " *****");
                            }
                            else
                            {
                                //If the current record is dealing with a Candidate:
                                if (_candidateId != 0)
                                {
                                    //Entity type, Candidate = 2:
                                    var entityTypeId = 2;
                                    //Get the activity id for the job flow record based on the event type id on the mploy table:
                                    var activityId = GetActivityId(contactLogRecord.idLogType, entityTypeId);

                                    var getGlobalEntityIdResult = _db.GetGlobalEntityIdForEntity(null, _candidateId, null).ToList();

                                    Guid? globalEntityId = getGlobalEntityIdResult[0];

                                    //If the Note field is blank, do not insert.
                                    //if (!string.IsNullOrEmpty(contactLogRecord.Note))
                                    //{                                  
                                        //Insert the ActivityLog record here:
                                        var insertedActivityLogId = _db.InsertActivityLog(globalEntityId, activityId, contactLogRecord.created,
                                            null, null, contactLogRecord.idUser);
                                        Debug.WriteLine("\n" + " Candidate Id: " + _candidateId + " Global Entity Id: " + globalEntityId + " Mploy Contact Log Id: " + contactLogRecord.idLog);
                                    //}
                                    if (activityId == -1)
                                    {
                                        Debug.WriteLine("\n" + "***** Following activity was not inserted *****" + "\n" + "  -> Contact Log Id: " + contactLogRecord.idLog + " Id Event Type: " + contactLogRecord.idLogType);
                                    }

                                }

                                //If the current record is dealing with a Customer:
                                //if (_customerId != 0)
                                //{
                                //    //*************************** Only migrate the Candidates for now ************************


                                //    //Entity type, Customer = 1:
                                //    var entityTypeId = 1;
                                //    //Get the activity id for the job flow record based on the event type id on the mploy table:
                                //    var activityId = GetActivityId(contactLogRecord.idLogType, entityTypeId);

                                //    //Insert the activity log associated to the customer:

                                //    //If the Note field is blank, do not insert.
                                //    if (!string.IsNullOrEmpty(contactLogRecord.Note))
                                //    {
                                    
                                //        //Insert the ActivityLog record here:
                                //        //var insertedActivityLogId = _db.InsertActivityLog(activityId, contactLogRecord.created,
                                //        //    contactLogRecord.Note, contactLogRecord.idUser);
                                //    }
                                //}
                            }
                        }//Log Id If statement
                    } 
                }
                catch
                (Exception ex)
                {
                    new LogWriterFactory().Create().Write(ex.Expand("Error occured in tbContactLog with idLog: " + _mployIdLog));
                }
            }
        }

        static int GetActivityId(int logTypeId, int entityTypeId)
        {
            /*****
            1	Left Voicemail
            2	Schedule Call
            3	Spoke With
            4	Sent Email
            5	Received Email
            6	Other Log Entry
            10	Met With
            20	Status Changed
            21	Sent Text
            22	LinkedIn Requested
            23	LinkedIn Accepted
            24	Received Voicemail
            25	Sent InMail
            *****/
            int activityTypeId = -1;

            //Depeding on the entityTypeId, return the associated activityTypeId:
            switch (entityTypeId)
            {
                case 1:
                    //Find the activity types for the entity type 1:
                    switch (logTypeId)
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                        case 10:
                        case 20:
                        case 21:
                        case 22:
                        case 23:
                        case 24:
                        case 25:
                            break;
                    }
                    break;
                case 2:
                    //Find the activity types for the entity type 2
                    switch (logTypeId)
                    {
                        case 1:
                            activityTypeId = 62;
                            break;
                        case 2:
                            activityTypeId = 63;
                            break;
                        case 3:
                            activityTypeId = 64;
                            break;
                        case 4:
                            activityTypeId = 65;
                            break;
                        case 5:
                            activityTypeId = 66;
                            break;
                        case 6:
                            activityTypeId = 67;
                            break;
                        case 10:
                            activityTypeId = 68;
                            break;
                        case 20:
                            activityTypeId = 69;
                            break;
                        case 21:
                            activityTypeId = 70;
                            break;
                        case 22:
                            activityTypeId = 71;
                            break;
                        case 23:
                            activityTypeId = 72;
                            break;
                        case 24:
                            activityTypeId = 73;
                            break;
                        case 25:
                            activityTypeId = 74;
                            break;
                    }
                    break;
            }           
            //Return the activity type id so it can be used to insert the new ActivityLog entry.
            return activityTypeId;            
        }

    }
}
        /**
         * Old Function - No longer used, but saving for now.
         **/
        //static int GetSalesRecruitingWorkflowId(int logTypeId)
        //{
        //    /**
        //        1	Left Voicemail
        //        2	Schedule Call
        //        3	Spoke With
        //        4	Sent Email
        //        5	Received Email
        //        6	Other Log Entry
        //        10	Met With
        //        20	Status Changed
        //        21	Sent Text
        //        22	LinkedIn Requested
        //        23	LinkedIn Accepted
        //        24	Received Voicemail
        //        25	Sent InMail
        //     **/

        //    var workflowId = -1;

        //    switch (logTypeId)
        //    {
        //        case 1:
        //            workflowId = 13;
        //            break;
        //        case 2:
        //            workflowId = 15;
        //            break;
        //        case 3:
        //            workflowId = 9;
        //            break;
        //        case 4:
        //            workflowId = 7;
        //            break;
        //        case 5:
        //            workflowId = 8;
        //            break;
        //        case 6:
        //            workflowId = 44;
        //            break;
        //        case 10:
        //            workflowId = 10;
        //            break;
        //        case 20:
        //            workflowId = 44;
        //            break;
        //        case 21:
        //            workflowId = 44;
        //            break;
        //        case 22:
        //            workflowId = 11;
        //            break;
        //        case 23:
        //            workflowId = 12;
        //            break;
        //        case 24:
        //            workflowId = 14;
        //            break;
        //        case 25:
        //            workflowId = 11;
        //            break;
        //        default:
        //            //Conversion
        //            workflowId = 44;
        //            break;
        //    }

        //    return workflowId;
        //}


/** OLD CODE **
 //Get the workflow id for the log type:
var workflowId = GetSalesRecruitingWorkflowId(contactLogRecord.idLogType);
                        
//Note cant be null and the incoming Note has records that are empty strings
var insertLogResult = _db.InsertContactLog(workflowId, contactLogRecord.Note, null, null, null, null, null, contactLogRecord.created, contactLogRecord.created, contactLogRecord.idContact, contactLogRecord.idUser);
**/