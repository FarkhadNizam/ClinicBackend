namespace ClinicBackend.Models
{
    public class Doctor
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Specialty { get; set; }
        public string? Avatar { get; set; }

        public ICollection<ScheduleSlot> Slots { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }

}
