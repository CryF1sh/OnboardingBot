using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Server;
using Server.Entities;

namespace AdminPanel.Pages.Questions
{
    public class EditModel : PageModel
    {
        private readonly Server.OnboardingBotContext _context;

        public EditModel(Server.OnboardingBotContext context)
        {
            _context = context;
        }

        [BindProperty]
        public UserQuestion UserQuestion { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.UserQuestions == null)
            {
                return NotFound();
            }

            var userquestion =  await _context.UserQuestions.FirstOrDefaultAsync(m => m.Id == id);
            if (userquestion == null)
            {
                return NotFound();
            }
            UserQuestion = userquestion;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(UserQuestion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserQuestionExists(UserQuestion.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool UserQuestionExists(int id)
        {
          return (_context.UserQuestions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
