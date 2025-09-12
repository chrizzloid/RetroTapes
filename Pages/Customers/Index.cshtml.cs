using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RetroTapes.Data;
using RetroTapes.Infrastructure;
using RetroTapes.Models;
using RetroTapes.Services;
using RetroTapes.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace RetroTapes.Pages.Customers
{
    public class IndexModel : PageModel
    {
        
        private CustomerService _service;
        public IndexModel(CustomerService service) => _service = service;

        [BindProperty(SupportsGet = true)] public string? q { get; set; }
        [BindProperty(SupportsGet = true)] public bool? active { get; set; }
        [BindProperty(SupportsGet = true)] public string? sort { get; set; }
        [BindProperty(SupportsGet = true)] public int pageIndex { get; set; } = 1;
        [BindProperty(SupportsGet = true)] public int pageSize { get; set; } = 20;

        public PagedResult<CustomerListItemVm> Result { get; set; } = default!;
        

       


        public async Task OnGetAsync()
        {                

            Result = await _service.SearchAsync(q, active, sort, pageIndex, pageSize);

        }
    }
}
