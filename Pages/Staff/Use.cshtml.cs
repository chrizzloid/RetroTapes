using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RetroTapes.Data;

namespace RetroTapes.Pages.Staff
{
    public class UseModel : PageModel
    {
        private readonly SakilaContext _db;
        public UseModel(SakilaContext db) => _db = db;

        public async Task<IActionResult> OnGetAsync(int id, string? returnUrl)
        {
            if (id < byte.MinValue || id > byte.MaxValue) return NotFound();
            var key = (byte)id;

            var staff = await _db.Staff.AsNoTracking()
                .Where(s => s.StaffId == key)
                .Select(s => new { s.StaffId, Name = s.FirstName + " " + s.LastName })
                .FirstOrDefaultAsync();

            if (staff is null) return NotFound();

            HttpContext.Session.SetInt32("ActiveStaffId", staff.StaffId);
            HttpContext.Session.SetString("ActiveStaffName", staff.Name);

            // Info-banner på index (du använder "TempFlashColor")
            TempData["StaffFlash"] = $"Aktiv anställd: {staff.Name}";
            TempData["StaffFlashColor"] = "info";

            var back = string.IsNullOrWhiteSpace(returnUrl) ? (Request.Headers["Referer"].ToString() ?? Url.Page("/Index")!) : returnUrl;
            return Redirect(back);
        }
    }
}