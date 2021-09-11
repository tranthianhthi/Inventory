namespace OnlineInventoryLib.Shopee.Requests
{
    public interface IRequestData
    {
        int partner_id { get; }
        int shopid { get; }
        long timestamp { get; }
    }
}
