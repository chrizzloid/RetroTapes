using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RetroTapes.Data;
using RetroTapes.ViewModels;

namespace RetroTapes.Pages.Staff
{
    public class DetailsModel : PageModel
    {
        private readonly SakilaContext _db;
        public DetailsModel(SakilaContext db) => _db = db;

        public StaffBasicVm Vm { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(byte? id)
        {
            if (id is null) return NotFound();

            var s = await _db.Staff.Include(x => x.Address)
                                   .FirstOrDefaultAsync(x => x.StaffId == id.Value);
            if (s == null) return NotFound();

            Vm = new StaffBasicVm
            {
                StaffId = s.StaffId,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email,
                PhoneNumber = s.Address?.Phone
            };
            return Page();
        }
    }
}
