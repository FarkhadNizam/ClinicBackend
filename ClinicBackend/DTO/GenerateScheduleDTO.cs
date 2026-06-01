namespace ClinicBackend.DTO
{
    public class GenerateScheduleDTO
    {
        public string DoctorId { get; set; }
        public string Date { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int DurationMinutes { get; set; }
    }
}