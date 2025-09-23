using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RetroTapes.Data;
using RetroTapes.Models;

namespace RetroTapes.Pages.Customers
{
    public class DeleteModel : PageModel
    {
        private readonly SakilaContext _db;

        public DeleteModel(SakilaContext db)
        {
            _db = db;
        }


        [BindProperty] public int Id { get; set; }
        [BindProperty] public DateTime LastUpdate { get; set; }
        public string DisplayName { get; set; } = "";

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var customer = await _db.Customers
                .AsNoTracking()
                .Where(c => c.CustomerId == id)
                .Select(c => new { c.CustomerId, c.LastUpdate, Name = c.FirstName + " " + c.LastName })
                .FirstOrDefaultAsync();

            if (customer == null) return NotFound();


            Id = customer.CustomerId;
            LastUpdate = customer.LastUpdate;
            DisplayName = customer.Name;
            return Page();

        }

        public async Task<IActionResult> OnPostAsync()
        {
            var entity = await _db.Customers.FirstOrDefaultAsync(c => c.CustomerId == Id);
            if (entity == null) return NotFound();

            _db.Entry(entity).Property(e => e.LastUpdate).OriginalValue = LastUpdate;

            _db.Customers.Remove(entity);

            try
            {
                await _db.SaveChangesAsync();
                TempData["Flash"] = "Kunden togs bort.";
                return RedirectToPage("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError(string.Empty, "Kunden ändrades/togs bort av någon annan Försök igen.");
                return Page();
            }
           
        }
    }
}
