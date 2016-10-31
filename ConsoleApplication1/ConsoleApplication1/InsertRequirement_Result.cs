//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ConsoleApplication1
{
    using System;
    
    public partial class InsertRequirement_Result
    {
        public int Id { get; set; }
        public System.Guid GlobalEntityId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string VMSField { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public Nullable<int> PrimaryCustomerId { get; set; }
        public int RequirementTypeId { get; set; }
        public int RequirementPriorityTypeId { get; set; }
        public int RequirementProjectTypeId { get; set; }
        public Nullable<int> PreferredTypicalRoleId { get; set; }
        public Nullable<int> TravelTypeId { get; set; }
        public Nullable<int> MaxTravelTypeId { get; set; }
        public Nullable<int> PaymentTermTypeId { get; set; }
        public System.DateTime PostingDate { get; set; }
        public Nullable<System.DateTime> DesiredStartDate { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<int> MinRate { get; set; }
        public Nullable<int> MaxRate { get; set; }
        public Nullable<int> Budget { get; set; }
        public Nullable<int> Duration { get; set; }
        public string ContractDetails { get; set; }
        public bool IsTravelRequired { get; set; }
        public bool IsDrugTestRequired { get; set; }
        public bool IsBackgroundCheckRequired { get; set; }
        public bool IsWorkFromHome { get; set; }
        public System.DateTime LastUpdated { get; set; }
        public Nullable<int> ModifiedById { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<int> CreatedById { get; set; }
        public Nullable<int> MPLOY_JobId { get; set; }
        public Nullable<int> MPLOY_ContactId { get; set; }
        public Nullable<System.DateTime> ClosedDate { get; set; }
        public Nullable<int> MPLOY_IdUserClosed { get; set; }
        public Nullable<int> MPLOY_IdUserFilled { get; set; }
        public Nullable<int> MPLOY_IdUserRecruiter { get; set; }
    }
}
