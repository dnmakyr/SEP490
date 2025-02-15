using System;
using System.Collections.Generic;

namespace verbum_service_domain.Models;

public partial class Service
{
    public string? ServiceName { get; set; }

    public string ServiceCode { get; set; } = null!;

    public decimal? ServicePrice { get; set; }

    public int ServiceOrder { get; set; }

    public virtual ICollection<Work> Works { get; set; } = new List<Work>();
}
