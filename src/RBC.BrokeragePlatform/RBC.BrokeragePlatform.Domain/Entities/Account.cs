namespace RBC.BrokeragePlatform.Domain.Entities
{
    /// <summary>
    /// Represents the user account in the brokerage platform. 
    /// This class will contain information such as account balance, account number, and other relevant details pertaining to the user's brokerage account.
    /// Remarks: The Account class models the essential information about a user's brokerage account, including the account holder's name, unique account number, and current cash balance. 
    /// This information is crucial for managing user accounts, processing trades, and calculating the total value of the account. 
    /// Each account is uniquely identified by an AccountId, which serves as the primary key in the database.
    /// </summary>
    public class Account
    {
        public int AccountId { get; set; }  // primary key for the Account entity, uniquely identifies each account in the database
        public string ClientName { get; set; }  = string.Empty; // name of the account holder, used for display and identification purposes
        public string AccountNumber { get; set; }  // unique account number assigned to the user's brokerage account, used for identification and transactions
        public decimal CashBalance { get; set; }    // current cash balance in the account, used for trading and calculating total account value
    }
}
