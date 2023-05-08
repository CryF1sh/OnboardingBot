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
    public class FloorLayoutsController : ControllerBase
    {
        private readonly OnboardingBotContext _context;

        public FloorLayoutsController(OnboardingBotContext context)
        {
            _context = context;
        }

        // GET: api/FloorLayouts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FloorLayout>>> GetFloorLayouts()
        {
          if (_context.FloorLayouts == null)
          {
              return NotFound();
          }
            return await _context.FloorLayouts.ToListAsync();
        }

        // GET: api/FloorLayouts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FloorLayout>> GetFloorLayout(int id)
        {
          if (_context.FloorLayouts == null)
          {
              return NotFound();
          }
            var floorLayout = await _context.FloorLayouts.FindAsync(id);

            if (floorLayout == null)
            {
                return NotFound();
            }

            return floorLayout;
        }

        // PUT: api/FloorLayouts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFloorLayout(int id, FloorLayout floorLayout)
        {
            if (id != floorLayout.Id)
            {
                return BadRequest();
            }

            _context.Entry(floorLayout).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FloorLayoutExists(id))
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

        // POST: api/FloorLayouts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FloorLayout>> PostFloorLayout(FloorLayout floorLayout)
        {
          if (_context.FloorLayouts == null)
          {
              return Problem("Entity set 'OnboardingBotContext.FloorLayouts'  is null.");
          }
            _context.FloorLayouts.Add(floorLayout);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFloorLayout", new { id = floorLayout.Id }, floorLayout);
        }

        // DELETE: api/FloorLayouts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFloorLayout(int id)
        {
            if (_context.FloorLayouts == null)
            {
                return NotFound();
            }
            var floorLayout = await _context.FloorLayouts.FindAsync(id);
            if (floorLayout == null)
            {
                return NotFound();
            }

            _context.FloorLayouts.Remove(floorLayout);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FloorLayoutExists(int id)
        {
            return (_context.FloorLayouts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
