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
    public class DetailsModel : PageModel
    {
        private readonly Server.OnboardingBotContext _context;

        public DetailsModel(Server.OnboardingBotContext context)
        {
            _context = context;
        }

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
    }
}
