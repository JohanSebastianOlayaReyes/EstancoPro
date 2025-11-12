namespace Entity.Dto
{
    /// <summary>
    /// DTO para reporte de ventas con totales agregados
    /// </summary>
    public class SalesReportDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalSales { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalTax { get; set; }
        public decimal GrandTotal { get; set; }
        public List<SalesByCategoryDto> SalesByCategory { get; set; } = new();
    }

    public class SalesByCategoryDto
    {
        public string CategoryName { get; set; }
        public int Count { get; set; }
        public decimal Total { get; set; }
    }
}
