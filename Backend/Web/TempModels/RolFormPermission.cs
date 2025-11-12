using System;
using System.Collections.Generic;

namespace Web.TempModels;

public partial class RolFormPermission
{
    public int RolId { get; set; }

    public int FormId { get; set; }

    public int PermissionId { get; set; }

    public int Id { get; set; }

    public bool Active { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public virtual Form Form { get; set; } = null!;

    public virtual Permission Permission { get; set; } = null!;

    public virtual Rol Rol { get; set; } = null!;
}
