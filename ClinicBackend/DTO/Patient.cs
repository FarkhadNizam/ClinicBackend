using System.ComponentModel.DataAnnotations;

namespace ClinicBackend.DTO
{
    public class PatientDTO
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        public string? Phone { get; set; }

        public string? InsuranceNumber { get; set; }
    }

    public class CreatePatientDTO
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        public string? Phone { get; set; }

        public string? InsuranceNumber { get; set; }
    }
}
