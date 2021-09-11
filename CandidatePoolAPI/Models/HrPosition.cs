using System;
using System.Collections.Generic;

namespace CandidatePoolAPI.Models
{
    public partial class HrPosition
    {
        public int Id { get; set; }
        public string PositionName { get; set; }
        public bool? ActiveFlag { get; set; }
        public bool? IsStorePosition { get; set; }
    }
}
