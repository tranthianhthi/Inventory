using OnlineInventoryLib.Lazada.Models;

namespace OnlineInventoryLib.Lazada.Responses
{
    public class GetOrderItemsResponse : BaseResponse 
    {
        public LazadaGetOrderItemsData data { get; set; }
    }
}
