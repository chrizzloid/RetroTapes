using System;
using System.ComponentModel.DataAnnotations;

namespace RetroTapes.Models
{
    // OBS! Den här modellen representerar en "bokning" av ett specifikt exemplar (Inventory).
    // Tanken: kunden reserverar en film tills ett visst klockslag (ExpiresAt).
    // Sen kan personalen "Fulfill" -> gör om bokningen till en Rental när kunden hämtar.
    public class Reservation
    {
        public int ReservationId { get; set; } // PK

        // Vem som bokar (måste finnas i Customer-tabellen)
        [Required]
        public int CustomerId { get; set; }

        // Vilket exemplar (Inventory) som bokas (inte bara film – det är viktigt pga flera exemplar kan finnas)
        [Required]
        public int InventoryId { get; set; }

        // När bokningen skapades (UTC så vi blir tidszons-säkra)
        public DateTime ReservedAt { get; set; } = DateTime.UtcNow;

        // När bokningen löper ut (därefter är den inte längre giltig)
        [Required]
        public DateTime ExpiresAt { get; set; }

        // Status: Active (giltig), Cancelled, Fulfilled (hämtad/utlämnad), Expired (gick ut)
        [Required]
        public ReservationStatus Status { get; set; } = ReservationStatus.Active;

        // Concurrency (så att två personer inte råkar skriva samtidigt utan att vi märker)
        public DateTime LastUpdate { get; set; } = DateTime.UtcNow;

        // Navigationer – valfria (vi använder dem i listvyerna)
        public Customer? Customer { get; set; }
        public Inventory? Inventory { get; set; }
    }

    // Liten enum för att göra statusvärdena lättlästa i kod och UI
    public enum ReservationStatus : byte
    {
        Active = 0,
        Cancelled = 1,
        Fulfilled = 2,
        Expired = 3
    }
}
