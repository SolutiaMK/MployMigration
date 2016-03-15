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
                    //
                    break;
                case 60:

                    break;
                case 100:

                    break;
                case 110:

                    break;
                case 120:

                    break;
                case 50:

                    break;
                case 40:

                    break;
                case 55:

                    break;
                case 90:

                    break;
                case 20:

                    break;
                case 30:

                    break;
                case 10:

                    break;
                case 35:

                    break;
                case 95:

                    break;
                case 105:

                    break;
                case 130:

                    break;
                case 140:

                    break;
                case 85:

                    break;
                case 102:

                    break;
                case 108:

                    break;
                case 51:

                    break;
                case 71:

                    break;
                case 52:

                    break;
                case 72:

                    break;
                default:

                    break;
            }

            return workflowId;
        }
    }
}
