using System;
using System.Collections.Generic;

namespace Web.TempModels;

public partial class Userrol
{
    public int UserId { get; set; }

    public int RolId { get; set; }

    public int Id { get; set; }

    public bool Active { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public virtual Rol Rol { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
