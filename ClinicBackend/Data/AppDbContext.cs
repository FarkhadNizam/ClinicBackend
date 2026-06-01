using Microsoft.EntityFrameworkCore;
using ClinicBackend.Models;

namespace ClinicBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<ScheduleSlot> ScheduleSlots { get; set; }
        public DbSet<MedicalVisit> MedicalVisits { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
    }
}
