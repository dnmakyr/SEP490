using System;
using System.Collections.Generic;

namespace verbum_service_domain.Models;

/// <summary>
/// chưa assign:NEW
/// 
/// asign linguists: IN_PROGRESS
///  
/// linguist làm xong: SUBMITTED
/// 
/// SM review ok: APPROVED
/// SM reject: IN_PROGRESS
/// 
/// client tạo issue thì tạo thêm 1 job mới ở sevice cuối cùng(TL-&gt; ED -&gt; EV)
/// </summary>
public partial class Job
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    /// <summary>
    /// NEW, IN_PROGRESS, SUBMITTED, ACCEPTED
    /// </summary>
    public string Status { get; set; } = null!;

    public DateTime? DueDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int WordCount { get; set; }

    public string DocumentUrl { get; set; } = null!;

    public string TargetLanguageId { get; set; } = null!;

    public Guid? WorkId { get; set; }

    public string? DeliverableUrl { get; set; }

    public string? RejectReason { get; set; }

    public virtual Issue? Issue { get; set; }

    public virtual Work? Work { get; set; }

    public virtual ICollection<User> Assignees { get; set; } = new List<User>();
}
