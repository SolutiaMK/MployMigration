using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ConsoleApplication1;

namespace ConsoleApplication1
{
    internal class LookupValue
    {
        //private string idTypeListToReturn = "";
        //Create functions to do the lookup work in here.
        //Call this class when I need to do lookups and use the appropriate function to return the correct value?

        //Method to go through the Handle Text and Handle Type. Takes in the current handle text and type and looks up the matching type from the DB.  Assigns the DB's type to the text and inserts it into the correct table.
        public static int AssignHandleTextType(int handleType, string handleText)
        {
            /* Mploys key for contact types:
            Other	0 - not in DB
            Mobile	1
            Home	2
            Pager	3 - not in DB
            Work	4
            Fax	    5
            Email	6
            URL	    7
            IM	    8
            LinkedIn 9
            Facebook 10
            Twitter	11
            Blog	12*/


           /* if (handleType.Contains(","))
            {
                handleType = handleType.Replace(",", "");
            }*/

            //var we are setting based on the types, will be returned holding the correct Id to insert into the ContactInformation.ContactInfoTypeId column.
            //var currentContactInfoType = 0;
            //if (handleType != null)
            //{
            var currentContactInfoType = 0;

                switch (handleType)
                {
                    case 1:
                        //Check to see waht the handle type equals and set the ContactInfoTypeId field to our DBs equivalent.
                        currentContactInfoType = 1;
                        break;
                    case 2:
                        currentContactInfoType = 6;
                        break;
                    case 3:
                        break;
                    case 4:
                        currentContactInfoType = 5;
                        break;
                    case 5:
                        currentContactInfoType = 13;
                        break;
                    case 6:
                        currentContactInfoType = 2;
                        break;
                    case 7:
                        currentContactInfoType = 11;
                        break;
                    case 8:
                        currentContactInfoType = 7;
                        break;
                    case 9:
                        currentContactInfoType = 3;
                        break;
                    case 10:
                        currentContactInfoType = 4;
                        break;
                    case 11:
                        currentContactInfoType = 9;
                        break;
                    case 12:
                        currentContactInfoType = 12;
                        break;
                    case 0:
                        break;
                }
            //}
           /* if (handleType != "0" && !string.IsNullOrEmpty(handleType) && handleType != "3")
            {
                insertContactInformation.Description = handleText;
                _db.ContactInformations.Add(insertContactInformation);
                _db.SaveChanges();
            }*/
                return currentContactInfoType;
        }

        public static int FindCitizenshipType(string value)
        {
            var citizenshipTypeId = 0;
            switch (value)
            {
                case "US Citizen":
                    citizenshipTypeId = 1;
                    break;
                case "Green Card":
                    citizenshipTypeId = 2;
                    break;
                case "H1":
                    citizenshipTypeId = 3;
                    break;
                case "Tn":
                    citizenshipTypeId = 4;
                    break;
                case "F-1":
                    citizenshipTypeId = 5;
                    break;
                case "EAD":
                    citizenshipTypeId = 6;
                    break;
                case "Other":
                    citizenshipTypeId = 7;
                    break;
            }
            return citizenshipTypeId;
        }

/*        private static string ConvertToMoney(string moneyString)
        {
            if (moneyString.Contains("-"))
            {
                moneyString = removeSalaryRange.Replace(moneyString, "");
            }
            else if (moneyString.Contains("."))
            {
                moneyString = removeDecimal.Replace(moneyString, "");
            }
            else
            {
                moneyString = null;
            }
            
            return moneyString;
        }

        private static Regex digitsOnly = new Regex(@"[^\d]");
        private static Regex removeSalaryRange = new Regex(@"(\d*\W*-)");
        private static Regex removeDecimal = new Regex(@"(\.\d*)");
*/

        private static Regex CleanMoneyString = new Regex(@"\.\d*|(\D)|\d*\s?\-");
        private static Regex FormateDate = new Regex(@"th");

