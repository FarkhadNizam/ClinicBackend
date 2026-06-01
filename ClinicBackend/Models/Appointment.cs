namespace ClinicBackend.Models
{
    public class Appointment
    {        
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid ScheduleSlotId { get; set; }
        public DateTime CreatedAt { get; set; }

        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
        public ScheduleSlot ScheduleSlot { get; set; }

        public DateTime? StatusUpdatedAt { get; set; }
        public Status status { get; set; }
        public CancelReason? cancelReason { get; set; }

        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }

        public enum CancelReason
        {
            None,
            PatientRequest,
            DoctorUnavailable,
            Emergency,
            DuplicateBooking,
            TechnicalIssue,
            Other
        }        

        public enum Status
        {
            Booked,
            Planned,
            Canceled,
            NoShow,
            InProgress,
            Completed
        }
    }
}
