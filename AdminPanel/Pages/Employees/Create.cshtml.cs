using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Server;
using Server.Entities;

namespace AdminPanel.Pages.Employees
{
    public class CreateModel : PageModel
    {
        private readonly Server.OnboardingBotContext _context;
        public List<Direction> AvailableDirections { get; set; }
        public List<int> SelectedDirections { get; set; }

        public CreateModel(Server.OnboardingBotContext context)
        {
            _context = context;

        }

        public IActionResult OnGet()
        {
            AvailableDirections = _context.Directions.ToList();

            return Page();
        }

        [BindProperty]
        public Employee Employee { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            var selectedDirections = Request.Form["SelectedDirectionIds"]
           .Select(x => int.Parse(x))
           .ToList(); // Преобразование выбранных значений в тип данных int

            if (!ModelState.IsValid || _context.Employees == null || Employee == null)
            {
                // Проверка валидации выбранных направлений
                if (selectedDirections.Count == 0)
                {
                    ModelState.AddModelError("SelectedDirections", "Выберите хотя бы одно направление.");
                }
                return Page();
            }

            _context.Employees.Add(Employee);
            await _context.SaveChangesAsync();

            // Обработка второго POST-запроса для сохранения выбранных направлений
            foreach (int directionId in selectedDirections)
            {
                EmployeeDirection employeeDirection = new EmployeeDirection
                {
                    EmployeeId = Employee.Id,
                    DirectionId = directionId
                };

                _context.EmployeeDirections.Add(employeeDirection);
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
