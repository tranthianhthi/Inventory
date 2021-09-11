using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using CandidatePoolAPI.Models;

namespace CandidatePoolAPI
{
    public partial class ACFCInventoryContext : DbContext
    {
        public ACFCInventoryContext()
        {
        }

        public ACFCInventoryContext(DbContextOptions<ACFCInventoryContext> options)
            : base(options)
        {
        }

        public virtual DbSet<HrCandidate> HrCandidate { get; set; }
        public virtual DbSet<HrCandidateStatus> HrCandidateStatus { get; set; }
        public virtual DbSet<HrDataField> HrDataField { get; set; }
        public virtual DbSet<HrDocument> HrDocument { get; set; }
        public virtual DbSet<HrEmployee> HrEmployee { get; set; }
        public virtual DbSet<HrEmployeeDocuments> HrEmployeeDocuments { get; set; }
        public virtual DbSet<HrPosition> HrPosition { get; set; }
        public virtual DbSet<HrRecruitmentResource> HrRecruitmentResource { get; set; }
        public virtual DbSet<HrResourceDataConfiguration> HrResourceDataConfiguration { get; set; }
       

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=192.168.80.4;Initial Catalog=ACFCInventory;User ID=acfcmango;Password=acfcmango;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HrCandidate>(entity =>
            {
                entity.ToTable("HR_candidate");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasColumnName("address")
                    .HasMaxLength(100);

                entity.Property(e => e.CandidateName)
                    .HasColumnName("candidate_name")
                    .HasMaxLength(200);

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.District)
                    .HasColumnName("district")
                    .HasMaxLength(20);

                entity.Property(e => e.Dob)
                    .HasColumnName("dob")
                    .HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Gender)
                    .HasColumnName("gender")
                    .HasMaxLength(10);

                entity.Property(e => e.MobileNo)
                    .IsRequired()
                    .HasColumnName("mobile_no")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedBy)
                    .HasColumnName("modified_by")
                    .HasMaxLength(210);

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modified_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Note)
                    .HasColumnName("note")
                    .HasMaxLength(4000);

                entity.Property(e => e.Position)
                    .HasColumnName("position")
                    .HasMaxLength(200);

                entity.Property(e => e.ResourceId).HasColumnName("resource_id");

                entity.Property(e => e.ResumeId).HasColumnName("resume_id");

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.Property(e => e.SubmittedDate)
                    .HasColumnName("submitted_date")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Resource)
                    .WithMany(p => p.HrCandidate)
                    .HasForeignKey(d => d.ResourceId)
                    .HasConstraintName("FK_candidate_recruitment_resource");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.HrCandidate)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK_candidate_candidate_status");
            });

            modelBuilder.Entity<HrCandidateStatus>(entity =>
            {
                entity.ToTable("HR_candidate_status");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.PrvStatusId).HasColumnName("prv_status_id");

                entity.Property(e => e.StatusTxt)
                    .IsRequired()
                    .HasColumnName("status_txt")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<HrDataField>(entity =>
            {
                entity.ToTable("HR_data_field");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ActiveFlag).HasColumnName("active_flag");

                entity.Property(e => e.CheckExisted).HasColumnName("check_existed");

                entity.Property(e => e.FieldName)
                    .IsRequired()
                    .HasColumnName("field_name")
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<HrDocument>(entity =>
            {
                entity.ToTable("HR_document");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ActiveFlag).HasColumnName("active_flag");

                entity.Property(e => e.DocumentName)
                    .IsRequired()
                    .HasColumnName("document_name")
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<HrEmployee>(entity =>
            {
                entity.ToTable("HR_employee");

                entity.HasIndex(e => e.CandidateId)
                    .HasName("IX_HR_employee")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AdditionalSalary).HasColumnName("additional_salary");

                entity.Property(e => e.BankAccount)
                    .HasColumnName("bank_account")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.BaseSalary).HasColumnName("base_salary");

                entity.Property(e => e.BrandCode)
                    .HasColumnName("brand_code")
                    .HasMaxLength(210);

                entity.Property(e => e.CandidateId).HasColumnName("candidate_id");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Dob)
                    .HasColumnName("dob")
                    .HasColumnType("datetime");

                entity.Property(e => e.EmployeeName)
                    .HasColumnName("employee_name")
                    .HasMaxLength(500);

                entity.Property(e => e.EmployeeNo)
                    .HasColumnName("employee_no")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.EstStartDate)
                    .HasColumnName("est_start_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.IsPrivateAccount).HasColumnName("is_private_account");

                entity.Property(e => e.Note)
                    .HasColumnName("note")
                    .HasMaxLength(4000);

                entity.Property(e => e.PositionId).HasColumnName("position_id");

                entity.Property(e => e.ProbationSalary).HasColumnName("probation_salary");

                entity.Property(e => e.StartDate)
                    .HasColumnName("start_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.StoreCode)
                    .HasColumnName("store_code")
                    .HasMaxLength(100);

                entity.Property(e => e.SubmittedAllDocuments).HasColumnName("submitted_all_documents");

                entity.Property(e => e.TerminationDate)
                    .HasColumnName("termination_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.TerminationReason)
                    .HasColumnName("termination_reason")
                    .HasMaxLength(4000);
            });

            modelBuilder.Entity<HrEmployeeDocuments>(entity =>
            {
                entity.ToTable("HR_employee_documents");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DocumentId).HasColumnName("document_id");

                entity.Property(e => e.EmployeeId).HasColumnName("employee_id");

                entity.Property(e => e.Submitted).HasColumnName("submitted");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updated_date")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Document)
                    .WithMany(p => p.HrEmployeeDocuments)
                    .HasForeignKey(d => d.DocumentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_employee_documents_document");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.HrEmployeeDocuments)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_employee_documents_employee");
            });

            modelBuilder.Entity<HrPosition>(entity =>
            {
                entity.ToTable("HR_position");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ActiveFlag).HasColumnName("active_flag");

                entity.Property(e => e.IsStorePosition).HasColumnName("is_store_position");

                entity.Property(e => e.PositionName)
                    .IsRequired()
                    .HasColumnName("position_name")
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<HrRecruitmentResource>(entity =>
            {
                entity.ToTable("HR_recruitment_resource");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ActiveFlag).HasColumnName("active_flag");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.IsLocalResource).HasColumnName("is_local_resource");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnName("last_updated_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.ReportStartRow).HasColumnName("report_start_row");

                entity.Property(e => e.ResourceName)
                    .IsRequired()
                    .HasColumnName("resource_name")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ResourceUrl)
                    .HasColumnName("resource_url")
                    .HasMaxLength(1000);
            });

            modelBuilder.Entity<HrResourceDataConfiguration>(entity =>
            {
                entity.ToTable("HR_resource_data_configuration");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ActiveFlag).HasColumnName("active_flag");

                entity.Property(e => e.ColumnName)
                    .IsRequired()
                    .HasColumnName("column_name")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.FieldFormat)
                    .HasColumnName("field_format")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.FieldId).HasColumnName("field_id");

                entity.Property(e => e.ResourceId).HasColumnName("resource_id");

                entity.Property(e => e.RowNum).HasColumnName("row_num");

                entity.HasOne(d => d.Field)
                    .WithMany(p => p.HrResourceDataConfiguration)
                    .HasForeignKey(d => d.FieldId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_resource_report_configuration_report_field_configuration");

                entity.HasOne(d => d.Resource)
                    .WithMany(p => p.HrResourceDataConfiguration)
                    .HasForeignKey(d => d.ResourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_resource_report_configuration_recruitment_resource");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
