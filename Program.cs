using Microsoft.EntityFrameworkCore;
using RetroTapes.Data;
using RetroTapes.Models;

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
            builder.Services.AddScoped<RetroTapes.Services.FilmService>();

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
                if(!db.Actors.Any())
                {
                    db.Actors.AddRange(
                        new Actor { FirstName = "Tom", LastName = "Hanks", LastUpdate = DateTime.UtcNow },
                        new Actor { FirstName = "Meryl", LastName = "Streep", LastUpdate = DateTime.UtcNow });

                    db.SaveChanges();

                }

                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }
    }
}
