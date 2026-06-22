using ClinicBackend.Data;
using ClinicBackend.DTO;
using ClinicBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WaitingListController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WaitingListController(
            AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddToWaitingList(
            [FromBody] CreateWaitingListEntryDto request)
        {
            var patient =
                await _context.Patients
                    .FindAsync(request.PatientId);

            if (patient == null)
            {
                return NotFound(
                    "Пациент не найден");
            }

            var specialty =
                await _context.Specialties
                    .FindAsync(request.SpecialtyId);

            if (specialty == null)
            {
                return NotFound(
                    "Специальность не найдена");
            }

            var alreadyExists =
                await _context.WaitingListEntries
                    .AnyAsync(x =>
                        x.PatientId ==
                        request.PatientId &&
                        x.SpecialtyId ==
                        request.SpecialtyId);

            if (alreadyExists)
            {
                return BadRequest(
                    "Пациент уже находится в листе ожидания");
            }

            var entry =
                new WaitingListEntry
                {
                    Id = Guid.NewGuid(),

                    PatientId =
                        request.PatientId,

                    SpecialtyId =
                        request.SpecialtyId,

                    RequestedAt =
                        DateTime.UtcNow
                };

            await _context.WaitingListEntries
                .AddAsync(entry);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                entry.Id,
                entry.RequestedAt
            });
        }
    }
}
