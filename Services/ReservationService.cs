using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using RetroTapes.Data;
using RetroTapes.Models;
using RetroTapes.Repositories;

namespace RetroTapes.Services
{
    // Enkel service där jag samlar regler så PageModels hålls tunna.
    public sealed class ReservationService
    {
        private readonly IReservationRepository _reservations;
        private readonly IRentalRepository _rentals;
        private readonly SakilaContext _db; // vi sparar i service för enkelhet

        public ReservationService(IReservationRepository reservations, IRentalRepository rentals, SakilaContext db)
        {
            _reservations = reservations;
            _rentals = rentals;
            _db = db;
        }

        // Skapa bokning med grundvalidering
        public async Task<int> CreateAsync(int customerId, int inventoryId, int holdHours = 24, CancellationToken ct = default)
        {
            if (holdHours < 1 || holdHours > 72)
                throw new ValidationException("HoldHours måste vara mellan 1 och 72 timmar.");

            // 1) Får inte vara aktiv uthyrning på exemplaret
            if (await _rentals.InventoryHasActiveRentalAsync(inventoryId, ct))
                throw new ValidationException("Detta exemplar är redan uthyrt.");

            // 2) Får inte finnas en annan aktiv (inte utgången) reservation på samma exemplar
            if (await _reservations.HasActiveReservationAsync(inventoryId, ct))
                throw new ValidationException("Det finns redan en aktiv bokning på detta exemplar.");

            var res = new Reservation
            {
                CustomerId = customerId,
                InventoryId = inventoryId,
                ReservedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(holdHours),
                Status = ReservationStatus.Active
            };

            await _reservations.AddAsync(res, ct);
            await _db.SaveChangesAsync(ct); // spara direkt för att få ID
            return res.ReservationId;
        }

        // Konvertera bokning till Rental när kunden hämtar (fulfill)
        public async Task<int> FulfillToRentalAsync(int reservationId, byte staffId, CancellationToken ct = default)
        {
            var res = await _reservations.GetByIdAsync(reservationId, includeGraph: false, ct)
                      ?? throw new KeyNotFoundException("Reservationen hittades inte.");

            if (res.Status != ReservationStatus.Active)
                throw new ValidationException("Endast aktiva bokningar kan hämtas ut.");

            if (res.ExpiresAt <= DateTime.UtcNow)
                throw new ValidationException("Bokningen har gått ut.");

            // Kolla igen att exemplaret inte är uthyrt i detta ögonblick
            if (await _rentals.InventoryHasActiveRentalAsync(res.InventoryId, ct))
                throw new ValidationException("Exemplaret är redan uthyrt.");

            // Skapa en Rental
            var rental = new Rental
            {
                CustomerId = res.CustomerId,
                InventoryId = res.InventoryId,
                StaffId = staffId,
                RentalDate = DateTime.UtcNow,
                ReturnDate = null // inte återlämnad ännu
            };

            await _rentals.AddAsync(rental, ct);

            // Markera bokningen som Fulfilled
            await _reservations.MarkFulfilledAsync(reservationId, ct);

            // Spara båda i samma transaktion
            await _db.SaveChangesAsync(ct);

            return rental.RentalId;
        }

        // Avboka
        public async Task CancelAsync(int reservationId, CancellationToken ct = default)
        {
            await _reservations.MarkCancelledAsync(reservationId, ct);
            await _db.SaveChangesAsync(ct);
        }
    }
}
