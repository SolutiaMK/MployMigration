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
    
    public partial class PersonMailAddress
    {
        public PersonMailAddress()
        {
            this.People = new HashSet<Person>();
        }
    
        public int Id { get; set; }
        public int PersonId { get; set; }
        public int MailingAddressId { get; set; }
        public int MailingAddressTypeId { get; set; }
        public System.DateTime LastUpdated { get; set; }
        public Nullable<int> ModifiedById { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<int> CreatedById { get; set; }
    
        public virtual MailingAddress MailingAddress { get; set; }
        public virtual MailingAddressType MailingAddressType { get; set; }
        public virtual ICollection<Person> People { get; set; }
        public virtual Person Person { get; set; }
        public virtual ICollection<Person> People { get; set; }
    }
}
