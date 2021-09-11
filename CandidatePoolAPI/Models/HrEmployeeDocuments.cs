using System;
using System.Collections.Generic;

namespace CandidatePoolAPI.Models
{
    public partial class HrEmployeeDocuments
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int DocumentId { get; set; }
        public bool? Submitted { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual HrDocument Document { get; set; }
        public virtual HrEmployee Employee { get; set; }
    }
}
