using System;
using System.Collections.Generic;

namespace Web.TempModels;

public partial class Purchase
{
    public int Id { get; set; }

    public DateTime? OrderedAt { get; set; }

    public DateTime? ReceivedAt { get; set; }

    public bool Status { get; set; }

    public decimal TotalCost { get; set; }

    public int SupplierId { get; set; }

    public bool Active { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public virtual ICollection<PurchaseProductDetail> PurchaseProductDetails { get; set; } = new List<PurchaseProductDetail>();

    public virtual Supplier Supplier { get; set; } = null!;
}
