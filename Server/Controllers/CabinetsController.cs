using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server;
using Server.Entities;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CabinetsController : ControllerBase
    {
        private readonly OnboardingBotContext _context;

        public CabinetsController(OnboardingBotContext context)
        {
            _context = context;
        }

        // GET: api/Cabinets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cabinet>>> GetCabinets()
        {
          if (_context.Cabinets == null)
          {
              return NotFound();
          }
            return await _context.Cabinets.ToListAsync();
        }

        // GET: api/Cabinets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cabinet>> GetCabinet(int id)
        {
          if (_context.Cabinets == null)
          {
              return NotFound();
          }
            var cabinet = await _context.Cabinets.FindAsync(id);

            if (cabinet == null)
            {
                return NotFound();
            }

            return cabinet;
        }

        // PUT: api/Cabinets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCabinet(int id, Cabinet cabinet)
        {
            if (id != cabinet.Id)
            {
                return BadRequest();
            }

            _context.Entry(cabinet).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CabinetExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Cabinets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Cabinet>> PostCabinet(Cabinet cabinet)
        {
          if (_context.Cabinets == null)
          {
              return Problem("Entity set 'OnboardingBotContext.Cabinets'  is null.");
          }
            _context.Cabinets.Add(cabinet);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCabinet", new { id = cabinet.Id }, cabinet);
        }

        // DELETE: api/Cabinets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCabinet(int id)
        {
            if (_context.Cabinets == null)
            {
                return NotFound();
            }
            var cabinet = await _context.Cabinets.FindAsync(id);
            if (cabinet == null)
            {
                return NotFound();
            }

            _context.Cabinets.Remove(cabinet);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CabinetExists(int id)
        {
            return (_context.Cabinets?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
