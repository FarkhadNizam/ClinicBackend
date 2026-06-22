namespace ClinicBackend.Models
{
    public enum UserRole
    {
        Registrar,
        Admin,
        Doctor
    }

    public class User
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
        public Doctor? Doctor { get; set; }
    }
}
