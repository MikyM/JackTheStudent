using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace JackTheStudent.Models
{
    public partial class JackTheStudentContext : DbContext
    {
        public JackTheStudentContext()
        {
        }

        public JackTheStudentContext(DbContextOptions<JackTheStudentContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ClassMaterials> ClassMaterials { get; set; }
        public virtual DbSet<Exams> Exams { get; set; }
        public virtual DbSet<Homeworks> Homeworks { get; set; }
        public virtual DbSet<LabReports> LabReports { get; set; }
        public virtual DbSet<PersonalReminders> PersonalReminders { get; set; }
        public virtual DbSet<ShortTests> ShortTests { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySQL(Environment.GetEnvironmentVariable("JACKTHESTUDENT_DB_CON_STRING"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClassMaterials>(entity =>
            {
                entity.HasKey(e => e.IdMaterial)
                    .HasName("PRIMARY");

                entity.ToTable("class_materials");

                entity.Property(e => e.IdMaterial)
                    .HasColumnName("id_material")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AdditionalInfo)
                    .HasColumnName("additional_info")
                    .HasColumnType("longtext");

                entity.Property(e => e.Class)
                    .IsRequired()
                    .HasColumnName("class")
                    .HasMaxLength(45);

                entity.Property(e => e.LogBy)
                    .IsRequired()
                    .HasColumnName("log_by")
                    .HasMaxLength(45);

                entity.Property(e => e.Materials)
                    .IsRequired()
                    .HasColumnName("materials")
                    .HasColumnType("longtext");
            });

            modelBuilder.Entity<Exams>(entity =>
            {
                entity.HasKey(e => e.IdExam)
                    .HasName("PRIMARY");

                entity.ToTable("exams");

                entity.Property(e => e.IdExam)
                    .HasColumnName("id_exam")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AdditionalInfo)
                    .HasColumnName("additional_info")
                    .HasColumnType("longtext");

                entity.Property(e => e.Class)
                    .IsRequired()
                    .HasColumnName("class")
                    .HasMaxLength(45);
                
                entity.Property(e => e.Date)
                    .IsRequired()
                    .HasColumnName("date");

                entity.Property(e => e.LogBy)
                    .IsRequired()
                    .HasColumnName("log_by")
                    .HasMaxLength(45);

                entity.Property(e => e.Materials)
                    .HasColumnName("materials")
                    .HasColumnType("longtext");
            });

            modelBuilder.Entity<Homeworks>(entity =>
            {
                entity.HasKey(e => e.IdHomework)
                    .HasName("PRIMARY");

                entity.ToTable("homeworks");

                entity.Property(e => e.IdHomework)
                    .HasColumnName("id_homework")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AdditionalInfo)
                    .HasColumnName("additional_info")
                    .HasColumnType("longtext");

                entity.Property(e => e.Class)
                    .IsRequired()
                    .HasColumnName("class")
                    .HasMaxLength(45);

                entity.Property(e => e.LogBy)
                    .IsRequired()
                    .HasColumnName("log_by")
                    .HasMaxLength(45);

                entity.Property(e => e.Materials)
                    .HasColumnName("materials")
                    .HasColumnType("longtext");
            });

            modelBuilder.Entity<LabReports>(entity =>
            {
                entity.HasKey(e => e.IdLabReport)
                    .HasName("PRIMARY");

                entity.ToTable("lab_reports");

                entity.Property(e => e.IdLabReport)
                    .HasColumnName("id_lab_report")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AdditionalInfo)
                    .HasColumnName("additional_info")
                    .HasColumnType("longtext");

                entity.Property(e => e.Class)
                    .IsRequired()
                    .HasColumnName("class")
                    .HasMaxLength(45);

                entity.Property(e => e.LogBy)
                    .IsRequired()
                    .HasColumnName("log_by")
                    .HasMaxLength(45);

                entity.Property(e => e.Materials)
                    .HasColumnName("materials")
                    .HasColumnType("longtext");
            });

            modelBuilder.Entity<PersonalReminders>(entity =>
            {
                entity.HasKey(e => e.IdReminder)
                    .HasName("PRIMARY");

                entity.ToTable("personal_reminders");

                entity.Property(e => e.IdReminder)
                    .HasColumnName("id_reminder")
                    .HasColumnType("int(11)");

                entity.Property(e => e.About)
                    .IsRequired()
                    .HasColumnName("about")
                    .HasMaxLength(45);

                entity.Property(e => e.AdditionalInfo)
                    .HasColumnName("additional_info")
                    .HasColumnType("longtext");

                entity.Property(e => e.LogBy)
                    .IsRequired()
                    .HasColumnName("log_by")
                    .HasMaxLength(45);

                entity.Property(e => e.Materials)
                    .HasColumnName("materials")
                    .HasColumnType("longtext");
            });

            modelBuilder.Entity<ShortTests>(entity =>
            {
                entity.HasKey(e => e.IdShortTest)
                    .HasName("PRIMARY");

                entity.ToTable("quick_tests");

                entity.Property(e => e.IdShortTest)
                    .HasColumnName("id_quick_test")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AdditionalInfo)
                    .HasColumnName("additional_info")
                    .HasColumnType("longtext");

                entity.Property(e => e.Class)
                    .IsRequired()
                    .HasColumnName("class")
                    .HasMaxLength(45);

                entity.Property(e => e.LogBy)
                    .IsRequired()
                    .HasColumnName("log_by")
                    .HasMaxLength(45);

                entity.Property(e => e.Materials)
                    .HasColumnName("materials")
                    .HasColumnType("longtext");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
