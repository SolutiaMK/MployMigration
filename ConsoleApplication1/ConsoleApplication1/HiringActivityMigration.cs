using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class HiringActivityMigration
    {

        static Entities _db;


        public static void ImportHiringActivity()
        {
            using (_db = new Entities())
            {
                //Read in the Hiring Aatvity records from the Mploy tables:


                //For each record, transform the data and insert into our DB:

                //Get the SalesRecruitingActivityLog.SalesRecruitingWorkflowId:
                var workflowId = GetSalesRecruitingWorkflowId();


            }
        }

        static int GetSalesRecruitingWorkflowId()
        {
            var workflowId = -1;

            switch (sourceId)
            {
                case 2:
                    sourceTypeId = 10;
                    break;
                default:

                    break;
            }

            return workflowId;
        }
    }
}
