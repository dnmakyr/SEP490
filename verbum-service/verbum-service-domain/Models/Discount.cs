using System;
using System.Collections.Generic;

namespace verbum_service_domain.Models;

public partial class Discount
{
    public Guid DiscountId { get; set; }

    public decimal? DiscountPercent { get; set; }

    public string? DiscountName { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
