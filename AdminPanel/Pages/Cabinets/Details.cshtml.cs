using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Server;
using Server.Entities;

namespace AdminPanel.Pages.Cabinets
{
    public class DetailsModel : PageModel
    {
        private readonly Server.OnboardingBotContext _context;

        public DetailsModel(Server.OnboardingBotContext context)
        {
            _context = context;
        }

      public Cabinet Cabinet { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Cabinets == null)
            {
                return NotFound();
            }

            var cabinet = await _context.Cabinets.FirstOrDefaultAsync(m => m.Id == id);
            if (cabinet == null)
            {
                return NotFound();
            }
            else 
            {
                Cabinet = cabinet;
            }
            return Page();
        }
    }
}
