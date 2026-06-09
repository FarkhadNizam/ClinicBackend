namespace ClinicBackend.DTO
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string Role { get; set; }
        public Guid UserId { get; set; }
    }
}
