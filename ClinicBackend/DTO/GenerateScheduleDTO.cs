namespace ClinicBackend.DTO
{
    public class GenerateScheduleDTO
    {
        public Guid DoctorId { get; set; }
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int DurationMinutes { get; set; }
    }
}