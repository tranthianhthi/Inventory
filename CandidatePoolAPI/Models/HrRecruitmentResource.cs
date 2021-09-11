using System;
using System.Collections.Generic;

namespace CandidatePoolAPI.Models
{
    public partial class HrRecruitmentResource
    {
        public HrRecruitmentResource()
        {
            HrCandidate = new HashSet<HrCandidate>();
            HrResourceDataConfiguration = new HashSet<HrResourceDataConfiguration>();
        }

        public int Id { get; set; }
        public string ResourceName { get; set; }
        public int? ReportStartRow { get; set; }
        public bool? ActiveFlag { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public bool? IsLocalResource { get; set; }
        public string ResourceUrl { get; set; }

        public virtual ICollection<HrCandidate> HrCandidate { get; set; }
        public virtual ICollection<HrResourceDataConfiguration> HrResourceDataConfiguration { get; set; }
    }
}
