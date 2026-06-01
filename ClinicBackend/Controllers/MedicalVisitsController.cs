// Controllers/MedicalVisitsController.cs
using ClinicBackend.Data;
using ClinicBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class MedicalVisitsController : ControllerBase
{
    private readonly AppDbContext _context;

    public MedicalVisitsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("patient/{patientId}")]
    public async Task<ActionResult<IEnumerable<MedicalVisit>>> GetVisitsForPatient(string patientId)
    {
        return await _context.MedicalVisits
            .Where(v => v.PatientId == patientId)
            .Include(v => v.Doctor)
            .ToListAsync();
    }

    [HttpGet("{visitId}/{patientId}")]
    public async Task<ActionResult<FullMedicalVisitDto>> GetVisitById(string visitId, string patientId)
    {
        var visit = await _context.MedicalVisits
            .Include(v => v.Doctor)
            .Include(v => v.Patient)
            .FirstOrDefaultAsync(v => v.Id == visitId);

        if (visit == null)
        {
            return NotFound();
        }

        // Визит существует, возвращаем как обычно
        var historyVisits = await _context.MedicalVisits
            .Where(v => v.PatientId == visit.PatientId && v.Id != visit.Id && v.Status != "planned")
            .Include(v => v.Doctor)
            .ToListAsync();

        var result = new FullMedicalVisitDto
        {
            Id = visit.Id,
            Date = visit.Date,
            Complaints = visit.Complaints,
            Diagnosis = visit.Diagnosis,
            Treatment = visit.Treatment,
            Status = visit.Status,
            Doctor = new DoctorDto
            {
                Id = new Guid(visit.Doctor.Id),
                FirstName = visit.Doctor.FirstName,
                LastName = visit.Doctor.LastName,
                Specialty = visit.Doctor.Specialty
            },
            Patient = new PatientDto
            {
                Id = visit.Patient.Id,
                FirstName = visit.Patient.FirstName,
                LastName = visit.Patient.LastName,
                BirthDate = visit.Patient.BirthDate,
                InsuranceNumber = visit.Patient.InsuranceNumber,
                Phone = visit.Patient.Phone
            },
            History = historyVisits.Select(h => new MedicalVisitDto
            {
                Id = h.Id,
                Date = h.Date,
                Complaints = h.Complaints,
                Diagnosis = h.Diagnosis,
                Treatment = h.Treatment,
                Status = h.Status,
                Doctor = new DoctorDto
                {
                    Id = new Guid(h.Doctor.Id),
                    FirstName = h.Doctor.FirstName,
                    LastName = h.Doctor.LastName,
                    Specialty = h.Doctor.Specialty
                }
            }).ToList()
        };

        return result;
    }



    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateVisit(string id, [FromBody] UpdateMedicalVisitDto visitDto)
    {
        var existing = await _context.MedicalVisits.FindAsync(id);
        if (existing == null)
        {
            return NotFound();
        }

        existing.Complaints = visitDto.Complaints;
        existing.Diagnosis = visitDto.Diagnosis;
        existing.Treatment = visitDto.Treatment;
        existing.Status = visitDto.Status;

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost]
    public async Task<ActionResult<MedicalVisit>> CreateVisit([FromBody] CreateMedicalVisitDto visitDto)
    {
        try
        {
            // Проверяем существование пациента и доктора
            var patient = await _context.Patients.FindAsync(visitDto.PatientId);
            if (patient == null)
                return BadRequest("Пациент не найден");

            var doctor = await _context.Doctors.FindAsync(visitDto.DoctorId);
            if (doctor == null)
                return BadRequest("Доктор не найден");

            var visit = new MedicalVisit
            {
                Id = Guid.NewGuid().ToString(),
                Date = DateTime.UtcNow.ToString("o"),
                Complaints = visitDto.Complaints,
                Diagnosis = visitDto.Diagnosis,
                Treatment = visitDto.Treatment,
                Status = visitDto.Status,
                PatientId = visitDto.PatientId,
                DoctorId = visitDto.DoctorId
            };

            _context.MedicalVisits.Add(visit);
            await _context.SaveChangesAsync();

            // Возвращаем созданный визит с кодом 200 OK
            return Ok(visitDto);
        }
        catch (Exception ex)
        {
            // Логируем ошибку
            Console.WriteLine($"Ошибка при создании визита: {ex.Message}");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    public class UpdateMedicalVisitDto
    {
        public string Complaints { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public string Status { get; set; }
    }

    public class CreateMedicalVisitDto
    {
        public string PatientId { get; set; }
        public string DoctorId { get; set; }
        public string Complaints { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public string Status { get; set; } = "completed";
    }
}


public class PatientDto
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string BirthDate { get; set; }
    public string InsuranceNumber { get; set; }
    public string Phone { get; set; }
}

public class MedicalVisitDto
{
    public string Id { get; set; }
    public string Date { get; set; }
    public string Complaints { get; set; }
    public string Diagnosis { get; set; }
    public string Treatment { get; set; }
    public string Status { get; set; }

    public DoctorDto Doctor { get; set; }
}

public class FullMedicalVisitDto
{
    public string Id { get; set; }
    public string Date { get; set; }
    public string Complaints { get; set; }
    public string Diagnosis { get; set; }
    public string Treatment { get; set; }
    public string Status { get; set; }

    public DoctorDto Doctor { get; set; }
    public PatientDto Patient { get; set; }

    public List<MedicalVisitDto> History { get; set; }
}