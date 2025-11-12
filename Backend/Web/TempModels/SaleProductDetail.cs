using System;
using System.Collections.Generic;

namespace Web.TempModels;

public partial class SaleProductDetail
{
    public int SaleId { get; set; }

    public int ProductId { get; set; }

    public int UnitMeasureId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TaxRate { get; set; }

    public decimal LineSubtotal { get; set; }

    public decimal LineTax { get; set; }

    public decimal LineTotal { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Sale Sale { get; set; } = null!;

    public virtual UnitMeasure UnitMeasure { get; set; } = null!;
}
