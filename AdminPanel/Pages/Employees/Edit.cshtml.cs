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

namespace AdminPanel.Pages.Employees
{
    public class EditModel : PageModel
    {
        private readonly Server.OnboardingBotContext _context;

        public EditModel(Server.OnboardingBotContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Employee Employee { get; set; } = default!;

        public List<Direction> Directions { get; set; }

        [BindProperty]
        public List<int> SelectedDirectionIds { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee =  await _context.Employees.FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }
            Employee = employee;
            var selectedDirectionIds = _context.EmployeeDirections
                .Where(ed => ed.EmployeeId == employee.Id)
                .Select(ed => ed.DirectionId)
                .ToList();

            SelectedDirectionIds = selectedDirectionIds;

            Directions = _context.Directions.ToList();
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Directions = _context.Directions.ToList();
                return Page();
            }

            // Получаем данные о сотруднике из базы данных
            var employee = await _context.Employees.FindAsync(Employee.Id);

            if (employee == null)
            {
                return NotFound();
            }

            // Обновляем свойства сотрудника
            employee.LastName = Employee.LastName;
            employee.FirstName = Employee.FirstName;
            employee.MiddleName = Employee.MiddleName;
            employee.Position = Employee.Position;
            employee.Description = Employee.Description;
            employee.PhotoLink = Employee.PhotoLink;
            employee.Email = Employee.Email;
            employee.Telephone = Employee.Telephone;
            employee.VkLink = Employee.VkLink;
            employee.TelegramLink = Employee.TelegramLink;

            // Удаляем существующие записи в таблице EmployeeDirections для данного сотрудника
            var existingDirections = _context.EmployeeDirections
                .Where(ed => ed.EmployeeId == employee.Id)
                .ToList();

            _context.EmployeeDirections.RemoveRange(existingDirections);

            // Добавляем новые записи в таблицу EmployeeDirections на основе выбранных направлений
            foreach (var directionId in SelectedDirectionIds)
            {
                var employeeDirection = new EmployeeDirection
                {
                    EmployeeId = employee.Id,
                    DirectionId = directionId
                };

                _context.EmployeeDirections.Add(employeeDirection);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(Employee.Id))
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


        private bool EmployeeExists(int id)
        {
          return (_context.Employees?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
