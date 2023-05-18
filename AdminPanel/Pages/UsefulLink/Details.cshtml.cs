using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Server;
using Server.Entities;

namespace AdminPanel.Pages.UsefulLink
{
    public class DetailsModel : PageModel
    {
        private readonly Server.OnboardingBotContext _context;

        public DetailsModel(Server.OnboardingBotContext context)
        {
            _context = context;
        }

      public Server.Entities.UsefulLink UsefulLink { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.UsefulLinks == null)
            {
                return NotFound();
            }

            var usefullink = await _context.UsefulLinks.FirstOrDefaultAsync(m => m.Id == id);
            if (usefullink == null)
            {
                return NotFound();
            }
            else 
            {
                UsefulLink = usefullink;
            }
            return Page();
        }
    }
}
