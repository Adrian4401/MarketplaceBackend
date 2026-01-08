using MarketplaceBackend.Presentation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketplaceBackend.Presentation.Controllers
{

    [ApiController]
    [Route("annoucement")]
    public class AnnoucementController : ControllerBase
    {
        private readonly AnnoucementDb _db;

        public AnnoucementController(AnnoucementDb db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.Annoucements.ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var annoucement = await _db.Annoucements.FindAsync(id);

            if (annoucement == null)
                return NotFound();

            return Ok(annoucement);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Annoucement newAnnoucement)
        {
            if (newAnnoucement.Description == null || newAnnoucement.Price < 0 || newAnnoucement.Title == null)
                return BadRequest();

            await _db.Annoucements.AddAsync(newAnnoucement);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newAnnoucement.Id }, newAnnoucement);
        }
    }
}