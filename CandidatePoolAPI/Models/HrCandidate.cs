using System;
using System.Collections.Generic;

namespace CandidatePoolAPI.Models
{
    public partial class HrCandidate
    {
        public int Id { get; set; }
        public int? ResourceId { get; set; }
        public string CandidateName { get; set; }
        public string Gender { get; set; }
        public DateTime? Dob { get; set; }
        public string Address { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string Position { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public int? ResumeId { get; set; }
        public int? StatusId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string Note { get; set; }

        public virtual HrRecruitmentResource Resource { get; set; }
        public virtual HrCandidateStatus Status { get; set; }
    }
}
