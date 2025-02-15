using System;
using System.Collections.Generic;

namespace verbum_service_domain.Models;

/// <summary>
/// tạo issue, job tạo ra bởi issue : OPEN
/// 
/// SM accept issue from client: IN_PROGRESS
/// SM reject issue from client: CANCEL
/// 
/// Linguist đánh: SUBMITTED
/// 
/// SM review linguist ok: RESOLVED 
/// SM reject resolve from linguist: IN_PROGRESS
/// 
/// client cancel lúc nào cx đc: CANCEL
/// </summary>
public partial class Issue
{
    public Guid IssueId { get; set; }

    public string? IssueName { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// CANCEL, OPEN, RESOLVED, ACCEPTED, IN-PROGRESS
    /// </summary>
    public string? Status { get; set; }

    public Guid? ClientId { get; set; }

    public string? IssueDescription { get; set; }

    public Guid? AssigneeId { get; set; }

    public string? CancelResponse { get; set; }

    public string? RejectResponse { get; set; }

    public Guid JobId { get; set; }

    public string SrcDocumentUrl { get; set; } = null!;

    public virtual User? Assignee { get; set; }

    public virtual User? Client { get; set; }

    public virtual ICollection<IssueAttachment> IssueAttachments { get; set; } = new List<IssueAttachment>();

    public virtual Job Job { get; set; } = null!;
}
