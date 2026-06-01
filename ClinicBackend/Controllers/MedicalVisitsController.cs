// Controllers/MedicalVisitsController.cs
using ClinicBackend.Data;
using ClinicBackend.DTO;
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

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateVisit(string id, [FromBody] UpdateMedicalVisitDto visitDto)
    {
        var existing = await _context.MedicalVisits.FindAsync(id);
        if (existing == null)
        {
            return NotFound();
        }

        existing.Complaints = visitDto.Complaints;
        existing.DiagnosisId = visitDto.DiagnosisId;
        existing.Treatment = visitDto.Treatment;        

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
                Id = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                Complaints = visitDto.Complaints,
                DiagnosisId = visitDto.DiagnosisId,
                Treatment = visitDto.Treatment,
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
}