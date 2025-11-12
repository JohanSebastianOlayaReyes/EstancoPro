using System;
using System.Collections.Generic;

namespace Web.TempModels;

public partial class CashSession
{
    public int Id { get; set; }

    public DateTime OpenedAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    public decimal OpeningAmount { get; set; }

    public decimal ClosingAmount { get; set; }

    public bool Active { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public virtual ICollection<CashMovement> CashMovements { get; set; } = new List<CashMovement>();

    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
