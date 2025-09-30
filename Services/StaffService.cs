using Microsoft.EntityFrameworkCore;
using RetroTapes.Data;
using RetroTapes.Services;
using RetroTapes.ViewModels;
using AddressEntity = RetroTapes.Models.Address;
using StaffEntity = RetroTapes.Models.Staff;

public class StaffService : IStaffService
{
    private readonly SakilaContext _db;
    public StaffService(SakilaContext db) => _db = db;

    public async Task<List<StaffBasicVm>> ListAsync(CancellationToken ct = default)
    {
        return await _db.Staff
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
            .ToListAsync(ct);
    }

    public async Task<StaffBasicVm?> GetForEditAsync(byte id, CancellationToken ct = default)
    {
        var s = await _db.Staff.AsNoTracking()
            .Include(x => x.Address)
            .FirstOrDefaultAsync(x => x.StaffId == id, ct);

        return s == null ? null : new StaffBasicVm
        {
            StaffId = s.StaffId,
            FirstName = s.FirstName,
            LastName = s.LastName,
            Email = s.Email,
            PhoneNumber = s.Address?.Phone
        };
    }

    public async Task<(bool ok, string? error, byte id)> CreateAsync(StaffBasicVm vm, CancellationToken ct = default)
    {

        AddressEntity? addr = null;
        if (!string.IsNullOrWhiteSpace(vm.PhoneNumber))
        {
            var cityId = await _db.Cities
                .OrderBy(c => c.CityId)
                .Select(c => c.CityId)
                .FirstAsync(ct);

            addr = new AddressEntity
            {
                Address1 = "(not provided)",
                District = "-",
                CityId = cityId,
                PostalCode = null,
                Phone = vm.PhoneNumber,
                LastUpdate = DateTime.UtcNow
            };
            _db.Addresses.Add(addr);
            await _db.SaveChangesAsync(ct); // behövs för AddressId
        }

        var entity = new StaffEntity
        {
            FirstName = vm.FirstName,
            LastName = vm.LastName,
            Email = vm.Email,
            AddressId = addr?.AddressId ?? default, // 0 om ingen address skapades
            StoreId = 1,          // om ni kör default
            Username = (vm.FirstName + "." + vm.LastName).ToLowerInvariant(),
            Active = true,
            LastUpdate = DateTime.UtcNow
        };

        _db.Staff.Add(entity);
        await _db.SaveChangesAsync(ct);

        return (true, null, entity.StaffId);
    }

    public async Task<(bool ok, string? error)> UpdateAsync(StaffBasicVm vm, CancellationToken ct = default)
    {
        var s = await _db.Staff
            .Include(x => x.Address)
            .FirstOrDefaultAsync(x => x.StaffId == vm.StaffId, ct);

        if (s == null) return (false, "Personalen hittades inte.");

        s.FirstName = vm.FirstName;
        s.LastName = vm.LastName;
        s.Email = vm.Email;
        s.LastUpdate = DateTime.UtcNow;

        // Telefon via Address.Phone. Hårdkodat så att en address skapas om den saknas för att möjliggöra lagring av telefonnummer.
        if (!string.IsNullOrWhiteSpace(vm.PhoneNumber))
        {
            if (s.Address == null)
            {
                var cityId = await _db.Cities
                    .OrderBy(c => c.CityId)
                    .Select(c => c.CityId)
                    .FirstAsync(ct);

                s.Address = new AddressEntity
                {
                    Address1 = "(not provided)",
                    District = "-",
                    CityId = cityId,
                    PostalCode = null,
                    Phone = vm.PhoneNumber,
                    LastUpdate = DateTime.UtcNow
                };
            }
            else
            {
                s.Address.Phone = vm.PhoneNumber;
                s.Address.LastUpdate = DateTime.UtcNow;
            }
        }
        await _db.SaveChangesAsync(ct);
        return (true, null);
    }

    public async Task<(bool ok, string? error)> DeleteAsync(byte id, CancellationToken ct = default)
    {
        var s = await _db.Staff.FirstOrDefaultAsync(x => x.StaffId == id, ct);
        if (s == null) return (false, "Personalen hittades inte.");

        bool hasPayments = await _db.Payments.AnyAsync(p => p.StaffId == id, ct);
        bool hasRentals = await _db.Rentals.AnyAsync(r => r.StaffId == id, ct);
        bool isManager = await _db.Stores.AnyAsync(st => st.ManagerStaffId == id, ct);

        if (hasPayments || hasRentals || isManager)
        {
            return (false, "Kan inte ta bort personalen: används i Payments, Rentals eller Stores.");
        }

        _db.Staff.Remove(s);
        await _db.SaveChangesAsync(ct);
        return (true, null);
    }

    public async Task<StaffBasicVm?> GetDeatailAsync(int id)
    {
        return await _db.Staff.AsNoTracking()
            .Select(x => new StaffBasicVm { StaffId = x.StaffId, FirstName = x.FirstName, LastName = x.LastName })
            .FirstOrDefaultAsync(x => x.StaffId == (byte)id);

    }
}
