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

namespace AdminPanel.Pages.UsefulLink
{
    public class EditModel : PageModel
    {
        private readonly Server.OnboardingBotContext _context;

        public EditModel(Server.OnboardingBotContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Server.Entities.UsefulLink UsefulLink { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.UsefulLinks == null)
            {
                return NotFound();
            }

            var usefullink =  await _context.UsefulLinks.FirstOrDefaultAsync(m => m.Id == id);
            if (usefullink == null)
            {
                return NotFound();
            }
            UsefulLink = usefullink;
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

            _context.Attach(UsefulLink).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsefulLinkExists(UsefulLink.Id))
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

        private bool UsefulLinkExists(int id)
        {
          return (_context.UsefulLinks?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
