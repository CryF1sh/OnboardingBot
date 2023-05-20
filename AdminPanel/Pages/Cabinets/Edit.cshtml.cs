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

namespace AdminPanel.Pages.Cabinets
{
    public class EditModel : PageModel
    {
        private readonly Server.OnboardingBotContext _context;
        public List<Server.Entities.FloorLayout> FloorLayouts { get; set; }

        public EditModel(Server.OnboardingBotContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Cabinet Cabinet { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Cabinets == null)
            {
                return NotFound();
            }

            List<Server.Entities.FloorLayout> floorLayouts;
            using (var context = new OnboardingBotContext())
            {
                floorLayouts = context.FloorLayouts.ToList();
            }

            FloorLayouts = floorLayouts;

            return Page();

            var cabinet =  await _context.Cabinets.FirstOrDefaultAsync(m => m.Id == id);
            if (cabinet == null)
            {
                return NotFound();
            }
            Cabinet = cabinet;
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

            _context.Attach(Cabinet).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CabinetExists(Cabinet.Id))
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

        private bool CabinetExists(int id)
        {
          return (_context.Cabinets?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
