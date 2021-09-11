using System;
using System.Collections.Generic;

namespace CandidatePoolAPI.Models
{
    public partial class HrDocument
    {
        public HrDocument()
        {
            HrEmployeeDocuments = new HashSet<HrEmployeeDocuments>();
        }

        public int Id { get; set; }
        public string DocumentName { get; set; }
        public bool? ActiveFlag { get; set; }

        public virtual ICollection<HrEmployeeDocuments> HrEmployeeDocuments { get; set; }
    }
}
