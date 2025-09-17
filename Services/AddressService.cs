using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RetroTapes.Data;
using RetroTapes.Infrastructure;
using RetroTapes.Models;
using RetroTapes.ViewModels;

namespace RetroTapes.Services
{
    public class AddressService
    {
        private readonly SakilaContext _db;
        public AddressService(SakilaContext db) => _db = db;


        public async Task<PagedResult<AddressListItemVm>> SearchAsync(
            string? q, int? cityId, string? sort, int pageIndex, int pageSize)
        {
            var query = _db.Addresses.Include(a => a.City).ThenInclude(c => c.Country).AsNoTracking().AsQueryable();

            // Filter
            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();
                query = query.Where(a => a.Address1.Contains(term)
                    || a.City.City1.Contains(term)
                    || a.City.Country.Country1.Contains(term));
            }

            if (cityId.HasValue)
                query = query.Where(a => a.CityId == cityId.Value);

            //Sortering
            query = sort switch
            {
                "address" => query.OrderBy(a => a.Address1),
                "address_desc" => query.OrderByDescending(a => a.Address1),
                "city" => query.OrderBy(a => a.City.City1),
                "city_desc" => query.OrderByDescending(a => a.City.City1),
                _ => query.OrderByDescending(a => a.LastUpdate)
            };

            // Räkna sidor
            var total = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(total / (double)pageSize);
            if (totalPages == 0) totalPages = 1;
            if (pageIndex < 1) pageIndex = 1;
            if (pageIndex > totalPages) pageIndex = totalPages;

            // Hämta data
            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AddressListItemVm
                {
                    AddressId = a.AddressId,
                    Address = a.Address1,
                    City = a.City.City1,
                    PostalCode = a.PostalCode,
                    Country = a.City.Country.Country1
                })
                .ToListAsync();

            return new PagedResult<AddressListItemVm>
            {
                Items = items,
                TotalCount = total,
                Page = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<CityVm>> GetCitiesAsync()
        {
            return await _db.Cities.AsNoTracking()
                .OrderBy(c => c.City1)
                .Select(c => new CityVm
                {
                    CityId = c.CityId,
                    CityName = c.City1
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetCityOptionsAsync()
        {
            var cities = await _db.Cities
                .AsNoTracking()
                .OrderBy(c => c.City1)
                .Select(c => new SelectListItem
                {
                    Value = c.CityId.ToString(),
                    Text = c.City1.ToString()
                })
                .ToListAsync();
            return cities;
        }

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
            address.Phone = vm.Phone;

            await _db.SaveChangesAsync();
            return (address, created);
        }

        public async Task<AddressDetailVm?> GetDetailAsync(int addressId)
        {

            return await _db.Addresses
                .AsNoTracking()
                .Where(a => a.AddressId == addressId)
                .Select(a => new AddressDetailVm
                {
                    AddressId = a.AddressId,
                    Address = a.Address1,
                    City = a.City.City1,
                    PostalCode = a.PostalCode,
                    Country = a.City.Country.Country1,
                    Phone = a.Phone
                })
                .FirstOrDefaultAsync();
        }




    }
}
