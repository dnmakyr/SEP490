using System;
using System.Collections.Generic;

namespace verbum_service_domain.Models;

/// <summary>
/// confirm mail: ACTIVE
/// tạo tài khoản: INACTIVE
/// </summary>
public partial class User
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime? EmailVerified { get; set; }

    public int? ImageId { get; set; }

    public string? Password { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string Status { get; set; } = null!;

    public int? TokenId { get; set; }

    public string RoleCode { get; set; } = null!;

    public virtual ICollection<Issue> IssueAssignees { get; set; } = new List<Issue>();

    public virtual ICollection<Issue> IssueClients { get; set; } = new List<Issue>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Revelancy> Revelancies { get; set; } = new List<Revelancy>();

    public virtual Role RoleCodeNavigation { get; set; } = null!;

    public virtual RefreshToken? Token { get; set; }

    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
}
