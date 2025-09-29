using Microsoft.EntityFrameworkCore;
using RetroTapes.Data;
using RetroTapes.Models;

namespace RetroTapes.Services
{
    public class LookupService
    {
        private readonly SakilaContext _db;
        public LookupService(SakilaContext db) => _db = db;

        public Task<List<Language>> GetLanguagesAsync() =>
            _db.Languages.AsNoTracking().OrderBy(l => l.Name).ToListAsync();

        public Task<List<Category>> GetCategoriesAsync() =>
            _db.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync();

        public Task<List<Actor>> GetActorsAsync() =>
            _db.Actors.AsNoTracking().OrderBy(a => a.LastName).ThenBy(a => a.FirstName).ToListAsync();

        public Task<List<Address>> GetAddressesAsync() =>
            _db.Addresses.AsNoTracking().OrderBy(a => a.Address1).ThenBy(a => a.AddressId).ToListAsync();

        public Task<List<Store>> GetStoresAsync() =>
            _db.Stores.AsNoTracking().OrderBy(s => s.StoreId).ToListAsync();
    }
}
