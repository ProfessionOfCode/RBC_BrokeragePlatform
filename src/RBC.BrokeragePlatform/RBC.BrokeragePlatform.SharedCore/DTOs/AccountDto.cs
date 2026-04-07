namespace RBC.BrokeragePlatform.SharedCore.DTOs
{
    public class AccountDto
    {
        public int AccountId { get; set; }
        public string ClientName { get; set; }  = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public decimal CashBalance { get; set; }

    }
}
