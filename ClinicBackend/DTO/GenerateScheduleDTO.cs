namespace ClinicBackend.DTO
{
    public class GenerateScheduleRequest
    {
        public Guid DoctorId { get; set; }
        public string Date { get; set; } = string.Empty;       
        public string StartTime { get; set; } = string.Empty;  
        public string EndTime { get; set; } = string.Empty;    
        public int DurationMinutes { get; set; }
    }

    public class GenerateScheduleResponse
    {
        public int GeneratedCount { get; set; }
        public string Message { get; set; }
    }
}