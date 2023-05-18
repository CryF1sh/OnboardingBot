﻿using System;
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
    public class IndexModel : PageModel
    {
        private readonly Server.OnboardingBotContext _context;

        public IndexModel(Server.OnboardingBotContext context)
        {
            _context = context;
        }

        public IList<Cabinet> Cabinet { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Cabinets != null)
            {
                Cabinet = await _context.Cabinets.ToListAsync();
            }
        }
    }
}
