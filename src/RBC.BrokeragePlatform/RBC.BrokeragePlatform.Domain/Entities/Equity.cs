namespace RBC.BrokeragePlatform.Domain.Entities
{
    /// <summary>
    /// Represents an equity (stock) in the brokerage platform, including its symbol and current market price.
    /// Remarks: The Equity class models the basic information about a stock, such as its ticker symbol and current price. 
    /// This information is essential for trading operations, portfolio management, and displaying market data to users. 
    /// Each equity is uniquely identified by an EquityId, which serves as the primary key in the database.
    /// </summary>
    internal class Equity
    {
        public int EquityId { get; set; }   // primary key for the Equity entity, uniquely identifies each equity in the database
        public string Symbol { get; set; } = string.Empty;  // stock symbol for the equity, such as "AAPL" for Apple Inc., used for trading and display purposes
        public decimal CurrentPrice { get; set; }   // current market price of the equity, used for calculating the value of positions and for trading decisions
    }
}
