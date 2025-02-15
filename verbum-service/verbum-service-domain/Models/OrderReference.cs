using System;
using System.Collections.Generic;

namespace verbum_service_domain.Models;

/// <summary>
/// TRANSLATION, REFERENCES
/// </summary>
public partial class OrderReference
{
    public Guid OrderId { get; set; }

    public string ReferenceFileUrl { get; set; } = null!;

    public string? Tag { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Order Order { get; set; } = null!;
}
