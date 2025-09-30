using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RetroTapes.Services;
using RetroTapes.ViewModels;
using StaffEntity = RetroTapes.Models.Staff;

namespace RetroTapes.Pages.Staff
{
    public class DeleteModel : PageModel
    {
        private readonly IStaffService _svc;
        public DeleteModel(IStaffService svc) { _svc = svc; }

        [BindProperty] public StaffEntity Staff { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            StaffBasicVm staff = await _svc.GetDeatailAsync(id);
            if (staff == null) return NotFound();

            Staff = new StaffEntity
            {
                StaffId = staff.StaffId,
                FirstName = staff.FirstName,
                LastName = staff.LastName
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var (ok, err) = await _svc.DeleteAsync((byte)id);
            if (!ok) { ModelState.AddModelError("", err ?? "Kunde inte ta bort."); return await OnGetAsync(id); }

            TempData["StaffFlash"] = "Anställd borttagen";
            TempData["StaffFlashColor"] = "danger"; //röd
            return RedirectToPage("Index");
        }
    }
}
