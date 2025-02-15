using System;
using System.Collections.Generic;

namespace verbum_service_domain.Models;

public partial class Category
{
    public string? CategoryName { get; set; }

    public int CategoryId { get; set; }

    public virtual ICollection<Revelancy> Revelancies { get; set; } = new List<Revelancy>();

    public virtual ICollection<Work> Works { get; set; } = new List<Work>();
}
