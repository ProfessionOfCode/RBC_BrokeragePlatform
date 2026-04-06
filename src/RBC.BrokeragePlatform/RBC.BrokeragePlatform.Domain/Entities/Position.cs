namespace RBC.BrokeragePlatform.Domain.Entities
{
    /// <summary>
    /// Represents a position in a user account for a specific equity, including share quantity and cost basis
    /// information.
    /// </summary>
    /// <remarks>A position associates an account with a particular equity (such as a stock), tracking the
    /// number of shares held and the average cost per share. This class is typically used in portfolio management or
    /// trading applications to model holdings at the account level.</remarks>
    public class Position
    {
        public int PositionId { get; set; } // primary key for the Position entity, uniquely identifies each position in the database
        public int AccountId { get; set; }  // foreign key to the Account entity, used to link this position to the specific user account it belongs to
        public int EquityId { get; set; }   // foreign key to the Equity entity, used to link this position to the specific stock it represents
        public string Symbol { get; set; } = string.Empty;  // stock symbol for the equity in this position, included for easy reference without needing to join with the Equity entity
        public int Quantity { get; set; }   // number of shares held in this position
        public decimal AverageCostPerShare { get; set; }    // cost basis for the shares in this position, used to calculate unrealized gains/losses
    }
}
