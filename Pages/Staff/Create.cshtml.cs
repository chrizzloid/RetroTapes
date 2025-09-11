using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RetroTapes.Data;
using RetroTapes.ViewModels;
using StaffEntity = RetroTapes.Models.Staff;
using AddressEntity = RetroTapes.Models.Address;

namespace RetroTapes.Pages.Staff
{
    public class CreateModel : PageModel
    {
        private readonly SakilaContext _db;
        public CreateModel(SakilaContext db) => _db = db;

        [BindProperty] public StaffBasicVm Vm { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var defaultCityId = await _db.Cities
                .OrderBy(c => c.CityId)
                .Select(c => c.CityId)
                .FirstAsync();

            var addr = new AddressEntity
            {
                Address1 = "(not provided)",  // placeholder
                District = "-",               // placeholder
                CityId = defaultCityId,
                PostalCode = null,
                Phone = Vm.PhoneNumber,
                LastUpdate = System.DateTime.UtcNow
            };
            _db.Addresses.Add(addr);
            await _db.SaveChangesAsync(); // får AddressId

            // 2) Skapa Staff (default:a StoreId, Username, Active)
            var staff = new StaffEntity
            {
                FirstName = Vm.FirstName,
                LastName = Vm.LastName,
                Email = Vm.Email,
                AddressId = addr.AddressId,
                StoreId = 1, // enkel default
                Username = (Vm.FirstName + "." + Vm.LastName).ToLowerInvariant(),
                Active = true,
                LastUpdate = System.DateTime.UtcNow
            };

            _db.Staff.Add(staff);
            await _db.SaveChangesAsync();
           
            TempData["StaffFlash"] = "Anställd tillagd!";
            TempData["StaffFlashColor"] = "success"; //grön
            return RedirectToPage("Index");

        }
    }
}
