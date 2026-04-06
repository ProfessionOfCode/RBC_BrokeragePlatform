namespace RBC.BrokeragePlatform.SharedCore.DTOs
{
    public class PositionDto
    {
        public int PositionId { get; set; }
        public int AccountId { get; set; }
        public int EquityId { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal AverageCostPerShare { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal CurrentValue { get; set; }

    }   
}
