using System.ComponentModel.DataAnnotations;

namespace ClinicBackend.Models
{
    public class Patient
    {
        public Guid Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string? Phone { get; set; }
        public string? InsuranceNumber { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }
}
