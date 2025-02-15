using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using verbum_service_domain.Models;

namespace verbum_service_infrastructure.DataContext;

public partial class verbumContext : DbContext
{
    public verbumContext(DbContextOptions<verbumContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<ClientTransaction> ClientTransactions { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Issue> Issues { get; set; }

    public virtual DbSet<IssueAttachment> IssueAttachments { get; set; }

    public virtual DbSet<Job> Jobs { get; set; }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderReference> OrderReferences { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Receipt> Receipts { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Revelancy> Revelancies { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Work> Works { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("category_pk");

            entity.ToTable("category");

            entity.Property(e => e.CategoryId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("category_id");
            entity.Property(e => e.CategoryName)
                .HasColumnType("character varying")
                .HasColumnName("category_name");
        });

        modelBuilder.Entity<ClientTransaction>(entity =>
        {
            entity.HasKey(e => new { e.TransactionId, e.ClientId }).HasName("client_transaction_pk");

            entity.ToTable("client_transaction");

            entity.HasIndex(e => e.Orderid, "client_transaction_unique").IsUnique();

            entity.Property(e => e.TransactionId)
                .HasColumnType("character varying")
                .HasColumnName("transaction_id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.IsDeposit)
                .HasDefaultValue(false)
                .HasColumnName("is_deposit");
            entity.Property(e => e.Orderid).HasColumnName("orderid");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.DiscountId).HasName("discount_pk");

            entity.ToTable("discount");

            entity.Property(e => e.DiscountId)
                .ValueGeneratedNever()
                .HasColumnName("discount_id");
            entity.Property(e => e.DiscountName)
                .HasColumnType("character varying")
                .HasColumnName("discount_name");
            entity.Property(e => e.DiscountPercent).HasColumnName("discount_percent");
        });

        modelBuilder.Entity<Issue>(entity =>
        {
            entity.HasKey(e => e.IssueId).HasName("issue_pk");

            entity.ToTable("issue", tb => tb.HasComment("tạo issue, job tạo ra bởi issue : OPEN\r\n\r\nSM accept issue from client: IN_PROGRESS\r\nSM reject issue from client: CANCEL\r\n\r\nLinguist đánh: SUBMITTED\r\n\r\nSM review linguist ok: RESOLVED \r\nSM reject resolve from linguist: IN_PROGRESS\r\n\r\nclient cancel lúc nào cx đc: CANCEL"));

            entity.HasIndex(e => e.JobId, "issue_unique").IsUnique();

            entity.Property(e => e.IssueId)
                .ValueGeneratedNever()
                .HasColumnName("issue_id");
            entity.Property(e => e.AssigneeId).HasColumnName("assignee_id");
            entity.Property(e => e.CancelResponse)
                .HasColumnType("character varying")
                .HasColumnName("cancel_response");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.IssueDescription)
                .HasColumnType("character varying")
                .HasColumnName("issue_description");
            entity.Property(e => e.IssueName)
                .HasColumnType("character varying")
                .HasColumnName("issue_name");
            entity.Property(e => e.JobId).HasColumnName("job_id");
            entity.Property(e => e.RejectResponse)
                .HasColumnType("character varying")
                .HasColumnName("reject_response");
            entity.Property(e => e.SrcDocumentUrl)
                .HasColumnType("character varying")
                .HasColumnName("src_document_url");
            entity.Property(e => e.Status)
                .HasComment("CANCEL, OPEN, RESOLVED, ACCEPTED, IN-PROGRESS")
                .HasColumnType("character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Assignee).WithMany(p => p.IssueAssignees)
                .HasForeignKey(d => d.AssigneeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("issue_assign_fk");

            entity.HasOne(d => d.Client).WithMany(p => p.IssueClients)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("issue_user_fk");

            entity.HasOne(d => d.Job).WithOne(p => p.Issue)
                .HasForeignKey<Issue>(d => d.JobId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("issue_job_fk");
        });

        modelBuilder.Entity<IssueAttachment>(entity =>
        {
            entity.HasKey(e => new { e.IssueId, e.AttachmentUrl }).HasName("issue_attachments_pk");

            entity.ToTable("issue_attachments", tb => tb.HasComment("tag lưu file solution và reference file: SOLUTION, ATTACHMENT"));

            entity.Property(e => e.IssueId).HasColumnName("issue_id");
            entity.Property(e => e.AttachmentUrl)
                .HasColumnType("character varying")
                .HasColumnName("attachment_url");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.Tag)
                .HasColumnType("character varying")
                .HasColumnName("tag");

            entity.HasOne(d => d.Issue).WithMany(p => p.IssueAttachments)
                .HasForeignKey(d => d.IssueId)
                .HasConstraintName("issue_attachments_issue_fk");
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("job_pkey");

            entity.ToTable("job", tb => tb.HasComment("chưa assign:NEW\r\n\r\nasign linguists: IN_PROGRESS\r\n \r\nlinguist làm xong: SUBMITTED\r\n\r\nSM review ok: APPROVED\r\nSM reject: IN_PROGRESS\r\n\r\nclient tạo issue thì tạo thêm 1 job mới ở sevice cuối cùng(TL-> ED -> EV)"));

            entity.HasIndex(e => e.DocumentUrl, "job_document_url_idx");

            entity.HasIndex(e => e.DeliverableUrl, "job_unique").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeliverableUrl)
                .HasColumnType("character varying")
                .HasColumnName("deliverable_url");
            entity.Property(e => e.DocumentUrl).HasColumnName("document_url");
            entity.Property(e => e.DueDate)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("due_date");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.RejectReason)
                .HasColumnType("character varying")
                .HasColumnName("reject_reason");
            entity.Property(e => e.Status)
                .HasComment("NEW, IN_PROGRESS, SUBMITTED, ACCEPTED")
                .HasColumnName("status");
            entity.Property(e => e.TargetLanguageId)
                .HasColumnType("character varying")
                .HasColumnName("target_language_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.WordCount).HasColumnName("word_count");
            entity.Property(e => e.WorkId).HasColumnName("work_id");

            entity.HasOne(d => d.Work).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.WorkId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("task_work_fk");
        });

        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasKey(e => e.LanguageId).HasName("language_pk");

            entity.ToTable("language");

            entity.Property(e => e.LanguageId)
                .HasColumnType("character varying")
                .HasColumnName("language_id");
            entity.Property(e => e.LanguageName).HasColumnName("language_name");
            entity.Property(e => e.Support)
                .HasDefaultValue(false)
                .HasColumnName("support");

            entity.HasMany(d => d.OrdersNavigation).WithMany(p => p.TargetLanguages)
                .UsingEntity<Dictionary<string, object>>(
                    "OrderTargetLanguage",
                    r => r.HasOne<Order>().WithMany()
                        .HasForeignKey("OrderId")
                        .HasConstraintName("order_target_language_order_fk"),
                    l => l.HasOne<Language>().WithMany()
                        .HasForeignKey("TargetLanguageId")
                        .HasConstraintName("order_target_language_language_fk"),
                    j =>
                    {
                        j.HasKey("TargetLanguageId", "OrderId").HasName("order_target_language_pk");
                        j.ToTable("order_target_language");
                        j.IndexerProperty<string>("TargetLanguageId")
                            .HasColumnType("character varying")
                            .HasColumnName("target_language_id");
                        j.IndexerProperty<Guid>("OrderId").HasColumnName("order_id");
                    });
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("order_pk");

            entity.ToTable("order", tb => tb.HasComment("tạo: NEW\r\n\r\nstaff: ACCEPTED, REJECTED, \r\n\r\nclient CANCELED\r\n\r\ndeposit xong:IN_PROGRESS, \r\n\r\nxong tất cả các job: COMPLETED.\r\n\r\nclient tạo isue: IN_PROGRESS\r\n\r\nresolve xong isue: COMPLETED\r\n\r\npay xong: DELIVERED"));

            entity.Property(e => e.OrderId)
                .ValueGeneratedNever()
                .HasColumnName("order_id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.DiscountId).HasColumnName("discount_id");
            entity.Property(e => e.DueDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("due_date");
            entity.Property(e => e.HasEditService)
                .HasDefaultValue(false)
                .HasColumnName("has_edit_service");
            entity.Property(e => e.HasEvaluateService)
                .HasDefaultValue(false)
                .HasColumnName("has_evaluate_service");
            entity.Property(e => e.HasTranslateService)
                .HasDefaultValue(false)
                .HasColumnName("has_translate_service");
            entity.Property(e => e.OrderName)
                .HasColumnType("character varying")
                .HasColumnName("order_name");
            entity.Property(e => e.OrderNote)
                .HasComment("255 char")
                .HasColumnType("character varying")
                .HasColumnName("order_note");
            entity.Property(e => e.OrderPrice).HasColumnName("order_price");
            entity.Property(e => e.OrderStatus)
                .HasColumnType("character varying")
                .HasColumnName("order_status");
            entity.Property(e => e.RejectReason)
                .HasColumnType("character varying")
                .HasColumnName("reject_reason");
            entity.Property(e => e.SourceLanguageId)
                .HasColumnType("character varying")
                .HasColumnName("source_language_id");

            entity.HasOne(d => d.Client).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("order_user_fk");

            entity.HasOne(d => d.Discount).WithMany(p => p.Orders)
                .HasForeignKey(d => d.DiscountId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("order_discount_fk");

            entity.HasOne(d => d.SourceLanguage).WithMany(p => p.Orders)
                .HasForeignKey(d => d.SourceLanguageId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("order_language_fk");
        });

        modelBuilder.Entity<OrderReference>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.ReferenceFileUrl }).HasName("order_references_pk");

            entity.ToTable("order_references", tb => tb.HasComment("TRANSLATION, REFERENCES"));

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ReferenceFileUrl)
                .HasColumnType("character varying")
                .HasColumnName("reference_file_url");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.Tag)
                .HasColumnType("character varying")
                .HasColumnName("tag");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderReferences)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("order_references_order_fk");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.RatingId).HasName("rating_pk");

            entity.ToTable("rating");

            entity.Property(e => e.RatingId)
                .ValueGeneratedNever()
                .HasColumnName("rating_id");
            entity.Property(e => e.Expectation).HasColumnName("expectation");
            entity.Property(e => e.InTime).HasColumnName("in_time");
            entity.Property(e => e.IssueResolved).HasColumnName("issue_resolved");
            entity.Property(e => e.MoreThought)
                .HasColumnType("character varying")
                .HasColumnName("more_thought");
            entity.Property(e => e.OrderId).HasColumnName("order_id");

            entity.HasOne(d => d.Order).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("rating_order_fk");
        });

        modelBuilder.Entity<Receipt>(entity =>
        {
            entity.HasKey(e => e.ReceiptId).HasName("receipt_pk");

            entity.ToTable("receipt", tb => tb.HasComment("true is deposit, false is payment"));

            entity.Property(e => e.ReceiptId)
                .ValueGeneratedNever()
                .HasColumnName("receipt_id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.DepositeOrPayment).HasColumnName("deposite_or_payment");
            entity.Property(e => e.Done)
                .HasDefaultValue(false)
                .HasColumnName("done");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.PayDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("pay_date");
            entity.Property(e => e.PaymentId)
                .HasColumnType("character varying")
                .HasColumnName("payment_id");

            entity.HasOne(d => d.Order).WithMany(p => p.Receipts)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("receipt_order_fk");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("refreshtoken_pk");

            entity.ToTable("refresh_token");

            entity.Property(e => e.TokenId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("token_id");
            entity.Property(e => e.ExpireAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("expire_at");
            entity.Property(e => e.IssuedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("issued_at");
            entity.Property(e => e.TokenContent)
                .HasColumnType("character varying")
                .HasColumnName("token_content");
        });

        modelBuilder.Entity<Revelancy>(entity =>
        {
            entity.HasKey(e => e.RevelancyId).HasName("revelancy_pk");

            entity.ToTable("revelancy");

            entity.Property(e => e.RevelancyId)
                .ValueGeneratedNever()
                .HasColumnName("revelancy_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.ServiceCode)
                .HasColumnType("character varying")
                .HasColumnName("service_code");
            entity.Property(e => e.SourceLanguageId)
                .HasColumnType("character varying")
                .HasColumnName("source_language_id");
            entity.Property(e => e.TargetLanguageId)
                .HasColumnType("character varying")
                .HasColumnName("target_language_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Category).WithMany(p => p.Revelancies)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("revelancy_category_fk");

            entity.HasOne(d => d.SourceLanguage).WithMany(p => p.RevelancySourceLanguages)
                .HasForeignKey(d => d.SourceLanguageId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("revelancy_language_fk");

            entity.HasOne(d => d.TargetLanguage).WithMany(p => p.RevelancyTargetLanguages)
                .HasForeignKey(d => d.TargetLanguageId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("revelancy_language_fk_1");

            entity.HasOne(d => d.User).WithMany(p => p.Revelancies)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("revelancy_user_fk");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("Role_pkey");

            entity.ToTable("role");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName).HasColumnName("role_name");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceCode).HasName("services_pk");

            entity.ToTable("services");

            entity.Property(e => e.ServiceCode)
                .HasColumnType("character varying")
                .HasColumnName("service_code");
            entity.Property(e => e.ServiceName)
                .HasColumnType("character varying")
                .HasColumnName("service_name");
            entity.Property(e => e.ServiceOrder).HasColumnName("service_order");
            entity.Property(e => e.ServicePrice).HasColumnName("service_price");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("User_pkey");

            entity.ToTable("user", tb => tb.HasComment("confirm mail: ACTIVE\r\ntạo tài khoản: INACTIVE"));

            entity.HasIndex(e => e.Email, "User_email_key").IsUnique();

            entity.HasIndex(e => new { e.Email, e.Password }, "user_login_idx");

            entity.HasIndex(e => e.TokenId, "user_unique").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp(3) without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.EmailVerified)
                .HasColumnType("timestamp(3) without time zone")
                .HasColumnName("email_verified");
            entity.Property(e => e.ImageId).HasColumnName("image_id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.RoleCode)
                .HasColumnType("character varying")
                .HasColumnName("role_code");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'ACTIVE'::text")
                .HasColumnName("status");
            entity.Property(e => e.TokenId).HasColumnName("token_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp(3) without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.RoleCodeNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleCode)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("user_role_fk");

            entity.HasOne(d => d.Token).WithOne(p => p.User)
                .HasForeignKey<User>(d => d.TokenId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("user_refresh_token_fk");

            entity.HasMany(d => d.Jobs).WithMany(p => p.Assignees)
                .UsingEntity<Dictionary<string, object>>(
                    "AssigneeJob",
                    r => r.HasOne<Job>().WithMany()
                        .HasForeignKey("JobId")
                        .HasConstraintName("assignee_job_job_fk"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("AssigneeId")
                        .HasConstraintName("assignee_job_user_fk"),
                    j =>
                    {
                        j.HasKey("AssigneeId", "JobId").HasName("assignee_job_pk");
                        j.ToTable("assignee_job");
                        j.IndexerProperty<Guid>("AssigneeId").HasColumnName("assignee_id");
                        j.IndexerProperty<Guid>("JobId").HasColumnName("job_id");
                    });
        });

        modelBuilder.Entity<Work>(entity =>
        {
            entity.HasKey(e => e.WorkId).HasName("work_pk");

            entity.ToTable("work");

            entity.Property(e => e.WorkId)
                .ValueGeneratedNever()
                .HasColumnName("work_id");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.DueDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("due_date");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ServiceCode)
                .HasColumnType("character varying")
                .HasColumnName("service_code");
            entity.Property(e => e.WorkName)
                .HasColumnType("character varying")
                .HasColumnName("work_name");

            entity.HasOne(d => d.Order).WithMany(p => p.Works)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("work_order_fk");

            entity.HasOne(d => d.ServiceCodeNavigation).WithMany(p => p.Works)
                .HasForeignKey(d => d.ServiceCode)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("work_services_fk");

            entity.HasMany(d => d.Categories).WithMany(p => p.Works)
                .UsingEntity<Dictionary<string, object>>(
                    "WorkCategory",
                    r => r.HasOne<Category>().WithMany()
                        .HasForeignKey("CategoryId")
                        .HasConstraintName("work_category_category_fk"),
                    l => l.HasOne<Work>().WithMany()
                        .HasForeignKey("WorkId")
                        .HasConstraintName("work_category_work_fk"),
                    j =>
                    {
                        j.HasKey("WorkId", "CategoryId").HasName("work_category_pk");
                        j.ToTable("work_category");
                        j.IndexerProperty<Guid>("WorkId").HasColumnName("work_id");
                        j.IndexerProperty<int>("CategoryId").HasColumnName("category_id");
                    });
        });
        modelBuilder.HasSequence("order_name_seq");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
