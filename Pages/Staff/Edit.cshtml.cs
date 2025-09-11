using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RetroTapes.Data;
using RetroTapes.ViewModels;
using StaffEntity = RetroTapes.Models.Staff;

namespace RetroTapes.Pages.Staff
{
    public class EditModel : PageModel
    {
        private readonly SakilaContext _db;
        public EditModel(SakilaContext db) => _db = db;

        [BindProperty] public StaffBasicVm Vm { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(byte? id)
        {
            if (id is null) return NotFound();

            var s = await _db.Staff
                .Include(x => x.Address)
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var s = await _db.Staff.Include(x => x.Address)
                                   .FirstOrDefaultAsync(x => x.StaffId == Vm.StaffId);
            if (s == null) return NotFound();

            s.FirstName = Vm.FirstName;
            s.LastName = Vm.LastName;
            s.Email = Vm.Email;
            s.LastUpdate = System.DateTime.UtcNow;

            if (s.Address == null)
            {            
                var defaultCityId = await _db.Cities.OrderBy(c => c.CityId).Select(c => c.CityId).FirstAsync();
                s.Address = new RetroTapes.Models.Address
                {
                    Address1 = "(not provided)",
                    District = "-",
                    CityId = defaultCityId,
                    PostalCode = null,
                    Phone = Vm.PhoneNumber,
                    LastUpdate = System.DateTime.UtcNow
                };
            }
            else
            {
                s.Address.Phone = Vm.PhoneNumber;
                s.Address.LastUpdate = System.DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
            TempData["StaffFlash"] = "Ändringar sparade";
            TempData["StaffFlashColor"] = "warning"; // gul
            return RedirectToPage("Index");

        }
    }
}
