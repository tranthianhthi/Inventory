using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COGInterfaceCommand.Common.COG
{
   public class TransferReceiptHeader
    {
        public string Authorization { get; set; }
        public string OcpApimSubscriptionKey { get; set; }
        public string DC_Code { get; set; }
        public string ContentType { get; set; }
    }
}
