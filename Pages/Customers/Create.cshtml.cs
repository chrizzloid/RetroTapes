using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RetroTapes.Data;
using RetroTapes.Models;
using RetroTapes.Services;
using RetroTapes.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetroTapes.Pages.Customers
{
    public class CreateModel : PageModel
    {
        private readonly CustomerService _service;
        private readonly SakilaContext _db;

        public CreateModel(CustomerService service, SakilaContext db)
        {        
            _service = service;
            _db = db;
        } 
        
        [BindProperty] public CustomerEditVm Vm { get; set; } = new();
        public IEnumerable<SelectListItem> StoreOptions { get; set; } = [];
        public IEnumerable<SelectListItem> AddressOptions { get; set; } = [];
                            
        public async Task OnGet()
        {
            await PopulateDropDownAsync();
           
        }

       

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDownAsync();
                return Page();
            }

            await _service.UpsertAsync(Vm);
            TempData["Flash"] = "Kund skapades.";
            return RedirectToPage("Index");          
        }

        private async Task PopulateDropDownAsync()
        {
            var stores = await _db.Stores
                .AsNoTracking()
                .OrderBy(s => s.StoreId)
                .Select(s => new SelectListItem
                {
                    Value = s.StoreId.ToString(),
                    Text = "Butik" + s.StoreId
                })
                .ToListAsync();

            var addresses = await _db.Addresses
                .AsNoTracking()
                .OrderBy(a => a.AddressId)
                .Select(a => new SelectListItem
                {
                    Value = a.AddressId.ToString(),
                    Text = a.Address1

                })
                .ToListAsync();

            StoreOptions = stores;
            AddressOptions = addresses;
            ViewData["StoreOptions"] = StoreOptions;
            ViewData["AddressOptions"] = AddressOptions;
        }




    }
}
