namespace ClinicBackend.Models
{
    public class ScheduleSlot
    {
        public string Id { get; set; }
        public string DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public string Date { get; set; }
        public string TimeFrom { get; set; }
        public string TimeTo { get; set; }
        public bool IsAvailable { get; set; }
    }

}
