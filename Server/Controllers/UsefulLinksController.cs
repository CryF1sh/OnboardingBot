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
    public class UsefulLinksController : ControllerBase
    {
        private readonly OnboardingBotContext _context;

        public UsefulLinksController(OnboardingBotContext context)
        {
            _context = context;
        }

        // GET: api/UsefulLinks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsefulLink>>> GetUsefulLinks()
        {
          if (_context.UsefulLinks == null)
          {
              return NotFound();
          }
            return await _context.UsefulLinks.ToListAsync();
        }

        // GET: api/UsefulLinks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UsefulLink>> GetUsefulLink(int id)
        {
          if (_context.UsefulLinks == null)
          {
              return NotFound();
          }
            var usefulLink = await _context.UsefulLinks.FindAsync(id);

            if (usefulLink == null)
            {
                return NotFound();
            }

            return usefulLink;
        }

        // PUT: api/UsefulLinks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsefulLink(int id, UsefulLink usefulLink)
        {
            if (id != usefulLink.Id)
            {
                return BadRequest();
            }

            _context.Entry(usefulLink).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsefulLinkExists(id))
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

        // POST: api/UsefulLinks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UsefulLink>> PostUsefulLink(UsefulLink usefulLink)
        {
          if (_context.UsefulLinks == null)
          {
              return Problem("Entity set 'OnboardingBotContext.UsefulLinks'  is null.");
          }
            _context.UsefulLinks.Add(usefulLink);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsefulLink", new { id = usefulLink.Id }, usefulLink);
        }

        // DELETE: api/UsefulLinks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsefulLink(int id)
        {
            if (_context.UsefulLinks == null)
            {
                return NotFound();
            }
            var usefulLink = await _context.UsefulLinks.FindAsync(id);
            if (usefulLink == null)
            {
                return NotFound();
            }

            _context.UsefulLinks.Remove(usefulLink);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsefulLinkExists(int id)
        {
            return (_context.UsefulLinks?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
