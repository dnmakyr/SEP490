using System;
using System.Collections.Generic;

namespace verbum_service_domain.Models;

public partial class Rating
{
    public Guid RatingId { get; set; }

    public Guid? OrderId { get; set; }

    public decimal? InTime { get; set; }

    public decimal? Expectation { get; set; }

    public decimal? IssueResolved { get; set; }

    public string? MoreThought { get; set; }

    public virtual Order? Order { get; set; }
}
