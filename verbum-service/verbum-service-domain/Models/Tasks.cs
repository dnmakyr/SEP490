using System;
using System.Collections.Generic;

namespace verbum_service_domain.Models;

public partial class Tasks
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public long Status { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int WordCount { get; set; }

    public string DocumentUrl { get; set; } = null!;

    public string TargetLanguageId { get; set; } = null!;

    public Guid? WorkId { get; set; }

    public virtual Work? Work { get; set; }
}
