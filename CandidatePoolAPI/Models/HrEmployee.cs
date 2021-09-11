using System;
using System.Collections.Generic;

namespace CandidatePoolAPI.Models
{
    public partial class HrEmployee
    {
        public HrEmployee()
        {
            HrEmployeeDocuments = new HashSet<HrEmployeeDocuments>();
        }

        public int Id { get; set; }
        public int CandidateId { get; set; }
        public string EmployeeNo { get; set; }
        public string EmployeeName { get; set; }
        public DateTime? Dob { get; set; }
        public DateTime? EstStartDate { get; set; }
        public DateTime? StartDate { get; set; }
        public double? BaseSalary { get; set; }
        public double? ProbationSalary { get; set; }
        public double? AdditionalSalary { get; set; }
        public string BankAccount { get; set; }
        public bool? IsPrivateAccount { get; set; }
        public string BrandCode { get; set; }
        public string StoreCode { get; set; }
        public int? PositionId { get; set; }
        public bool? SubmittedAllDocuments { get; set; }
        public DateTime? TerminationDate { get; set; }
        public string TerminationReason { get; set; }
        public string Note { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual ICollection<HrEmployeeDocuments> HrEmployeeDocuments { get; set; }
    }
}
