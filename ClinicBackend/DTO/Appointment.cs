namespace ClinicBackend.DTO
{
    public class CreateAppointmentDTO
    {
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
    }

    public class AppointmentDTO
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
    }
}
