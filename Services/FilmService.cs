using Microsoft.EntityFrameworkCore;
using RetroTapes.Data;
using RetroTapes.Infrastructure;
using RetroTapes.Models;
using RetroTapes.ViewModels;


namespace RetroTapes.Services
{
    public class FilmService
    {
        private readonly SakilaContext _db;
        public FilmService(SakilaContext db) => _db = db;

        public async Task<(Film film, bool created)> UpsertAsync(FilmEditVm vm)
        {
            Film film;
            var created = false;

            if (vm.FilmId.HasValue)
            {
                film = await _db.Films
                    .Include(f => f.FilmCategories)
                    .Include(f => f.FilmActors)
                    .FirstOrDefaultAsync(f => f.FilmId == vm.FilmId.Value)
                    ?? throw new KeyNotFoundException("Filmen finns inte.");

                if (vm.LastUpdate.HasValue)
                    _db.Entry(film).Property(f => f.LastUpdate).OriginalValue = vm.LastUpdate.Value;
            }
            else
            {
                film = new Film();
                _db.Films.Add(film);
                created = true;
            }

            // Map fält
            film.Title = vm.Title.Trim();
            film.Description = vm.Description;
            film.ReleaseYear = vm.ReleaseYear;
            film.LanguageId = vm.LanguageId;

            // Shadow property för OriginalLanguageId om den inte finns som riktig prop
            if (vm.OriginalLanguageId.HasValue)
            {
                var prop = _db.Entry(film).Metadata.FindProperty("OriginalLanguageId");
                if (prop != null)
                {
                    _db.Entry(film).Property("OriginalLanguageId").CurrentValue = vm.OriginalLanguageId.Value;
                }
            }

            // Many-to-many: kategorier
            var existingCatIds = film.FilmCategories.Select(fc => fc.CategoryId).ToHashSet();
            var targetCatIds = (vm.CategoryIds ?? new List<byte>()).Distinct().ToHashSet();

            foreach (var rm in film.FilmCategories.Where(fc => !targetCatIds.Contains(fc.CategoryId)).ToList())
                _db.FilmCategories.Remove(rm);

            foreach (var cid in targetCatIds.Except(existingCatIds))
                film.FilmCategories.Add(new FilmCategory { CategoryId = cid });

            // Many-to-many: skådespelare
            var existingActorIds = film.FilmActors.Select(fa => fa.ActorId).ToHashSet();
            var targetActorIds = (vm.ActorIds ?? new List<int>()).Distinct().ToHashSet();

            foreach (var rm in film.FilmActors.Where(fa => !targetActorIds.Contains(fa.ActorId)).ToList())
                _db.FilmActors.Remove(rm);

            foreach (var aid in targetActorIds.Except(existingActorIds))
                film.FilmActors.Add(new FilmActor { ActorId = aid });

            await _db.SaveChangesAsync();
            return (film, created);
        }

        public async Task<PagedResult<FilmListItemVm>> SearchAsync(string q, int? languageId, int? categoryId, string? sort, int pageIndex, int pageSize)
        {
            var query = _db.Films.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();
                query = query.Where(f => f.Title.Contains(term)
                    || (f.Description != null && f.Description.Contains(term)));
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

            // Clamp page så vi inte skippas förbi slutet
            var totalPages = (int)Math.Ceiling(total / (double)pageSize);
            if (totalPages == 0) totalPages = 1;
            if (pageIndex < 1) pageIndex = 1;
            if (pageIndex > totalPages) pageIndex = totalPages;

            var items = await query
                .Skip((pageIndex - 1) * pageSize)
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

            return new PagedResult<FilmListItemVm>
            {
                Items = items,
                TotalCount = total,
                Page = pageIndex,
                PageSize = pageSize
            };

        }

        internal async Task<FilmEditVm?> GetEditVmAsync(int id)
        {
            return await _db.Films
                .AsNoTracking()
                .Where(f => f.FilmId == id)
                .Select(f => new FilmEditVm
                {
                    FilmId = f.FilmId,
                    Title = f.Title,
                    Description = f.Description,
                    ReleaseYear = f.ReleaseYear,
                    LanguageId = f.LanguageId,
                    LastUpdate = f.LastUpdate,
                    CategoryIds = f.FilmCategories.Select(fc => fc.CategoryId).ToList(),
                    ActorIds = f.FilmActors.Select(fa => fa.ActorId).ToList(),
                    OriginalLanguageId = f.OriginalLanguageId

                })
                .FirstOrDefaultAsync();
        }

        internal async Task<FilmDetailVm?> GetDeatailAsync(int id)
        {
            return await _db.Films
                .AsNoTracking()
                .Where(x => x.FilmId == id)
                .Select(x => new FilmDetailVm
                {
                    FilmId = x.FilmId,
                    Title = x.Title,
                    Description = x.Description,
                    ReleaseYear = x.ReleaseYear,
                    Language = x.Language.Name,
                    Categories = string.Join(", ", x.FilmCategories.Select(fc => fc.Category.Name).OrderBy(n => n)),
                    Actors = string.Join(", ", x.FilmActors.Select(fa => fa.Actor.FirstName + " " + fa.Actor.LastName).OrderBy(n => n)),
                    LastUpdate = x.LastUpdate
                })
                .FirstOrDefaultAsync();

        }

        internal async Task<bool> DeleteAsync(int id, DateTime lastUpdate)
        {
            // kolla inventory
            if (await _db.Inventories.AnyAsync(inv => inv.FilmId == id))
                return false;

            // rensa M:N relationer
            _db.FilmCategories.RemoveRange(_db.FilmCategories.Where(fc => fc.FilmId == id));
            _db.FilmActors.RemoveRange(_db.FilmActors.Where(fa => fa.FilmId == id));

            // concurrency check
            var film = new Film { FilmId = id, LastUpdate = lastUpdate };
            _db.Entry(film).Property(f => f.LastUpdate).OriginalValue = lastUpdate;

            _db.Films.Attach(film);
            _db.Films.Remove(film);

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
