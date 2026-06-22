namespace ClinicBackend.DTO
{
    public class UpdateMedicalVisitDto
    {
        public string Complaints { get; set; }
        public Guid DiagnosisId { get; set; }
        public string Treatment { get; set; }        
    }

    public class CreateMedicalVisitDto
    {        
        public string Complaints { get; set; }

        public Guid DiagnosisId { get; set; }

        public string? Treatment { get; set; }
    }

    public class MedicalVisitDto
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public string? Complaints { get; set; }

        public Guid? DiagnosisId { get; set; }

        public string? DiagnosisName { get; set; }

        public string? DiagnosisCode { get; set; }

        public string? Treatment { get; set; }
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

    public class AppointmentVisitDto
    {
        public Guid AppointmentId { get; set; }

        public PatientDto Patient { get; set; }

        public MedicalVisitDto? Visit { get; set; }

        public List<MedicalVisitHistoryDto> History { get; set; }
    }

    public class PatientDto
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime BirthDate { get; set; }

        public string? InsuranceNumber { get; set; }

        public string? Phone { get; set; }
    }

    public class MedicalVisitHistoryDto
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public string? DiagnosisName { get; set; }

        public string? Complaints { get; set; }

        public string? Treatment { get; set; }

        public string DoctorName { get; set; }
    }
}
