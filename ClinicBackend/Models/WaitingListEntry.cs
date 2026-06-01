namespace ClinicBackend.Models
{
    public class WaitingListEntry
    {
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }

        public Guid SpecialtyId { get; set; }

        public DateTime RequestedAt { get; set; }

        public Patient Patient { get; set; }

        public Specialty Specialty { get; set; }
    }
}
