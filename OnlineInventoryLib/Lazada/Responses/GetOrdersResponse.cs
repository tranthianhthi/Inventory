using OnlineInventoryLib.Lazada.Models;

namespace OnlineInventoryLib.Lazada.Responses
{
    public class GetOrdersResponse : BaseResponse
    {
        public LazadaGetOrdersData data { get; set; }
    }
}
