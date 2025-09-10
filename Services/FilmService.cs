using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RetroTapes.Data;
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

            film.LastUpdate = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return (film, created);
        }

    }
}
