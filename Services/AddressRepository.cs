using Microsoft.EntityFrameworkCore;
using RetroTapes.Data;
using RetroTapes.Infrastructure;
using RetroTapes.ViewModels;

namespace RetroTapes.Services
{
    public class AddressRepository
    {
        private readonly SakilaContext _db;
        public AddressRepository(SakilaContext db) => _db = db;

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

        public async Task<List<CityVm>> GetCitiesAsync()
        {
            return await _db.Cities.AsNoTracking()
                .OrderBy(c => c.City1)
                .Select(c => new CityVm
                {
                    CityId = c.CityId,
                    Name = c.City1
                })
                .ToListAsync();
        }
    }
}