        //Method to parse through the xml from the Mploy Candidate file.  
        public static Candidate ParseXML(string xdata, Candidate insertCandidate)
        {
            //Remove the $ and ',' from the xdata so that I can convert the numbers to ints.
            //xdata = xdata.Replace("$", "");
            xdata = xdata.Replace(",", "");
            //Read in the xml string
            String pattern = "(<.*?>)|(.+?(?=<|$))"; //"(?<=>)(\w*)(?=<\/)";
            //Break it up using Regex 
            Regex rgx = new Regex(pattern);
            MatchCollection matches = rgx.Matches(xdata);

            int currentSalary = 0;
            int currentRate = 0;

            //Go through the matches list and assign the values between the tags to the correct spots in the DB
            if (matches.Count > 0)
            {

                foreach (Match match in matches)
                {
                    //Console.WriteLine("XML: " + match.Value);
                    var nextValue = match.NextMatch().Value;

                    if (match.Value == "<jobtype>" && nextValue != "</jobtype>")
                    {
                        //There are new fields for this value
                        //CandidateEmploymentInterestType table has Ids to put into the CandidateEmploymentInterestTypeId column in the Candidate table.
                    }
                    else if (match.Value == "<contract>" && nextValue != "</contract>")
                    {
                        //CandidateContractTypeId is the column in Candidate 
                        //Get the TypeId from the table CandidateContractType
                        //Account for "W2 Without Benefits" & "W2 w/o Benefits"... spelling variations exsist in the Mploy data. Some have "or"

                    }
                    else if (match.Value == "<currentsalary>" && nextValue != "</currentsalary>")
                    {

                        nextValue = CleanMoneyString.Replace(nextValue, "");
                        insertCandidate.CurrentSalary= nextValue != "" ? Convert.ToInt32(nextValue) : 0;
                        currentSalary = nextValue != "" ? Convert.ToInt32(nextValue) : 0;
                        //For right now I am removing the front end of a salary range.  The way the DB is set up I am not able to store a range.
                        //nextValue = ConvertToMoney(nextValue);
                    }
                    else if (match.Value == "<desiredsalary>" && nextValue != "</desiredsalary>")
                    {

                        //Some fields contain the word "same" in the desired column.
                        if (nextValue.ToLower().Contains("same"))
                        {
                            insertCandidate.DesiredSalary = currentSalary;
                        }
                        else
                        {
                            nextValue = CleanMoneyString.Replace(nextValue, "");
                            insertCandidate.DesiredSalary = nextValue != "" ? Convert.ToInt32(nextValue) : 0;
                        }
                        
                    }
                    else if (match.Value == "<currentrate>" && nextValue != "</currentrate>")
                    {
                        nextValue = CleanMoneyString.Replace(nextValue, "");
                        insertCandidate.CurrentRate = nextValue != "" ? Convert.ToInt32(nextValue) : 0;
                        currentRate = nextValue != "" ? Convert.ToInt32(nextValue) : 0;

                    }
                    else if (match.Value == "<desiredrate>" && nextValue != "</desiredrate>")
                    {
                        //Some fields contain the word "same" in the desired column.
                        if (nextValue.ToLower().Contains("same"))
                        {
                            insertCandidate.DesiredRate = currentRate;
                        }
                        else
                        {
                            nextValue = CleanMoneyString.Replace(nextValue, "");
                            insertCandidate.DesiredRate = nextValue != "" ? Convert.ToInt32(nextValue) : 0;

                            /*nextValue = ConvertToMoney(nextValue);
                            nextValue = digitsOnly.Replace(nextValue, "");
                            insertCandidate.DesiredRate = Convert.ToInt32(nextValue.Replace(" ", ""));*/
                        }
                    }
                    else if (match.Value == "<citizenship>" && nextValue != "</citizenship>")
                    {
                        //CitizenshipType goes into the Person table...
                        //var citizenshipTypeId = FindCitizenshipType(match.Value);
                        //insertPerson.CitizenshipTypeId = citizenshipTypeId;
                    }
                    else if (match.Value == "<maximumtravel>" && nextValue != "</maximumtravel>")
                    {
                        //TravelId goes from 1-10 with distance going from 10-100
                        //counter to increment the value of what the MaxTravelTypeId should be. As i (0-100) increments by 10, the travelTypeId increments by 1 to match the Ids in the DB table (MaxTravelType)
                        int travelTypeId = 0;
                        for (int i = 0; i <= 100; i += 10)
                        {
                            //if i == 0 we do not want to insert a MaxTravelId. If i != 0, then insert that
                            if (Convert.ToInt32(nextValue) == i && i != 0)
                            { 
                                    insertCandidate.MaxTravelTypeId = travelTypeId;
                                
                            }
                            travelTypeId++;
                        }
                    }
                    else if (match.Value == "<relocateto>" && nextValue != "</relocateto>")
                    {
                        // 0 = no (false), 1 = yes (true)
                        switch (nextValue.ToLower())
                        {
                            case "yes":
                                insertCandidate.IsOpenToRelocation = true;
                                break;
                            case "no":
                                insertCandidate.IsOpenToRelocation = false;
                                break;
                            default:
                                insertCandidate.IsOpenToRelocation = null;
                                break;
                        }                    
                    }
                    else if (match.Value == "<dateavailable>" && nextValue != "</dateavailable>")
                    {
                        //insertCandidate.AvailableDate = nextValue.Any(char.IsDigit) ? Convert.ToDateTime(nextValue) : DateTime.Now;
                        /*if (nextValue.Contains("th"))
                        {
                            nextValue = FormateDate.Replace(nextValue, "");
                            insertCandidate.AvailableDate = Convert.ToDateTime(nextValue);
                        }
                        else
                        {
                            insertCandidate.AvailableDate = DateTime.Now;
                        }*/
                        //insertCandidate.AvailableDate = DateTime.Now;
                    }
                    else if (match.Value == "<referredby>" && nextValue != "</referredby>")
                    {
                        //I think you get the typeId from the CandidateReferralType table and then that Id goes into the Candidate table in the ReferralId column.
                        //these "referredby" tags will hold the name of the person who referred them current candidate.
                    }else if (nextValue == "</xdata>")
                    {
                        //At this point we are at the end of the xdata string and it will error if it runs again.  Once on "</xdata>" there is no NextValue to grab.
                        break;
                    }
                }
            }
            return insertCandidate;
        }

