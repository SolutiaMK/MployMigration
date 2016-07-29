using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using ConsoleApplication1;
using ConsoleApplication1.Helper;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace ConsoleApplication1
{
    internal class OrganizationMigration
    {


        static Entities _db;
        private static int _companyId = -1;
        private static Guid _globalEntityId;
        private static int _companyMailingAddressId = -1;



        public static void OrganizationImport()
        {
            using (_db = new Entities())
            {
                try
                {
                    //Read in the data from the TempOrganizationFileTable:
                    var organizationData = _db.ReadOrganization().ToList();
        
                    

                    //For each record in from TempOrganizationFileTable (Organization Mploy file):
                    foreach (var orgRecord in organizationData)
                    {

                        Company insertCompany = new Company();

                        var companyTypeId = GetCompanyType(orgRecord.idOrganizationType);

                        //Get the Id for the person assigned to this comapny based on the mploy user id:
                        //This Id is used in the SP to assign the Sales or Recruiting Person down the road.
                        int? personIdFromUser = 0;
                        if (orgRecord.idUserOwner != null && orgRecord.idUserOwner != 0)
                        {
                            var result = _db.GetUserFromMployUserId(Convert.ToInt32(orgRecord.idUserOwner)).ToList();
                            personIdFromUser = Convert.ToInt32(result[0]);
                        }
                        else
                        {
                            personIdFromUser = null;
                        }

                        //Send the correct IdUser into the SP to insert a company:
                        int? idUser = -1;
                        if (orgRecord.idUserOwner == null || orgRecord.idUserOwner == 0)
                        {
                            idUser = null;
                        }
                        else
                        {
                            idUser = Convert.ToInt32(orgRecord.idUserOwner);
                        }
                        

                        //If the Organization name is not null or empty, then insert the Org:
                        if (!string.IsNullOrEmpty(orgRecord.Organization))
                        {
                            var companyInsertResult = _db.InsertCompany(orgRecord.Organization, companyTypeId, null, null, null, null, null, personIdFromUser, 0, Convert.ToInt32(orgRecord.idOrganization), idUser).ToList();

                            foreach (var item in companyInsertResult)
                            {
                                _companyId = item.Id;
                                _globalEntityId = item.GlobalEntityId;

                            }

                            Debug.WriteLine("\n" + orgRecord.Organization);

                            //Call the function to insert the Company Mailing Address:
                            InsertCompanyMailingAddress(orgRecord);

                            //Call the function to insert the Company Contact Information:
                            InsertCompanyContactInformation(orgRecord);

                            //Insert the Company Notes if they have notes associated to the incoming record:
                            if (!string.IsNullOrEmpty(orgRecord.InternalNote))
                            {
                                var noteResult = _db.InsertCompanyNote(_companyId, orgRecord.InternalNote, 0);
                            }

                            if (!string.IsNullOrEmpty(orgRecord.ExternalNote))
                            {
                                var noteResult = _db.InsertCompanyNote(_companyId, orgRecord.ExternalNote, 0);
                            }

                            //Insert into the CompanyBranch table
                            var branchId = ImportHelperMethods.GetBranchId(orgRecord.State, orgRecord.City);
                            var companyBranchResult = _db.InsertCompanyBranch(_companyId, branchId, orgRecord.Created, 0,
                                orgRecord.idUserOwner);
                        }
                        else
                        {
                            Debug.WriteLine("\n" +"***** Organization does not have a name: " + orgRecord.idOrganization + " *****");
                        }                                                                    
                    }

                }
                catch
                (Exception ex)
                {
                    new LogWriterFactory().Create().Write(ex.Expand("Error occured with Mploy Organization Id: " + _companyId));
                }
            }

        }
        //Helper methods:

        //Takes in the company type string from the mploy org record
        //Returns our DB's TypeId for the incoming org record
        static int GetCompanyType(int companyTypeId)
        {
            var returnedCompanyTypeId = -1;

            /* MPLOY Company Types:
                1	Not Potential Client
                2	Prospect
                3	Active Client
                4	Vendor
                5	Business Partner
                6	Competitor
             * */

            switch (companyTypeId)
            {
                case 1:                    
                    returnedCompanyTypeId = 4;
                    break;
                case 2:                  
                    returnedCompanyTypeId = 6;
                    break;
                case 3:
                    returnedCompanyTypeId = 1;
                    break;
                case 4:
                    returnedCompanyTypeId = 9;
                    break;
                case 5:
                    returnedCompanyTypeId = 2;
                    break;
                case 6:
                    returnedCompanyTypeId = 3;
                    break;
                default:
                    //Id of 8 = Unknown
                    returnedCompanyTypeId = 8;
                    break;
            }                    
            
            return returnedCompanyTypeId;
        }

        static void InsertCompanyMailingAddress(ReadOrganization_Result orgRecord)
        {
            ///////////////////////////////  MailingAddress for a Company ////////////////////////////////////          
            //If there is an address to insert:

            //Might want to insert an address for 
            if (!string.IsNullOrEmpty(orgRecord.Address) && !string.IsNullOrEmpty(orgRecord.City) && !string.IsNullOrEmpty(orgRecord.State) && !string.IsNullOrEmpty(orgRecord.Zip))
            {
                var result = _db.InsertCompanyMailingAddress(_companyId, orgRecord.Address, orgRecord.City, orgRecord.State, orgRecord.Zip, 0).ToList();
                _companyMailingAddressId = Convert.ToInt32(result[0]);
            }
        }

        static void InsertCompanyContactInformation(ReadOrganization_Result orgRecord)
        {
            ///////////////////////////////  CompanyContactInformation ////////////////////////////////////
            CompanyContactInformation insertCompanyContactInformation = new CompanyContactInformation();

            //Insert the company Phone, Fax, and URL info:
            
            //Phone:
            if (!string.IsNullOrEmpty(orgRecord.Phone))
            {
                var result = _db.InsertCompanyContactInformation(_companyId, 14, orgRecord.Phone, null, null, 0);
            }

            //Fax:
            if (!string.IsNullOrEmpty(orgRecord.Fax))
            {
                var result = _db.InsertCompanyContactInformation(_companyId, 13, orgRecord.Fax, null, null, 0);
            }

            //URL:
            if (!string.IsNullOrEmpty(orgRecord.URL))
            {
                var result = _db.InsertCompanyContactInformation(_companyId, 11, orgRecord.URL, null, null, 0);
            }

        }

    }
}
