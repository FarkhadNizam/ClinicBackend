using System;
using System.Linq;
using BCrypt.Net;
using ClinicBackend.Data;
using ClinicBackend.Models;

namespace ClinicBackend.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            if (!context.Users.Any())
            {
                var users = new[]
                {
                    new User { Id = Guid.NewGuid(), Login = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin"), Role = UserRole.Admin },
                    new User { Id = Guid.NewGuid(), Login = "registrar", PasswordHash = BCrypt.Net.BCrypt.HashPassword("registrar"), Role = UserRole.Registrar },
                    new User { Id = Guid.NewGuid(), Login = "doctor", PasswordHash = BCrypt.Net.BCrypt.HashPassword("doctor"), Role = UserRole.Doctor }
                };
                context.Users.AddRange(users);
                context.SaveChanges();
            }

            if (!context.Doctors.Any())
            {
                //var doctors = new[]
                //{
                //    new Doctor { Id = Guid.NewGuid(), FirstName = "Иван", LastName = "Иванов", Specialty = "Терапевт" },
                //    new Doctor { Id = Guid.NewGuid(), FirstName = "Петр", LastName = "Петров", Specialty = "Кардиолог" },
                //    new Doctor { Id = Guid.NewGuid(), FirstName = "Анна", LastName = "Сидорова", Specialty = "Невролог" }
                //};
                //context.Doctors.AddRange(doctors);
                //context.SaveChanges();
            }

            //if (!context.Patients.Any())
            //{
            //    var patients = new[]
            //    {
            //        new Patient { Id = Guid.NewGuid(), FirstName = "Алексей", LastName = "Смирнов", BirthDate = "1990-05-20", Phone = "123-456-7890" },
            //        new Patient { Id = Guid.NewGuid(), FirstName = "Мария", LastName = "Кузнецова", BirthDate = "1985-10-10", Phone = "234-567-8901" },
            //        new Patient { Id = Guid.NewGuid(), FirstName = "Дмитрий", LastName = "Соколов", BirthDate = "2000-01-15", Phone = "345-678-9012" }
            //    };
            //    context.Patients.AddRange(patients);
            //    context.SaveChanges();
            //}

            //if (!context.ScheduleSlots.Any())
            //{
            //    var doctors = context.Doctors.ToList();
            //    var dates = new[] { "2025-05-26", "2025-05-27" };

            //    foreach (var doctor in doctors)
            //    {
            //        foreach (var date in dates)
            //        {
            //            for (int hour = 9; hour < 17; hour++)
            //            {
            //                var from = $"{hour:D2}:00";
            //                var to = $"{hour + 1:D2}:00";

            //                context.ScheduleSlots.Add(new ScheduleSlot
            //                {
            //                    Id = Guid.NewGuid(),
            //                    DoctorId = doctor.Id,
            //                    Date = date,
            //                    TimeFrom = from,
            //                    TimeTo = to,
            //                    IsAvailable = true
            //                });
            //            }
            //        }
            //    }

            //    context.SaveChanges();
            //}
        }
    }
}
