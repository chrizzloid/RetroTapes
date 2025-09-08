using Microsoft.EntityFrameworkCore;
using RetroTapes.Models;

namespace RetroTapes.Data
{
    public partial class SakilaContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Film>()
                .Property(f => f.LastUpdate)
                .IsConcurrencyToken();
        }
    }
}
