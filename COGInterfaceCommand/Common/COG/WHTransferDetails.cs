using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COGInterfaceCommand.Common.COG
{
   public class WHTransferDetails
    {
        public string sku { get; set; }
        public int quantityReceived { get; set; }

        public WHTransferDetails(string sku, int qty)
        {
            this.sku = sku;
            this.quantityReceived = qty;
        }
    }
}
