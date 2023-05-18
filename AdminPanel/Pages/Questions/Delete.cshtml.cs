using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Server;
using Server.Entities;

namespace AdminPanel.Pages.Questions
{
    public class DeleteModel : PageModel
    {
        private readonly Server.OnboardingBotContext _context;

        public DeleteModel(Server.OnboardingBotContext context)
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

            var userquestion = await _context.UserQuestions.FirstOrDefaultAsync(m => m.Id == id);

            if (userquestion == null)
            {
                return NotFound();
            }
            else 
            {
                UserQuestion = userquestion;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.UserQuestions == null)
            {
                return NotFound();
            }
            var userquestion = await _context.UserQuestions.FindAsync(id);

            if (userquestion != null)
            {
                UserQuestion = userquestion;
                _context.UserQuestions.Remove(UserQuestion);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
