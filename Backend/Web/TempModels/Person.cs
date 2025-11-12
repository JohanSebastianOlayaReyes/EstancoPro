using System;
using System.Collections.Generic;

namespace Web.TempModels;

public partial class Person
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public int PhoneNumber { get; set; }

    public int NumberIdentification { get; set; }

    public bool Active { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public virtual User? User { get; set; }
}
