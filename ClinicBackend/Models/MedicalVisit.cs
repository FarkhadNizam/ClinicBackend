namespace ClinicBackend.Models
{
    public class MedicalVisit
    {
        public string Id { get; set; }
        public string PatientId { get; set; }
        public Patient Patient { get; set; }

        public string DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public string Date { get; set; }
        public string Complaints { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public string Status { get; set; } // completed/planned
    }

}
