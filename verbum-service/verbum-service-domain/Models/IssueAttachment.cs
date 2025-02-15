using System;
using System.Collections.Generic;

namespace verbum_service_domain.Models;

/// <summary>
/// tag lưu file solution và reference file: SOLUTION, ATTACHMENT
/// </summary>
public partial class IssueAttachment
{
    public Guid IssueId { get; set; }

    public string AttachmentUrl { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public string? Tag { get; set; }

    public virtual Issue Issue { get; set; } = null!;
}
