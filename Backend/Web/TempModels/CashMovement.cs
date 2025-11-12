using System;
using System.Collections.Generic;

namespace Web.TempModels;

public partial class CashMovement
{
    public int CashSessionId { get; set; }

    public DateTime At { get; set; }

    public string Type { get; set; } = null!;

    public decimal Amount { get; set; }

    public string? Reason { get; set; }

    public int? RelatedId { get; set; }

    public string? RelatedEntity { get; set; }

    public virtual CashSession CashSession { get; set; } = null!;
}
