using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RetroTapes.Services;
using RetroTapes.ViewModels;

namespace RetroTapes.Pages.Staff
{
    public class EditModel : PageModel
    {
        private readonly IStaffService _svc;
        public EditModel(IStaffService svc) => _svc = svc;

        [BindProperty] public StaffBasicVm Vm { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var vm = await _svc.GetForEditAsync((byte)id);
            if (vm is null) return NotFound();
            Vm = vm;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var (ok, err) = await _svc.UpdateAsync(Vm);
            if (!ok) { ModelState.AddModelError("", err ?? "Kunde inte spara ändringar."); return Page(); }

            TempData["StaffFlash"] = "Ändringar sparade";
            TempData["StaffFlashColor"] = "warning"; // gul
            return RedirectToPage("Index");
        }
    }
}
