using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RetroTapes.Data;
using RetroTapes.Services;
using StaffEntity = RetroTapes.Models.Staff;

namespace RetroTapes.Pages.Staff
{
    public class DeleteModel : PageModel
    {
        private readonly SakilaContext _db; 
        private readonly IStaffService _svc; 
        public DeleteModel(SakilaContext db, IStaffService svc) { _db = db; _svc = svc; }

        [BindProperty] public StaffEntity Staff { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var s = await _db.Staff.AsNoTracking()
                .Select(x => new StaffEntity { StaffId = x.StaffId, FirstName = x.FirstName, LastName = x.LastName })
                .FirstOrDefaultAsync(x => x.StaffId == (byte)id);
            if (s is null) return NotFound();
            Staff = s; return Page();
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
