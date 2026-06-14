namespace ClinicBackend.DTO
{
    public class DashboardDto
    {
        public int TotalAppointments { get; set; }

        public int PlannedAppointments { get; set; }

        public int CompletedAppointments { get; set; }

        public int CancelledAppointments { get; set; }

        public int NoShowAppointments { get; set; }

        public int TotalPatients { get; set; }

        public int TotalDoctors { get; set; }

        public int WaitingListCount { get; set; }

        public double CompletionRate { get; set; }

        public double NoShowRate { get; set; }
    }

    public class DailyAppointmentsDto
    {
        public DateTime Date { get; set; }

        public int Count { get; set; }
    }

    public class DiagnosisStatisticsDto
    {
        public string DiagnosisName { get; set; }

        public string MkbCode { get; set; }

        public string Category { get; set; }

        public int Count { get; set; }
    }

    public class DoctorWorkloadDto
    {
        public Guid DoctorId { get; set; }

        public string DoctorName { get; set; }

        public string Specialty { get; set; }

        public int TotalAppointments { get; set; }

        public int CompletedAppointments { get; set; }

        public int CanceledAppointments { get; set; }

        public int NoShowAppointments { get; set; }
    }

    public class SpecialtyStatisticsDto
    {
        public string SpecialtyName { get; set; }

        public int AppointmentCount { get; set; }
    }

    public class NoShowAnalyticsDto
    {
        public string DoctorName { get; set; }

        public string SpecialtyName { get; set; }

        public int Count { get; set; }
    }

    public class WaitingListAnalyticsDto
    {
        public string SpecialtyName { get; set; }

        public int PatientsCount { get; set; }
    }
}
