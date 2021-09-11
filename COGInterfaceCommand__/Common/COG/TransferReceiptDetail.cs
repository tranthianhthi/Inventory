using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;



namespace COGInterfaceCommand.Common.COG
{
   public class TransferReceiptDetail
    {
        public TransferReceiptHeader hearder { get; set; }
        public TransferReceiptSku[] Skus { get; set; }
        public string storeCode { get; set; }

    }
    public class TransferReceiptSku
    {        
        public int QuantityReceived { get; set; }
        public string sku { get; set; }
    }
   
}
