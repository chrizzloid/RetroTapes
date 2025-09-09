using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RetroTapes.Data;
using RetroTapes.Infrastructure;
using RetroTapes.ViewModels;

namespace RetroTapes.Pages.Films
{
    public class IndexModel : PageModel
    {
        private readonly SakilaContext _db;
        public IndexModel(SakilaContext db) => _db = db;

        [BindProperty(SupportsGet = true)] public string? q { get; set; }
        [BindProperty(SupportsGet = true)] public byte? categoryId { get; set; }
        [BindProperty(SupportsGet = true)] public byte? languageId { get; set; }
        [BindProperty(SupportsGet = true)] public string? sort { get; set; }
        [BindProperty(SupportsGet = true)] public int page { get; set; } = 1;
        [BindProperty(SupportsGet = true)] public int pageSize { get; set; } = 20;

        public PagedResult<FilmListItemVm> Result { get; set; } = new();
        public SelectList CategoryOptions { get; set; } = default!;
        public SelectList LanguageOptions { get; set; } = default!;

        public async Task OnGetAsync()
        {
            var query = _db.Films.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();
                query = query.Where(f => f.Title.Contains(term) || (f.Description != null && f.Description.Contains(term)));
            }

            if (languageId.HasValue)
                query = query.Where(f => f.LanguageId == languageId.Value);

            if (categoryId.HasValue)
                query = query.Where(f => f.FilmCategories.Any(fc => fc.CategoryId == categoryId.Value));

            query = sort switch
            {
                "title" => query.OrderBy(f => f.Title),
                "title_desc" => query.OrderByDescending(f => f.Title),
                "year" => query.OrderBy(f => f.ReleaseYear),
                "year_desc" => query.OrderByDescending(f => f.ReleaseYear),
                _ => query.OrderByDescending(f => f.LastUpdate)
            };

            var total = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(f => new FilmListItemVm
                {
                    FilmId = f.FilmId,
                    Title = f.Title,
                    ReleaseYear = f.ReleaseYear,
                    LanguageName = f.Language.Name,
                    Categories = string.Join(", ", f.FilmCategories.Select(fc => fc.Category.Name).OrderBy(n => n)),
                    LastUpdate = f.LastUpdate
                })
                .ToListAsync();

            Result = new PagedResult<FilmListItemVm>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };

            CategoryOptions = new SelectList(
                await _db.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync(),
                "CategoryId", "Name");

            LanguageOptions = new SelectList(
                await _db.Languages.AsNoTracking().OrderBy(l => l.Name).ToListAsync(),
                "LanguageId", "Name");
        }


    }
}

