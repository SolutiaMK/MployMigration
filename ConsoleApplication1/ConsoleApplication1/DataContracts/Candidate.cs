using System;

namespace ConsoleApplication1.DataContracts
{
    public class Candidate
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String MiddleName { get; set; }
        public Nullable<int> GenderTypeId { get; set; }
        public Nullable<int> RaceTypeId { get; set; }
        public Nullable<int> CitizenshipTypeId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastUpdated { get; set; }
        public int ModifiedById { get; set; }
        public DateTime? CreateDate { get; set; }
        public int CreateById { get; set; }
                
    }
}
