using OnlineInventoryLib.Lazada.Models;
using OnlineInventoryLib.Prism.Models;
using OnlineInventoryLib.Tiki.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OnlineInventoryLib.Interfaces
{

    public interface ITikiService
    {
        bool RefreshToken(Online_store_Tiki tikiAuth);
        Root GetProducts(string lazadaAccessToken, int v1, string v2, string lazFilter = "all", DateTime? create_after = null);
        void UpdateStock(TikiProduct prod);

        //Task<T> GetItemList<T, R>(R request);
    }
}
