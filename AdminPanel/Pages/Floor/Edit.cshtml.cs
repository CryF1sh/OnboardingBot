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

namespace AdminPanel.Pages.Floor
{
    public class EditModel : PageModel
    {
        private readonly Server.OnboardingBotContext _context;

        public EditModel(Server.OnboardingBotContext context)
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

            var floorlayout =  await _context.FloorLayouts.FirstOrDefaultAsync(m => m.Id == id);
            if (floorlayout == null)
            {
                return NotFound();
            }
            FloorLayout = floorlayout;
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

            _context.Attach(FloorLayout).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FloorLayoutExists(FloorLayout.Id))
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

        private bool FloorLayoutExists(int id)
        {
          return (_context.FloorLayouts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
