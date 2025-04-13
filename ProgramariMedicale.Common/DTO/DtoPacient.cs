namespace ProgramariMedicale.Common.DTO;


    public  class DtoPacient
    {
        public  string IDNP { get; set; }
        public string FullName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }

        public string Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }

    }
