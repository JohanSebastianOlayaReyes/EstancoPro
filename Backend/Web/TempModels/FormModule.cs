using System;
using System.Collections.Generic;

namespace Web.TempModels;

public partial class FormModule
{
    public int FormId { get; set; }

    public int ModuleId { get; set; }

    public int Id { get; set; }

    public bool Active { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public virtual Form Form { get; set; } = null!;

    public virtual Module Module { get; set; } = null!;
}
