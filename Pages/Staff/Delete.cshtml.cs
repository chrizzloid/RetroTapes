using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RetroTapes.Data;
using RetroTapes.Models;
using StaffEntity = RetroTapes.Models.Staff;

namespace RetroTapes.Pages.Staff
{
    public class DeleteModel : PageModel
    {
        private readonly RetroTapes.Data.SakilaContext _context;

        public DeleteModel(RetroTapes.Data.SakilaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public StaffEntity Staff { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _context.Staff.FirstOrDefaultAsync(m => m.StaffId == id);

            if (staff is not null)
            {
                Staff = staff;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _context.Staff.FindAsync(id);
            if (staff != null)
            {
                Staff = staff;
                _context.Staff.Remove(Staff);
                await _context.SaveChangesAsync();
            }
            TempData["StaffFlash"] = "Anställd borttagen!";
            TempData["StaffFlashColor"] = "danger"; //röd
            return RedirectToPage("./Index");
        }
    }
}
