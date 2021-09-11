using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COGInterfaceCommand.Common.COG
{
   public class BackTransferDetails
    {
        public string sku { get; set; }
        public int quantityReceived { get; set; }

        public BackTransferDetails(string sku, int qty)
        {
            this.sku = sku;
            this.quantityReceived = qty;
        }

        public BackTransferDetails(TransferDetails detail)
        {
            this.sku = detail.sku;
            this.quantityReceived = detail.quantity;
        }
    }
}
