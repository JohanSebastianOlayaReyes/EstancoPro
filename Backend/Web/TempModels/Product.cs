using System;
using System.Collections.Generic;

namespace Web.TempModels;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Photo { get; set; }

    public decimal UnitCost { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? TaxRate { get; set; }

    public int StockOnHand { get; set; }

    public int ReorderPoint { get; set; }

    public int CategoryId { get; set; }

    public int UnitMeasureId { get; set; }

    public bool Active { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<ProductUnitPrice> ProductUnitPrices { get; set; } = new List<ProductUnitPrice>();

    public virtual ICollection<PurchaseProductDetail> PurchaseProductDetails { get; set; } = new List<PurchaseProductDetail>();

    public virtual ICollection<SaleProductDetail> SaleProductDetails { get; set; } = new List<SaleProductDetail>();

    public virtual UnitMeasure UnitMeasure { get; set; } = null!;
}
