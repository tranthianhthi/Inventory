using OnlineInventoryLib.Prism.Models;
using OnlineInventoryLib.Shopee.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineInventoryLib.Interfaces
{
    public interface IShopeeService
    {
       Task<ShopeeUpdateVariationsLog> UpdateShopeeVariationStock(List<PrismQtyOnHand> prismQtyOnHands, int pageSize = 50, List<OnlineUPC> onlineUPCs = null);

        Task<ShopeeUpdateVariationsLog> RemoveVariations(List<OnlineUPC> onlineUPCs, int pageSize = 50);
    }
}
