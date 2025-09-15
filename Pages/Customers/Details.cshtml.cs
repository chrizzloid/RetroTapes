using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
    public class DetailsModel : PageModel
    {
        private CustomerService _service;
        public DetailsModel(CustomerService service) => _service = service;


        public CustomerDetailVm Vm { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var vm = await _service.GetDetailAsync(id);
            if (vm == null) { return NotFound(); }

            Vm = vm;
            return Page();
                    
        }
    }
}
