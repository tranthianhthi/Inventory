using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COGInterfaceCommand.Common.COG
{
    public class TransferDetails
    {
        public string sku { get; set; }
        public int quantity { get; set; }

        public TransferDetails(string sku, int qty)
        {
            this.sku = sku;
            this.quantity = qty;
        }
    }
}
