namespace ClinicBackend.DTO
{
    public class CreateDoctorDTO
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Guid SpecialtyId { get; set; }        
        public int PlannedWeeklyHours { get; set; } = 40;
    }

   public class DoctorDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Guid SpecialtyId { get; set; }
        public string? SpecialtyName { get; set; }
        public int PlannedWeeklyHours { get; set; }
    }
}