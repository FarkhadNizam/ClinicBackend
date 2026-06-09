namespace ClinicBackend.DTO
{
    public class CreateAppointmentDTO
    {
        public Guid PatientId { get; set; }

        public Guid ScheduleSlotId { get; set; }
    }

    public class AppointmentDTO
    {
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }

        public string PatientName { get; set; }

        public int PatientAge { get; set; }

        public DateTime Date { get; set; }

        public DateTime Time { get; set; }

        public string Status { get; set; }
    }

    public class ScheduleSlotDTO
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public DateTime TimeFrom { get; set; }

        public DateTime TimeTo { get; set; }
    }

    public class DoctorAvailabilityDto
    {
        public Guid DoctorId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int AvailableSlotsCount { get; set; }

        public DateTime? NearestSlot { get; set; }
    }

    public class CreateWaitingListEntryDto
    {
        public Guid PatientId { get; set; }

        public Guid SpecialtyId { get; set; }
    }
}
