using System;
using System.Collections.Generic;

namespace CandidatePoolAPI.Models
{
    public partial class HrDataField
    {
        public HrDataField()
        {
            HrResourceDataConfiguration = new HashSet<HrResourceDataConfiguration>();
        }

        public int Id { get; set; }
        public string FieldName { get; set; }
        public bool ActiveFlag { get; set; }
        public bool? CheckExisted { get; set; }

        public virtual ICollection<HrResourceDataConfiguration> HrResourceDataConfiguration { get; set; }
    }
}
