using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RetroTapes.Data;
using RetroTapes.ViewModels;

namespace RetroTapes.Pages.Staff
{
    public class IndexModel : PageModel
    {
        private readonly SakilaContext _db;
        public IndexModel(SakilaContext db) => _db = db;

        public List<StaffBasicVm> Items { get; set; } = new();

        public async Task OnGetAsync()
        {
            Items = await _db.Staff
                .AsNoTracking()
                .Include(s => s.Address)
                .OrderBy(s => s.LastName).ThenBy(s => s.FirstName)
                .Select(s => new StaffBasicVm
                {
                    StaffId = s.StaffId,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Email = s.Email,
                    PhoneNumber = s.Address != null ? s.Address.Phone : null
                })
                .ToListAsync();
        }
    }
}
