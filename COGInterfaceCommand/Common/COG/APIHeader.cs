using System;
using System.Collections.Generic;
using System.Text;

namespace COGInterfaceCommand.Common.COG
{
    public class APIHeader
    {
        public string APIMOperation { get; set; }
        public Guid AcknowledgementGuid { get; set; }
        public string DC_Code { get; set; }
        public string LoadType { get; set; }
    }
}
