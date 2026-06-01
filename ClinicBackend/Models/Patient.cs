namespace ClinicBackend.Models
{
    public class Patient
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BirthDate { get; set; }
        public string? Phone { get; set; }
        public string? InsuranceNumber { get; set; }

        public ICollection<MedicalVisit> Visits { get; set; }
    }


}
