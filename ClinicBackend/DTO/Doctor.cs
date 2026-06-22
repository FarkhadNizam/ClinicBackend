namespace ClinicBackend.DTO
{
    public class CreateDoctorDTO
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Guid SpecialtyId { get; set; }

        public int PlannedWeeklyHours { get; set; }

        public string Login { get; set; }
    }

    public class DoctorCreatedDTO
    {
        public Guid DoctorId { get; set; }

        public string Login { get; set; }

        public string TemporaryPassword { get; set; }
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