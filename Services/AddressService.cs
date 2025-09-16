using Microsoft.EntityFrameworkCore;
using RetroTapes.Data;
using RetroTapes.Models;
using RetroTapes.ViewModels;

namespace RetroTapes.Services
{
    public class AddressService
    {
        private readonly SakilaContext _db;
        public AddressService(SakilaContext db) => _db = db;

        public async Task<(Address address, bool created)> UpsertAsync(AddressEditVm vm)
        {
            Address address;
            var created = false;

            if (vm.AddressId.HasValue)
            {
                address = await _db.Addresses
                    //.Include(f => f.FilmCategories)
                    //.Include(f => f.FilmActors)
                    .FirstOrDefaultAsync(a => a.AddressId == vm.AddressId.Value)
                    ?? throw new KeyNotFoundException("Adressen finns inte.");

                if (vm.LastUpdate.HasValue)
                    _db.Entry(address).Property(a => a.LastUpdate).OriginalValue = vm.LastUpdate.Value;
            }
            else
            {
                address = new Address();
                _db.Addresses.Add(address);
                created = true;
            }

            // Mappa fält
            address.Address1 = vm.Address.Trim();
            address.CityId = vm.CityId;
            address.PostalCode = vm.PostalCode;

            await _db.SaveChangesAsync();
            return (address, created);
        }
    }
}
