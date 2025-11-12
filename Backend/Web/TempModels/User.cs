using System;
using System.Collections.Generic;

namespace Web.TempModels;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int RolId { get; set; }

    public int PersonId { get; set; }

    public bool Active { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public virtual Person Person { get; set; } = null!;

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual Rol Rol { get; set; } = null!;

    public virtual ICollection<Userrol> Userrols { get; set; } = new List<Userrol>();
}
