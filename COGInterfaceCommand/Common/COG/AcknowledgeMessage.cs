using System;
using System.Collections.Generic;
using System.Text;

namespace COGInterfaceCommand.Common.COG
{
    public class AcknowledgeMessage
    {
        /// <summary>
        /// GUID produced from calling the ASN API
        /// </summary>
        public Guid AcknowledgementGuid { get; set; }

        /// <summary>
        /// Y = Means a successful response was received and the receiver is able to process the result. N = There was an issue processing the result from the ASN API. 
        /// </summary>
        public char success_ind { get; set; }

        /// <summary>
        /// Optional for success_ind = Y. Mandatory for success_ind = N The error_msg accepts a string of up to 255 chars. This provides information of any error details that can help troubleshooting the issue with the result.
        /// </summary>
        public string error_msg { get; set; } 
    }
}
