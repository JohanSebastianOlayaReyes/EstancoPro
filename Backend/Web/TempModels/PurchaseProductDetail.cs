using System;
using System.Collections.Generic;

namespace Web.TempModels;

public partial class PurchaseProductDetail
{
    public int Id { get; set; }

    public int Quantity { get; set; }

    public decimal UnitCost { get; set; }

    public decimal LineTotal { get; set; }

    public int PurchaseId { get; set; }

    public int ProductId { get; set; }

    public int UnitMeasureId { get; set; }

    public bool Active { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Purchase Purchase { get; set; } = null!;

    public virtual UnitMeasure UnitMeasure { get; set; } = null!;
}
