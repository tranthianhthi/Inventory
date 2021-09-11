using OnlineInventoryLib.Lazada.Models;

namespace OnlineInventoryLib.Lazada.Responses
{
    public class GetProductResponse : BaseResponse
    {
        public LazadaGetProductData data { get; set; }
    }
}
