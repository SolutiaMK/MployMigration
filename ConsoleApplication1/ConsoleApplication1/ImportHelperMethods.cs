using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class ImportHelperMethods
    {

        public static int GetBranchId(string state, string city)
        {
            var branchId = -1;

            if (state.ToUpper() == "MN")
            {
                branchId = 2;
            }
            else if(city.ToUpper() == "DALLAS")
            {
                branchId = 3;
            }
            else if (city.ToUpper() == "AUSTIN")
            {
                branchId = 4;
            }
            else
            {
                //Default to the MN branch:
                branchId = 2;
            }
            return branchId;
        }

        public static int? GetIdOrganization(int idOrganization)
        {
            //Make sure that the orgId being sent to the insert SP has the correct values.
            int? orgId = -1;
            if (idOrganization == 0)
            {
                orgId = null;
            }
            else
            {
                orgId = Convert.ToInt32(idOrganization);
            }
            return orgId;
        }
        

        public static int? GetGenderTypeId(string gender)
        {
            int? genderTypeId = null;

            switch (gender)
            {
                case "Male":
                    genderTypeId = 1;
                    break;
                case "Female":
                    genderTypeId = 2;
                    break;
            }

            return genderTypeId;
        }

        public static int? GetUserId(int idUser)
        {
            //Send the correct IdUser into the SP to insert a company:
            int? idUserVar = -1;
            if (idUser == 0)
            {
                idUserVar = null;
            }
            else
            {
                idUserVar = Convert.ToInt32(idUser);
            }
            return idUserVar;
        }

        public static int? GetSourceTypeId(int? sourceId)
        {
            int? sourceTypeId = -1;

            switch (sourceId)
            {
                case 2:
                    sourceTypeId = 10;
                    break;
                case 3:
                    sourceTypeId = null;
                    break;
                case 4:
                    sourceTypeId = 29;
                    break;
                case 5:
                    sourceTypeId = null;
                    break;
                case 6:
                    sourceTypeId = 16;
                    break;
                case 7:
                    sourceTypeId = 12;
                    break;
                case 8:
                    sourceTypeId = 14;
                    break;
                case 9:
                    sourceTypeId = null;
                    break;
                case 10:
                    sourceTypeId = 27;
                    break;
                case 11:
                    sourceTypeId = 8;
                    break;
                case 12:
                    sourceTypeId = 1;
                    break;
                case 13:
                    sourceTypeId = 25;
                    break;
                case 14:
                    sourceTypeId = null;
                    break;
                case 15:
                    sourceTypeId = 24;
                    break;
                case 16:
                    sourceTypeId = 11;
                    break;
                case 17:
                    sourceTypeId = 37;
                    break;
                case 18:
                    sourceTypeId = 7;
                    break;
                case 20:
                    sourceTypeId = 17;
                    break;
                case 21:
                    sourceTypeId = 26;
                    break;
                case 22:
                    sourceTypeId = 28;
                    break;
                case 23:
                    sourceTypeId = 23;
                    break;
                case 24:
                    sourceTypeId = 31;
                    break;
                case 25:
                    sourceTypeId = 33;
                    break;
                case 26:
                    sourceTypeId = null;
                    break;
                case 27:
                    sourceTypeId = 30;
                    break;
                case 28:
                    sourceTypeId = 6;
                    break;
                case 29:
                    sourceTypeId = 5;
                    break;
                case 30:
                    sourceTypeId = 2;
                    break;
                case 31:
                    sourceTypeId = null;
                    break;
                case 32:
                    sourceTypeId = 18;
                    break;
                case 33:
                    sourceTypeId = 3;
                    break;
                case 34:
                    sourceTypeId = 32;
                    break;
                case 35:
                    sourceTypeId = 36;
                    break;
                case 36:
                    sourceTypeId = 21;
                    break;
                case 37:
                    sourceTypeId = 35;
                    break;
                default:
                    sourceTypeId = 19;
                    break;
            }

            return sourceTypeId;
        }
    }
}
