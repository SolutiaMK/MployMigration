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

                        //if (contactLogRecord.idLog > 33494)
                        //{
                            _mployIdLog = contactLogRecord.idLog;
                        
                            //Get the workflow id for the log type:
                            var workflowId = GetSalesRecruitingWorkflowId(contactLogRecord.idLogType);
                        
                            //Note cant be null and the incoming Note has records that are empty strings
                            var insertLogResult = _db.InsertContactLog(workflowId, contactLogRecord.Note, null, null, null, null, null, contactLogRecord.created, contactLogRecord.created, contactLogRecord.idContact, contactLogRecord.idUser);

                            Debug.WriteLine("\n" + "ContactLog Id: " + _mployIdLog);
                        //}
                    } 
                }
                catch
                (Exception ex)
                {
                    new LogWriterFactory().Create().Write(ex.Expand("Error occured in tbContactLog with idLog: " + _mployIdLog));
                }
            }
        }

        static int GetSalesRecruitingWorkflowId(int logTypeId)
        {
            /**
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
             **/

            var workflowId = -1;

            switch (logTypeId)
            {
                case 1:
                    workflowId = 13;
                    break;
                case 2:
                    workflowId = 15;
                    break;
                case 3:
                    workflowId = 9;
                    break;
                case 4:
                    workflowId = 7;
                    break;
                case 5:
                    workflowId = 8;
                    break;
                case 6:
                    workflowId = 44;
                    break;
                case 10:
                    workflowId = 10;
                    break;
                case 20:
                    workflowId = 44;
                    break;
                case 21:
                    workflowId = 44;
                    break;
                case 22:
                    workflowId = 11;
                    break;
                case 23:
                    workflowId = 12;
                    break;
                case 24:
                    workflowId = 14;
                    break;
                case 25:
                    workflowId = 11;
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
