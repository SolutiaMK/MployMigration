using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Schema;
using ConsoleApplication1;
using ConsoleApplication1.Helper;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace ConsoleApplication1
{
    class RequirementProcessWorkflow
    {
        static Entities _db;

        static int _mployJobId = -1;


        public static void ImportRequirementProcessWorkflow()
        {
            using (_db = new Entities())
            {
                try
                {
                    //Read in the Job table here:
                    var jobData = _db.ReadJob().ToList();

                    foreach (var jobRecord in jobData) //.Where(x => x.idJob == 3092)
                    {
                        _mployJobId = jobRecord.idJob;
                        //Import a WorkflowStateLog for the job record.
                        //Based on mapping, each requirement should have a workflowStateLog entry.

                        //Call functions to find out the current states and etc. to place a requirement into a workflow state.

                        //Insert the current job into the WorkflowStateLog table:
                        //Requirement Process = workflow id 1
                        var workflowTypeId = 1;
                        int? workflowStateId = null;
                        int? workflowStateSubId = null;

                        workflowStateId = GetRequirementWorkflowStateId(jobRecord.Closed, jobRecord.ClosedReason);
                        //workflowStateSubId = GetRequirementWorkflowStateSubId(workflowStateId, jobRecord.ClosedReason);

                        ObjectResult<InsertRequirementProcessWorkflow_Result> reqProcessWorkflow;
                        
                        if (workflowStateId == 23)
                        {
                            reqProcessWorkflow = _db.InsertRequirementProcessWorkflow(workflowTypeId,
                                workflowStateId, workflowStateSubId, null, jobRecord.Closed, null, 0, null,
                                jobRecord.iduser, jobRecord.Closed, jobRecord.idJob);
                        }
                        else
                        {
                            reqProcessWorkflow = _db.InsertRequirementProcessWorkflow(workflowTypeId, workflowStateId, workflowStateSubId, null, jobRecord.Created, null, 0, null, jobRecord.iduser, jobRecord.Closed, jobRecord.idJob);
                        }


                        //reqVar = reqProcessWorkflow;

                        var workflowLogId = -1;
                        var globalEntityId = string.Empty;

                        

                        foreach (var item in reqProcessWorkflow)
                        {
                            workflowLogId = item.Id;
                            globalEntityId = item.GlobalEntityId.ToString();
                        }

                        Debug.WriteLine("\n" + "Workflow log Id: " + workflowLogId + "GlobalEntityId: " + globalEntityId);
                    }
                }
                catch
                    (Exception ex)
                {
                    new LogWriterFactory().Create().Write(ex.Expand("Error occured with entry: " + _mployJobId));
                }

            }
        }

        //Based on the Job's closed date and closed reason, find and return the workflowStateId:
        static int? GetRequirementWorkflowStateId(DateTime? closedDate, string closedReason)
        {
            int? workflowStateId = null;

            //If closedDate IS null then workflowStateId = 2 (qualified)
            if (closedDate == null && string.IsNullOrEmpty(closedReason))
            {
                workflowStateId = 2; //Qualified
            }
            else if (closedDate != null && !string.IsNullOrEmpty(closedReason))
            {
                workflowStateId = 23; //Lost
            }
            else if (closedDate != null && string.IsNullOrEmpty(closedReason))
            {
                workflowStateId = 24; //Win
            }

            return workflowStateId;
        }

        static int? GetRequirementWorkflowStateSubId(int? workflowStateId, string closedReason)
        {
            int? workflowStateSubId = null;

            switch (workflowStateId)
            {
                case 23:
                    if (closedReason.Contains("Cancelled by Client".ToLower()))
                    {
                        
                    }
                    else if (closedReason.Contains("Filled by Client".ToLower()))
                    {
                        
                    }
                    else if (closedReason.Contains("Requirement not qualified".ToLower()))
                    {
                        
                    }
                    break;               
            }

            return workflowStateSubId;
        }
    }
}
