namespace ClinicBackend.Models
{
    public class Diagnosis
    {
        public Guid DiagnosisId { get; set; }

        public string MkbCode { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }
    }
}
