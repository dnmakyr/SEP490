using System;
using System.Collections.Generic;

namespace verbum_service_domain.Models;

public partial class Work
{
    public Guid WorkId { get; set; }

    public Guid? OrderId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? DueDate { get; set; }

    public string? ServiceCode { get; set; }

    public string? WorkName { get; set; }

    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();

    public virtual Order? Order { get; set; }

    public virtual Service? ServiceCodeNavigation { get; set; }

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
}
