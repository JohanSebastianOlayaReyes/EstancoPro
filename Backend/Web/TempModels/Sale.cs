using System;
using System.Collections.Generic;

namespace Web.TempModels;

public partial class Sale
{
    public int Id { get; set; }

    public DateTime SoldAt { get; set; }

    public string Status { get; set; } = null!;

    public decimal Subtotal { get; set; }

    public decimal TaxTotal { get; set; }

    public decimal GrandTotal { get; set; }

    public int CashSessionId { get; set; }

    public bool Active { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public virtual CashSession CashSession { get; set; } = null!;

    public virtual ICollection<SaleProductDetail> SaleProductDetails { get; set; } = new List<SaleProductDetail>();
}
