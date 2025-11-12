using System;
using System.Collections.Generic;

namespace Web.TempModels;

public partial class Module
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool Active { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public virtual ICollection<FormModule> FormModules { get; set; } = new List<FormModule>();
}
