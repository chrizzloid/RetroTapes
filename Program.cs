using Microsoft.EntityFrameworkCore;
using RetroTapes.Data;
using RetroTapes.Models;
using RetroTapes.Services;

namespace RetroTapes
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("SakilaDb")
      ?? throw new InvalidOperationException("Connection string 'SakilaDb' not found.");



            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddDbContext<SakilaContext>(options => options.UseSqlServer(connectionString));
            builder.Services.AddScoped<RetroTapes.Services.LookupService>();
            builder.Services.AddScoped<RetroTapes.Services.FilmService>();
            builder.Services.AddScoped<IStaffService, StaffService>();
            builder.Services.AddScoped<RetroTapes.Services.CustomerService>();

            builder.Services.AddScoped<RetroTapes.Services.AddressService>();
            builder.Services.AddScoped<RetroTapes.Repositories.IRentalRepository, RetroTapes.Repositories.EfRentalRepository>();
            builder.Services.AddScoped<RetroTapes.Repositories.IPaymentRepository, RetroTapes.Repositories.EfPaymentRepository>();

            builder.Services.AddScoped<IInventoryService, InventoryService>();

            builder.Services.AddSession(o =>
            {
                o.IdleTimeout = TimeSpan.FromHours(8);
                o.Cookie.HttpOnly = true;
                o.Cookie.IsEssential = true;
            });
            builder.Services.AddHttpContextAccessor();


            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<SakilaContext>();

                if (!db.Languages.Any())
                {
                    db.Languages.AddRange(
                        new Language { Name = "English" },
                        new Language { Name = "Italian" },
                        new Language { Name = "Spanish" },
                        new Language { Name = "Swedish" }
                        );
                }
                if (!db.Categories.Any())
                {

                    db.Categories.AddRange(
                        new Category { Name = "Action", LastUpdate = DateTime.UtcNow },
                        new Category { Name = "Comedy", LastUpdate = DateTime.UtcNow },
                        new Category { Name = "Drama", LastUpdate = DateTime.UtcNow },
                        new Category { Name = "Romance", LastUpdate = DateTime.UtcNow },
                        new Category { Name = "Horror", LastUpdate = DateTime.UtcNow }
                        );

                }
                if (!db.Actors.Any())
                {
                    db.Actors.AddRange(
                        new Actor { FirstName = "Tom", LastName = "Hanks", LastUpdate = DateTime.UtcNow },
                        new Actor { FirstName = "Meryl", LastName = "Streep", LastUpdate = DateTime.UtcNow });

                    db.SaveChanges();

                }
                // --- extra seed för paginerings-test ---
                if (!db.Films.Any())
                {
                    var langId = db.Languages.Select(l => l.LanguageId).First(); // t.ex. English
                    var catId = db.Categories.Select(c => c.CategoryId).First(); // t.ex. Action

                    for (int i = 1; i <= 55; i++)
                    {
                        var film = new Film
                        {
                            Title = $"Testfilm {i:D2}",
                            Description = "Demo för pagination",
                            ReleaseYear = "2001",
                            LanguageId = langId,
                            LastUpdate = DateTime.UtcNow
                        };
                        db.Films.Add(film);
                        db.SaveChanges(); // behövs för att få FilmId innan vi kan skapa M:N (enkelt i dev)

                        // Koppla varannan film till en kategori för att testa kategori-filter senare
                        if (i % 2 == 1)
                        {
                            db.FilmCategories.Add(new FilmCategory { FilmId = film.FilmId, CategoryId = catId });
                            db.SaveChanges();
                        }
                    }
                }


                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();


            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }
    }
}
