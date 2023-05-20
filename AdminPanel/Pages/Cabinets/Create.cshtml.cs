using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Server;
using Server.Entities;

namespace AdminPanel.Pages.Cabinets
{
    public class CreateModel : PageModel
    {
        private readonly Server.OnboardingBotContext _context;
        public List<Server.Entities.FloorLayout> FloorLayouts { get; set; }


        //public CreateModel(Server.OnboardingBotContext context, List<Server.Entities.FloorLayout> floorLayouts)
        //{
        //    _context = context;
        //    FloorLayouts = floorLayouts;
        //}

        public IActionResult OnGet()
        {
            List<Server.Entities.FloorLayout> floorLayouts;
            using (var context = new OnboardingBotContext())
            {
                floorLayouts = context.FloorLayouts.ToList();
            }

            FloorLayouts = floorLayouts;

            return Page();
        }

        [BindProperty]
        public Cabinet Cabinet { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.Cabinets == null || Cabinet == null)
            {
                return Page();
            }

            _context.Cabinets.Add(Cabinet);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
