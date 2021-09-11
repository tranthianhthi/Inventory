using System;
using System.Collections.Generic;

namespace CandidatePoolAPI.Models
{
    public partial class HrResourceDataConfiguration
    {
        public int Id { get; set; }
        public int ResourceId { get; set; }
        public int FieldId { get; set; }
        public string ColumnName { get; set; }
        public int? RowNum { get; set; }
        public string FieldFormat { get; set; }
        public bool? ActiveFlag { get; set; }

        public virtual HrDataField Field { get; set; }
        public virtual HrRecruitmentResource Resource { get; set; }
    }
}
