namespace ClinicBackend.DTO
{
    public class UpdateMedicalVisitDto
    {
        public string Complaints { get; set; }
        public Guid DiagnosisId { get; set; }
        public string Treatment { get; set; }
        public string Status { get; set; }
    }

    public class CreateMedicalVisitDto
    {
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public string Complaints { get; set; }
        public Guid DiagnosisId { get; set; }
        public string Treatment { get; set; }
    }

    public class MedicalVisitDto
    {
        public string Id { get; set; }
        public string Date { get; set; }
        public string Complaints { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public string Status { get; set; }

        public DoctorDto Doctor { get; set; }
    }

    public class FullMedicalVisitDto
    {
        public string Id { get; set; }
        public string Date { get; set; }
        public string Complaints { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public string Status { get; set; }

        public DoctorDto Doctor { get; set; }
        public PatientDTO Patient { get; set; }

        public List<MedicalVisitDto> History { get; set; }
    }
}
