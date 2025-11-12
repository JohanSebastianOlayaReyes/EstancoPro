using System;
using System.Collections.Generic;

namespace Web.TempModels;

public partial class UnitMeasure
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool Active { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public virtual ICollection<ProductUnitPrice> ProductUnitPrices { get; set; } = new List<ProductUnitPrice>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<PurchaseProductDetail> PurchaseProductDetails { get; set; } = new List<PurchaseProductDetail>();

    public virtual ICollection<SaleProductDetail> SaleProductDetails { get; set; } = new List<SaleProductDetail>();
}
