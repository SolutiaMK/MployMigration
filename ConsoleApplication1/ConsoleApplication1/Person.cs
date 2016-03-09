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
    using System.Collections.Generic;
    
    public partial class Person
    {
        public Person()
        {
            this.Candidates = new HashSet<Candidate>();
            this.Customers = new HashSet<Customer>();
            this.PersonContactInformations = new HashSet<PersonContactInformation>();
            this.PersonMailAddresses = new HashSet<PersonMailAddress>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public Nullable<int> PreferredPhoneNumberId { get; set; }
        public Nullable<int> PreferredEmailAddressId { get; set; }
        public Nullable<int> PreferredTypicalRoleId { get; set; }
        public Nullable<int> GenderTypeId { get; set; }
        public Nullable<int> RaceTypeId { get; set; }
        public Nullable<int> CitizenshipTypeId { get; set; }
        public System.DateTime LastUpdated { get; set; }
        public Nullable<int> ModifiedById { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<int> CreatedById { get; set; }
        public Nullable<int> MPLOY_ContactId { get; set; }
        public Nullable<int> MPLOY_UserId { get; set; }
    
        public virtual ICollection<Candidate> Candidates { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
        public virtual GenderType GenderType { get; set; }
        public virtual PersonContactInformation PersonContactInformation { get; set; }
        public virtual PersonContactInformation PersonContactInformation1 { get; set; }
        public virtual ICollection<PersonContactInformation> PersonContactInformations { get; set; }
        public virtual ICollection<PersonMailAddress> PersonMailAddresses { get; set; }
    }
}