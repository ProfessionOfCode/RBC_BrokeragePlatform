namespace RBC.BrokeragePlatform.SharedCore.DTOs
{
    public class AccountDto
    {
        public int AccountId { get; set; }
        public string ClientName { get; set; }
        public string AccountNumber { get; set; }
        public decimal CashBalance { get; set; }

    }
}
