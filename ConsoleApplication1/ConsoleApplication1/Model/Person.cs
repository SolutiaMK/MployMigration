namespace ConsoleApplication1.Model
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string Gender { get; set; }
        public string Status { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Email { get; set; }
        public int handle0type { get; set; }
        public string handle0text { get; set; }
        public int handle1type { get; set; }
        public string handle1text { get; set; }
        public int handle2type { get; set; }
        public string handle2text { get; set; }
        public int handle3type { get; set; }
        public string handle3text { get; set; }
        
        public int IdOrganization { get; set; }
        public string Title { get; set; }
        public int IdContact { get; set; }
        public int IdUser { get; set;  }
        public int IdContactType { get; set; }

        public string xdata { get; set; }
        public string Notes { get; set; }
        public string ResumeText { get; set; }
        public int Source { get; set; }
        public int IdStatus { get; set; }

        //[Column(TypeName = "DateTime2")]
        //public DateTime Created { get; set; }
        public System.DateTime? Created { get; set; }
    }
}
