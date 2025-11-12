using System;
using System.Collections.Generic;

namespace Web.TempModels;

public partial class ProductUnitPrice
{
    public int ProductId { get; set; }

    public int UnitMeasureId { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? UnitCost { get; set; }

    public decimal ConversionFactor { get; set; }

    public string? Barcode { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual UnitMeasure UnitMeasure { get; set; } = null!;
}
