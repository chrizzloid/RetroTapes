using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RetroTapes.Data;

namespace RetroTapes.Pages.Staff
{
    public class ChooseModel : PageModel
    {
        private readonly SakilaContext _db;
        public ChooseModel(SakilaContext db) => _db = db;

        public List<(int Id, string Name)> Items { get; set; } = new();
        public string ReturnUrl { get; set; } = "/";

        public async Task OnGetAsync(string? returnUrl)
        {
            ReturnUrl = string.IsNullOrWhiteSpace(returnUrl)
                ? (Request.Headers["Referer"].ToString() ?? "/")
                : returnUrl;

            var raw = await _db.Staff
                .AsNoTracking()
                .OrderBy(s => s.LastName).ThenBy(s => s.FirstName)
                .Select(s => new
                {
                    Id = (int)s.StaffId,
                    Name = s.FirstName + " " + s.LastName
                })
                .ToListAsync();

            Items = raw.Select(x => (x.Id, x.Name)).ToList();
        }
    }
}