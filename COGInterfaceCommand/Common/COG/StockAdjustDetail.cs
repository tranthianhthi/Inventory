using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COGInterfaceCommand.Common.COG
{
   public class StockAdjustDetail
    {
        public string sku { get; set; }
        public int quantity { get; set; }

        public StockAdjustDetail(string sku, int qty)
        {
            this.sku = sku;
            this.quantity = qty;
        }
    }
}