        public static Requirement ParseJobXML(string xdata, Requirement insertRequirement)
        {
            //Remove the $ and ',' from the xdata so that I can convert the numbers to ints.
            
            xdata = xdata.Replace(",", "");
            //Read in the xml string
            String pattern = "(<.*?>)|(.+?(?=<|$))"; //"(?<=>)(\w*)(?=<\/)";
            //Break it up using Regex 
            Regex rgx = new Regex(pattern);
            MatchCollection matches = rgx.Matches(xdata);


            //<xdata><statusx>Certified</statusx><travelrequired></travelrequired><annualsalary></annualsalary><placementfee></placementfee><hourlypayrate></hourlypayrate><hourlybillrate></hourlybillrate><contractduration>3 Week Assignment</contractduration></xdata>
            //Go through the matches list and assign the values between the tags to the correct spots in the DB
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    //Console.WriteLine("XML: " + match.Value);
                    var nextValue = match.NextMatch().Value;

                    if (nextValue == "</xdata>")
                    {
                        //At this point we are at the end of the xdata string and it will error if it runs again.  Once on "</xdata>" there is no NextValue to grab.
                        break;
                    }
                    else if (match.Value == "<statusx>" && nextValue != "</statusx>")
                    {

                    }
                    else if (match.Value == "<travelrequired>" && nextValue != "</travelrequired>")
                    {
                        if (nextValue.ToLower().Contains("yes"))
                        {
                            //insertRequirement.TravelRequired = true;
                        }
                        else
                        {
                            //insertRequirement.TravelRequired = false;
                        }
                    }
                    else if (match.Value == "<annualsalary>" && nextValue != "</annualsalary>")
                    {
                        
                        nextValue = CleanMoneyString.Replace(nextValue, "");
                        insertRequirement.MinRate = Convert.ToInt32(nextValue);
                        insertRequirement.MaxRate = Convert.ToInt32(nextValue);


                    }
                    else if (match.Value == "<annualsalary>" && nextValue == "</annualsalary>")
                    {
                        insertRequirement.MinRate = Convert.ToInt32(0);
                        insertRequirement.MaxRate = Convert.ToInt32(0);
                    }
                    else if (match.Value == "<placementfee>" && nextValue != "</placementfee>")
                    {

                    }
                    else if (match.Value == "<hourlypayrate>" && nextValue != "</hourlypayrate>")
                    {

                    }
                    else if (match.Value == "<hourlybillrate>" && nextValue != "</hourlybillrate>")
                    {

                    }
                    else if (match.Value == "<contactduration>" && nextValue != "</contactduration>")
                    {
                        
                    }
                    else if (match.Value == "<accountmanager>" && nextValue != "</accountmanager>")
                    {

                    }
                    else if (match.Value == "<Textbox1>" && nextValue != "</Textbox1>")
                    {
                        
                    }
                }
            }
            return insertRequirement;
        }

        //public static SalesRecruitingActivityLog ParseHiringActivityXML(string xdata, SalesRecruitingActivityLog insertRequirementActivityLog)
        //{
        //    //Remove the $ and ',' from the xdata so that I can convert the numbers to ints.
        //    xdata = xdata.Replace(",", "");
        //    //Read in the xml string
        //    String pattern = "(<.*?>)|(.+?(?=<|$))"; //"(?<=>)(\w*)(?=<\/)";
        //    //Break it up using Regex 
        //    Regex rgx = new Regex(pattern);
        //    MatchCollection matches = rgx.Matches(xdata);

        //    //Go through the matches list and assign the values between the tags to the correct spots in the DB
        //    if (matches.Count > 0)
        //    {
        //        foreach (Match match in matches)
        //        {
        //            //Console.WriteLine("XML: " + match.Value);
        //            var nextValue = match.NextMatch().Value;

        //            if (nextValue == "</xdata>")
        //            {
        //                //At this point we are at the end of the xdata string and it will error if it runs again.  Once on "</xdata>" there is no NextValue to grab.
        //                break;
        //            }
        //            else if (match.Value == "<payrate>" && nextValue != "</payrate>")
        //            {
        //                nextValue = CleanMoneyString.Replace(nextValue, "");
        //                if (!string.IsNullOrEmpty(nextValue))
        //                {
        //                    insertRequirementActivityLog.PayRate = Convert.ToInt32(nextValue);
        //                }
        //                else
        //                {
        //                    insertRequirementActivityLog.PayRate = null;
        //                }  
        //            }
        //            else if (match.Value == "<billrate>" && nextValue != "</billrate>")
        //            {
        //                nextValue = CleanMoneyString.Replace(nextValue, "");
        //                if (!string.IsNullOrEmpty(nextValue))
        //                {
        //                    insertRequirementActivityLog.BillRate = Convert.ToInt32(nextValue);
        //                }
        //                else
        //                {
        //                    insertRequirementActivityLog.BillRate = null;
        //                }                        
        //            }

        //        }
        //    }
        //    return insertRequirementActivityLog;
        //}

        //This function takes in a string that holds which stored procedure we want run. Then it returns a list of Key/Value  pairs of the Id/Description from the specified Type table.
        public static List<KeyValuePair<int, string>> GetTypeIdList(List<KeyValuePair<int, string>> sourceTypeList )
        {
            //List<KeyValuePair<int, string>> sourceTypeList = new List<KeyValuePair<int, string>>(19);

            Entities dbSourceList;

            using (dbSourceList = new Entities())
            {
                try
                {
                    var dataList = dbSourceList.ReadSourceType().ToList();
                    sourceTypeList.Insert(0, new KeyValuePair<int, string>(0, "skipFirst"));
/*                    //Check the Stored Procedure that needs to run:
                    if (idTypeListToReturn == "sourceTypeId")
                    {
                        data = _db.ReadSourceType().ToList();
                    }
                    else if (idTypeListToReturn == "candidateContractTypeId")
                    {
                        data = _db.ReadCandidateContractType().ToList();
                    }*/
                    
                    //Start at 1 to match the numbering in the DB table
                    int i = 1;
                    //Read each SourceType from the data
                    foreach (var list in dataList)
                    {
                        sourceTypeList.Insert(i, (new KeyValuePair<int, string>(list.Id, list.Name)));

                        i++;
                    }
                }
                catch
                (Exception ex)
                {

                    Console.WriteLine(ex.ToString());
                }
            }
            return sourceTypeList;
        } 
        //Function to lookup the SourceTypeId for the Candidate table from the SourceType table
        public static Candidate FindSourceType(Candidate insertCandidate, string source, List<KeyValuePair<int, string>> sourceTypeIdList)
        {
            SourceType sourceType = new SourceType();
            //Gets the list of Ids and Descriptions for each SourceType stored in our DB. Returns a list of Key Value pairs (Id, Description).
            //string idTypeListToReturn = "sourceTypeId";
            //var sourceTypeIdList = GetTypeIdList(idTypeListToReturn);            

            foreach (var type in sourceTypeIdList)
            {
                if (type.Value.Equals(source))
                {
                    insertCandidate.SourceTypeId = type.Key;
                }else
                {
                    insertCandidate.SourceTypeId = 19;

                }
            }
            return insertCandidate;
        }

        public static void FindOwnerId()
        {
            
        }
    }
}
