namespace ClinicBackend.Models
{
    public class Appointment
    {
        // If status = booked and no slot => waitinglist

        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid ScheduleSlotId { get; set; }
        public DateTime CreatedAt { get; set; }

        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
        public ScheduleSlot ScheduleSlot { get; set; }

        public Status status { get; set; }
        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }

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
