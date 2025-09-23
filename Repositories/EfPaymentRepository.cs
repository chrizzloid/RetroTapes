using Microsoft.EntityFrameworkCore;
using RetroTapes.Data;
using RetroTapes.Infrastructure;
using RetroTapes.Models;

namespace RetroTapes.Repositories
{
    public sealed class EfPaymentRepository : IPaymentRepository
    {
        private readonly SakilaContext _db;
        public EfPaymentRepository(SakilaContext db) => _db = db;

        public async Task<Payment?> GetByIdAsync(int paymentId, bool includeGraph = false, CancellationToken ct = default)
        {
            IQueryable<Payment> q = _db.Payments;
            if (includeGraph)
            {
                q = q
                    .Include(p => p.Customer)
                    .Include(p => p.Staff)
                    .Include(p => p.Rental)       // rental -> inventory/film kan laddas vid behov senare
                        .ThenInclude(r => r.Inventory)
                            .ThenInclude(i => i.Film);
            }
            return await q.FirstOrDefaultAsync(p => p.PaymentId == paymentId, ct);
        }

        public async Task<PagedResult<Payment>> GetPageAsync(
            int page, int pageSize, int? customerId = null, int? rentalId = null, byte? staffId = null,
            DateTime? from = null, DateTime? to = null, CancellationToken ct = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            var q = _db.Payments.AsNoTracking()
                .Include(p => p.Customer)
                .Include(p => p.Staff)
                .Include(p => p.Rental)
                .AsQueryable();

            if (customerId is int cid) q = q.Where(p => p.CustomerId == cid);
            if (rentalId is int rid) q = q.Where(p => p.RentalId == rid);
            if (staffId is byte sid) q = q.Where(p => p.StaffId == sid);
            if (from is DateTime f) q = q.Where(p => p.PaymentDate >= f);
            if (to is DateTime t) q = q.Where(p => p.PaymentDate < t);

            q = q.OrderByDescending(p => p.PaymentDate).ThenBy(p => p.PaymentId);

            var total = await q.CountAsync(ct);
            var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

            return new PagedResult<Payment> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
        }

        public async Task AddAsync(Payment payment, CancellationToken ct = default)
            => await _db.Payments.AddAsync(payment, ct);

        public Task UpdateAsync(Payment payment, CancellationToken ct = default)
        {
            _db.Payments.Update(payment);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Payment payment, CancellationToken ct = default)
        {
            _db.Payments.Remove(payment);
            return Task.CompletedTask;
        }
    }
}
