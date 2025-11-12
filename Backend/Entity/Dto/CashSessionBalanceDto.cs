namespace Entity.Dto
{
    /// <summary>
    /// DTO para el balance de una sesión de caja
    /// </summary>
    public class CashSessionBalanceDto
    {
        public int SessionId { get; set; }
        public decimal OpeningAmount { get; set; }
        public decimal ExpectedAmount { get; set; }
        public decimal ActualAmount { get; set; }
        public decimal Difference { get; set; }
        public List<CashMovementDto> Movements { get; set; } = new();
    }
}
