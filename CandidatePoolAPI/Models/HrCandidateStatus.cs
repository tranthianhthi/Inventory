using System;
using System.Collections.Generic;

namespace CandidatePoolAPI.Models
{
    public partial class HrCandidateStatus
    {
        public HrCandidateStatus()
        {
            HrCandidate = new HashSet<HrCandidate>();
        }

        public int Id { get; set; }
        public string StatusTxt { get; set; }
        public int? PrvStatusId { get; set; }

        public virtual ICollection<HrCandidate> HrCandidate { get; set; }
    }
}
