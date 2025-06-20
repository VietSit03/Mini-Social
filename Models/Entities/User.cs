using System;
using System.Collections.Generic;

namespace MiniSocialAPI.Models.Entities;

public partial class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PasswordHash { get; set; }

    public string? Status { get; set; }

    public string? FullName { get; set; }

    public string? AvatarUrl { get; set; }

    public string? Bio { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<UserOauth> UserOauths { get; set; } = new List<UserOauth>();
}
