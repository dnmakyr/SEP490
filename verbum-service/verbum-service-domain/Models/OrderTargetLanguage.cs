using System;
using System.Collections.Generic;

namespace verbum_service_domain.Models;

public partial class OrderTargetLanguage
{
    public Guid OrderId { get; set; }

    public string TargetLanguageId { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;

    public virtual Language TargetLanguage { get; set; } = null!;
}
