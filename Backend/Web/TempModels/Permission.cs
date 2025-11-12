using System;
using System.Collections.Generic;

namespace Web.TempModels;

public partial class Permission
{
    public int Id { get; set; }

    public string TypePermission { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool Active { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public virtual ICollection<RolFormPermission> RolFormPermissions { get; set; } = new List<RolFormPermission>();
}
