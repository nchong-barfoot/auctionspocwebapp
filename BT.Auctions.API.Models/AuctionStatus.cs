namespace BT.Auctions.API.Models
{
    public enum AuctionStatus
    {
        None = 0,
        InProgress = 1,
        NoBids = 2,
        PassedIn = 3,
        Sold = 4,
        Selling = 5,
        Withdrawn = 6,
        SoldPrior = 7,
        Postponed = 8
    }
}
