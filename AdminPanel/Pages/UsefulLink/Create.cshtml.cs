using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Server;
using Server.Entities;

namespace AdminPanel.Pages.UsefulLink
{
    public class CreateModel : PageModel
    {
        private readonly Server.OnboardingBotContext _context;

        public CreateModel(Server.OnboardingBotContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Server.Entities.UsefulLink UsefulLink { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.UsefulLinks == null || UsefulLink == null)
            {
                return Page();
            }

            _context.UsefulLinks.Add(UsefulLink);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
