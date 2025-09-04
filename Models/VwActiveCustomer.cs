using System;
using System.Collections.Generic;

namespace RetroTapes.Models;

public partial class VwActiveCustomer
{
    public int CustomerId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Active { get; set; } = null!;
}
