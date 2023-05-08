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
    public class UserQuestionsController : ControllerBase
    {
        private readonly OnboardingBotContext _context;

        public UserQuestionsController(OnboardingBotContext context)
        {
            _context = context;
        }

        // GET: api/UserQuestions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserQuestion>>> GetUserQuestions()
        {
          if (_context.UserQuestions == null)
          {
              return NotFound();
          }
            return await _context.UserQuestions.ToListAsync();
        }

        // GET: api/UserQuestions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserQuestion>> GetUserQuestion(int id)
        {
          if (_context.UserQuestions == null)
          {
              return NotFound();
          }
            var userQuestion = await _context.UserQuestions.FindAsync(id);

            if (userQuestion == null)
            {
                return NotFound();
            }

            return userQuestion;
        }

        // PUT: api/UserQuestions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserQuestion(int id, UserQuestion userQuestion)
        {
            if (id != userQuestion.Id)
            {
                return BadRequest();
            }

            _context.Entry(userQuestion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserQuestionExists(id))
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

        // POST: api/UserQuestions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserQuestion>> PostUserQuestion(UserQuestion userQuestion)
        {
          if (_context.UserQuestions == null)
          {
              return Problem("Entity set 'OnboardingBotContext.UserQuestions'  is null.");
          }
            _context.UserQuestions.Add(userQuestion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserQuestion", new { id = userQuestion.Id }, userQuestion);
        }

        // DELETE: api/UserQuestions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserQuestion(int id)
        {
            if (_context.UserQuestions == null)
            {
                return NotFound();
            }
            var userQuestion = await _context.UserQuestions.FindAsync(id);
            if (userQuestion == null)
            {
                return NotFound();
            }

            _context.UserQuestions.Remove(userQuestion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserQuestionExists(int id)
        {
            return (_context.UserQuestions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
