namespace ClinicBackend.Models
{
    public class Appointment
    {
        public Guid Id { get; set; }
        public string PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public string ScheduleSlotId { get; set; }
        public DateTime CreatedAt { get; set; }

        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
        public ScheduleSlot ScheduleSlot { get; set; }
    }
}
