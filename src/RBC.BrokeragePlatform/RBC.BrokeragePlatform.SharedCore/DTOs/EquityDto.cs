namespace RBC.BrokeragePlatform.SharedCore.DTOs
{
    public class EquityDto 
    {
        public int EquityId { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public decimal CurrentPrice { get; set; }
    }
}
