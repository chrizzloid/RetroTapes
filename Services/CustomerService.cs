using Microsoft.EntityFrameworkCore;
using RetroTapes.Data;
using RetroTapes.Infrastructure;
using RetroTapes.Models;
using RetroTapes.ViewModels;

namespace RetroTapes.Services
{
    public class CustomerService
    {
        private readonly SakilaContext _db;
        public CustomerService(SakilaContext db) => _db = db;

        //Lista/sök 

        public async Task<PagedResult<CustomerListItemVm>> SearchAsync(string? q, bool? active, string? sort, int pageNumber, int pageSize)
        {

            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var qry = _db.Customers.AsNoTracking(); //Snabbare läsning då EF Core inte behöver hålla koll på entiteter för ändring

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                qry = qry.Where(c =>
                (c.FirstName + " " + c.LastName).Contains(q) ||
                (c.LastName + " " + c.FirstName).Contains(q) ||
                (c.Email != null && c.Email.Contains(q)));
            }

            if (active.HasValue)
            {
                var flag = active.Value ? "Y" : "N";
                qry = qry.Where(c => c.Active == flag);
            }

            // Sortering/filtrering här istället för i Index.cshtml.cs
            qry = sort switch
            {
                "rentals" => qry.OrderBy(c => c.Rentals.Count(r => r.ReturnDate == null)),
                "rentals_desc" => qry.OrderByDescending(c => c.Rentals.Count(r => r.ReturnDate == null)),
                "active" => qry.OrderBy(c => c.Active),
                "active_desc" => qry.OrderByDescending(c => c.Active),
                "name" => qry.OrderBy(c => c.LastName).ThenBy(c => c.FirstName),
                "name_desc" => qry.OrderByDescending(c => c.LastName).ThenByDescending(c => c.FirstName),
                "email" => qry.OrderBy(c => c.Email),
                "email_desc" => qry.OrderByDescending(c => c.Email),
                _ => qry.OrderBy(c => c.LastName).ThenBy(c => c.FirstName) // default
            };

            var total = await qry.CountAsync();

            var items = await qry
                //.OrderBy(c => c.LastName).ThenBy(c => c.FirstName) // Denna överrider string? Sort i Customers/Index.cshtml.cs
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CustomerListItemVm
                {
                    CustomerId = c.CustomerId,
                    CustomerName = c.FirstName + " " + c.LastName,
                    Email = c.Email,
                    Active = c.Active == "Y", //Konverterar till bool här då "active" i Sakila är 'varchar(1)'.
                    ActiveRentals = c.Rentals.Count(r => r.ReturnDate == null)
                })
                .ToListAsync();

            return new PagedResult<CustomerListItemVm>
            {
                Items = items,
                TotalCount = total,
                Page = pageNumber,
                PageSize = pageSize
            };
        }

        //Detalj : Aktiva hyror och betalningshistorik
        public async Task<CustomerDetailVm?> GetDetailAsync(int customerId, int maxPayments = 50)
        {

            return await _db.Customers
                .AsNoTracking()
                .Where(c => c.CustomerId == customerId)
                .Select(c => new CustomerDetailVm
                {
                    CustomerId = c.CustomerId,
                    Name = c.FirstName + " " + c.LastName,
                    Email = c.Email,
                    Active = c.Active == "Y",
                    ActiveRentalsCount = c.Rentals.Count(r => r.ReturnDate == null)
                })
                .FirstOrDefaultAsync();
        }


        public async Task<(int customerId, bool created)> UpsertAsync(CustomerEditVm vm)
        {
            Customer entity;

            if (vm.CustomerId.HasValue)
            {
                entity = await _db.Customers
                    .FirstOrDefaultAsync(c => c.CustomerId == vm.CustomerId.Value)
                    ?? throw new KeyNotFoundException("Kunden finns inte");


                if (vm.LastUpdate.HasValue)
                    _db.Entry(entity).Property(e => e.LastUpdate).OriginalValue = vm.LastUpdate.Value;
            }

            else
            {
                entity = new Customer
                {
                    CreateDate = DateTime.UtcNow,
                    LastUpdate = DateTime.UtcNow,
                };
                _db.Customers.Add(entity);
            }

            //Fält-mapping
            entity.FirstName = vm.FirstName.Trim();
            entity.LastName = vm.LastName.Trim();
            entity.Email = string.IsNullOrWhiteSpace(vm.Email) ? null : vm.Email.Trim();
            entity.Active = vm.Active ? "Y" : "N";
            entity.StoreId = vm.StoreId;
            entity.AddressId = vm.AddressId;


            //touch
            entity.LastUpdate = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return (entity.CustomerId, !vm.CustomerId.HasValue);

        }

        public async Task<CustomerEditVm?> GetEditVmAsync(int id)
        {
            return await _db.Customers.AsNoTracking()
                .Where(c => c.CustomerId == id)
                .Select(c => new CustomerEditVm
                {
                    CustomerId = c.CustomerId,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email,
                    Active = c.Active == "Y",
                    StoreId = c.StoreId,
                    AddressId = c.AddressId,

                    LastUpdate = DateTime.UtcNow,
                })
                .FirstOrDefaultAsync();


        }




        //DELETE hård delete

        //public async Task DeleteAsync(int customerId)
        //{
        //    var entity = await _db.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId)
        //            ?? throw new KeyNotFoundException("Kunden finns inte");

        //    _db.Customers.Remove(entity);
        //    await _db.SaveChangesAsync();
        //}

        // Customer Picker dropdown
        public async Task<IReadOnlyList<CustomerListItemVm>> QuickPickAsync(string q, int take = 10)
        {
            q = (q ?? "").Trim();
            if (q.Length == 0) return Array.Empty<CustomerListItemVm>();

            return await _db.Customers.AsNoTracking()
                .Where(c =>
                    (c.FirstName + " " + c.LastName).Contains(q) ||
                    (c.LastName + " " + c.FirstName).Contains(q) ||
                    (c.Email != null && c.Email.Contains(q)))
                .OrderBy(c => c.LastName).ThenBy(c => c.FirstName)
                .Take(take)
                .Select(c => new CustomerListItemVm
                {
                    CustomerId = c.CustomerId,
                    CustomerName = c.FirstName + " " + c.LastName,
                    Email = c.Email,
                    Active = c.Active == "Y", // string -> bool
                    ActiveRentals = c.Rentals.Count(r => r.ReturnDate == null)
                })
                .ToListAsync();

        }

        internal async Task<CustomerDeleteVm?> GetDeleteInfoAsync(int id)
        {
            return await _db.Customers
                .AsNoTracking()
                .Where(c => c.CustomerId == id)
                .Select(c => new CustomerDeleteVm
                {
                    CustomerId = c.CustomerId,
                    Name = c.FirstName + " " + c.LastName,
                    LastUpdate = c.LastUpdate,
                })
                .FirstOrDefaultAsync();

        }

        internal async Task<bool> DeleteAsync(int id, DateTime lastUpdate)
        {
            // kontrollera beroenden
            if (await _db.Rentals.AnyAsync(r => r.CustomerId == id) ||
                await _db.Payments.AnyAsync(p => p.CustomerId == id))
            {
                return false; // har relaterade poster, kan inte ta bort
            }

            // concurrency check
            var customer = new Customer { CustomerId = id, LastUpdate = lastUpdate };
            _db.Entry(customer).Property(c => c.LastUpdate).OriginalValue = lastUpdate;

            _db.Customers.Attach(customer);
            _db.Customers.Remove(customer);

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
