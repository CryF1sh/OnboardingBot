using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Server;
using Server.Entities;

namespace AdminPanel.Pages.Floor
{
    public class DeleteModel : PageModel
    {
        private readonly Server.OnboardingBotContext _context;

        public DeleteModel(Server.OnboardingBotContext context)
        {
            _context = context;
        }

        [BindProperty]
      public FloorLayout FloorLayout { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.FloorLayouts == null)
            {
                return NotFound();
            }

            var floorlayout = await _context.FloorLayouts.FirstOrDefaultAsync(m => m.Id == id);

            if (floorlayout == null)
            {
                return NotFound();
            }
            else 
            {
                FloorLayout = floorlayout;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.FloorLayouts == null)
            {
                return NotFound();
            }
            var floorlayout = await _context.FloorLayouts.FindAsync(id);

            if (floorlayout != null)
            {
                FloorLayout = floorlayout;
                _context.FloorLayouts.Remove(FloorLayout);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
