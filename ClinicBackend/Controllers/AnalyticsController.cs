using ClinicBackend.Data;
using ClinicBackend.DTO;
using ClinicBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicBackend.Controllers;

[ApiController]
[Route("api/analytics")]
public class AnalyticsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AnalyticsController(
        AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardDto>>
        GetDashboard()
    {
        var totalAppointments =
            await _context.Appointments
                .CountAsync();

        var plannedAppointments =
            await _context.Appointments
                .CountAsync(a =>
                    a.status ==
                    Appointment.Status.Planned);

        var completedAppointments =
            await _context.Appointments
                .CountAsync(a =>
                    a.status ==
                    Appointment.Status.Completed);

        var cancelledAppointments =
            await _context.Appointments
                .CountAsync(a =>
                    a.status ==
                    Appointment.Status.Canceled);

        var noShowAppointments =
            await _context.Appointments
                .CountAsync(a =>
                    a.status ==
                    Appointment.Status.NoShow);

        var totalPatients =
            await _context.Patients
                .CountAsync();

        var totalDoctors =
            await _context.Doctors
                .CountAsync();

        var waitingListCount =
            await _context.WaitingListEntries
                .CountAsync();

        return Ok(new DashboardDto
        {
            TotalAppointments =
                totalAppointments,

            PlannedAppointments =
                plannedAppointments,

            CompletedAppointments =
                completedAppointments,

            CancelledAppointments =
                cancelledAppointments,

            NoShowAppointments =
                noShowAppointments,

            TotalPatients =
                totalPatients,

            TotalDoctors =
                totalDoctors,

            WaitingListCount =
                waitingListCount,

            CompletionRate =
                totalAppointments == 0
                    ? 0
                    : completedAppointments * 100.0 /
                      totalAppointments,

            NoShowRate =
                totalAppointments == 0
                    ? 0
                    : noShowAppointments * 100.0 /
                      totalAppointments
        });
    }

    [HttpGet("appointments-per-day")]
    public async Task<ActionResult<List<DailyAppointmentsDto>>>
        GetAppointmentsPerDay()
    {
        var result =
            await _context.Appointments
                .Include(a => a.ScheduleSlot)
                .GroupBy(a =>
                    a.ScheduleSlot.Date.Date)
                .Select(g =>
                    new DailyAppointmentsDto
                    {
                        Date = g.Key,
                        Count = g.Count()
                    })
                .OrderBy(x => x.Date)
                .ToListAsync();

        return Ok(result);
    }

    [HttpGet("diagnoses")]
    public async Task<ActionResult<List<DiagnosisStatisticsDto>>>
        GetDiagnosisStatistics()
    {
        var result =
            await _context.MedicalVisits
                .Include(v => v.Diagnosis)
                .Where(v => v.Diagnosis != null)
                .GroupBy(v => new
                {
                    v.Diagnosis.Name,
                    v.Diagnosis.MkbCode,
                    v.Diagnosis.Category
                })
                .Select(g =>
                    new DiagnosisStatisticsDto
                    {
                        DiagnosisName = g.Key.Name,
                        MkbCode = g.Key.MkbCode,
                        Category = g.Key.Category,
                        Count = g.Count()
                    })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

        return Ok(result);
    }

    [HttpGet("doctor-workload")]
    public async Task<ActionResult<List<DoctorWorkloadDto>>>
    GetDoctorWorkload()
    {
        var result =
            await _context.Doctors
                .Include(d => d.Specialty)
                .Include(d => d.Appointments)
                .Select(d =>
                    new DoctorWorkloadDto
                    {
                        DoctorId = d.Id,

                        DoctorName =
                            d.LastName + " " +
                            d.FirstName,

                        Specialty =
                            d.Specialty.Name,

                        TotalAppointments =
                            d.Appointments.Count(),

                        CompletedAppointments =
                            d.Appointments.Count(a =>
                                a.status ==
                                Appointment.Status.Completed),

                        CanceledAppointments =
                            d.Appointments.Count(a =>
                                a.status ==
                                Appointment.Status.Canceled),

                        NoShowAppointments =
                            d.Appointments.Count(a =>
                                a.status ==
                                Appointment.Status.NoShow)
                    })
                .OrderByDescending(x =>
                    x.TotalAppointments)
                .ToListAsync();

        return Ok(result);
    }

    [HttpGet("specialties")]
    public async Task<ActionResult<List<SpecialtyStatisticsDto>>>
    GetSpecialtyStatistics()
    {
        var result =
            await _context.Specialties
                .Select(s =>
                    new SpecialtyStatisticsDto
                    {
                        SpecialtyName =
                            s.Name,

                        AppointmentCount =
                            _context.Appointments
                                .Count(a =>
                                    a.Doctor.SpecialtyId ==
                                    s.Id)
                    })
                .OrderByDescending(x =>
                    x.AppointmentCount)
                .ToListAsync();

        return Ok(result);
    }

    [HttpGet("no-show")]
    public async Task<ActionResult<List<NoShowAnalyticsDto>>> GetNoShowAnalytics()
    {
        var result =
            await _context.Appointments
                .Where(a => a.status == Appointment.Status.NoShow)
                .GroupBy(a => new
                {
                    Doctor = a.Doctor.LastName + " " + a.Doctor.FirstName,
                    Specialty = a.Doctor.Specialty.Name
                })
                .Select(g => new NoShowAnalyticsDto
                {
                    DoctorName = g.Key.Doctor,
                    SpecialtyName = g.Key.Specialty,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

        return Ok(result);
    }

    [HttpGet("waiting-list")]
    public async Task<ActionResult<List<WaitingListAnalyticsDto>>>
    GetWaitingListAnalytics()
    {
        var result =
            await _context.WaitingListEntries
                .GroupBy(w => w.Specialty.Name)
                .Select(g =>
                    new WaitingListAnalyticsDto
                    {
                        SpecialtyName = g.Key,
                        PatientsCount = g.Count()
                    })
                .OrderByDescending(x => x.PatientsCount)
                .ToListAsync();

        return Ok(result);
    }
}