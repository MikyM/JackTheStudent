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

        public virtual DbSet<ClassTypes> ClassTypes { get; set; }
        public virtual DbSet<Group> Group { get; set; }
        public virtual DbSet<ClassMaterials> ClassMaterials { get; set; }
        public virtual DbSet<Exams> Exams { get; set; }
        public virtual DbSet<Homeworks> Homeworks { get; set; }
        public virtual DbSet<LabReports> LabReports { get; set; }
        public virtual DbSet<PersonalReminders> PersonalReminders { get; set; }
        public virtual DbSet<ShortTests> ShortTests { get; set; }
        public virtual DbSet<DickAppointments> DickAppointments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured) {
                optionsBuilder.UseMySQL(Environment.GetEnvironmentVariable("JACKTHESTUDENT_DB_CON_STRING"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClassTypes>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PRIMARY");

                entity.ToTable("class_types");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(45);

                entity.Property(e => e.ShortName)
                    .IsRequired()
                    .HasColumnName("short_name")
                    .HasMaxLength(10);   
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PRIMARY");

                entity.ToTable("group");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.GroupId)
                    .IsRequired()
                    .HasColumnName("group")
                    .HasMaxLength(20); 
            });

            modelBuilder.Entity<ClassMaterials>(entity =>
            {
                entity.HasKey(e => e.IdMaterial)
                    .HasName("PRIMARY");

                entity.ToTable("class_materials");

                entity.Property(e => e.IdMaterial)
                    .HasColumnName("id")
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

                entity.Property(e => e.LogById)
                    .IsRequired()
                    .HasColumnName("log_by_id")
                    .HasColumnType("longtext");

                entity.Property(e => e.LogByUsername)
                    .IsRequired()
                    .HasColumnName("log_by_username")
                    .HasColumnType("longtext");

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
                    .HasColumnName("id")
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

                entity.Property(e => e.LogById)
                    .IsRequired()
                    .HasColumnName("log_by_id")
                    .HasColumnType("longtext");

                entity.Property(e => e.LogByUsername)
                    .IsRequired()
                    .HasColumnName("log_by_username")
                    .HasColumnType("longtext");    

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
                    .HasColumnName("id")
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

                entity.Property(e => e.GroupId)
                    .IsRequired()
                    .HasColumnName("group_id");
                
                entity.Property(e => e.LogById)
                    .IsRequired()
                    .HasColumnName("log_by_id")
                    .HasColumnType("longtext");

                entity.Property(e => e.LogByUsername)
                    .IsRequired()
                    .HasColumnName("log_by_username")
                    .HasColumnType("longtext");

                entity.Property(e => e.Materials)
                    .HasColumnName("materials")
                    .HasColumnType("longtext");
            });

            modelBuilder.Entity<LabReports>(entity =>
            {
                entity.HasKey(e => e.IdLabReport)
                    .HasName("PRIMARY");

                entity.ToTable("lab");

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

                entity.Property(e => e.Date)
                    .IsRequired()
                    .HasColumnName("date");

                entity.Property(e => e.GroupId)
                    .IsRequired()
                    .HasColumnName("group_id");
                
                entity.Property(e => e.LogById)
                    .IsRequired()
                    .HasColumnName("log_by_id")
                    .HasColumnType("longtext");

                entity.Property(e => e.LogByUsername)
                    .IsRequired()
                    .HasColumnName("log_by_username")
                    .HasColumnType("longtext");

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
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.About)
                    .IsRequired()
                    .HasColumnName("about")
                    .HasMaxLength(45);

                entity.Property(e => e.AdditionalInfo)
                    .HasColumnName("additional_info")
                    .HasColumnType("longtext");

                entity.Property(e => e.LogById)
                    .IsRequired()
                    .HasColumnName("log_by_id")
                    .HasColumnType("longtext");

                entity.Property(e => e.LogByUsername)
                    .IsRequired()
                    .HasColumnName("log_by_username")
                    .HasColumnType("longtext");

                entity.Property(e => e.Materials)
                    .HasColumnName("materials")
                    .HasColumnType("longtext");
            });

            modelBuilder.Entity<ShortTests>(entity =>
            {
                entity.HasKey(e => e.IdShortTest)
                    .HasName("PRIMARY");

                entity.ToTable("short_tests");

                entity.Property(e => e.IdShortTest)
                    .HasColumnName("id")
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

                entity.Property(e => e.GroupId)
                    .IsRequired()
                    .HasColumnName("group_id");
                    
                entity.Property(e => e.LogById)
                    .IsRequired()
                    .HasColumnName("log_by_id")
                    .HasColumnType("longtext");

                entity.Property(e => e.LogByUsername)
                    .IsRequired()
                    .HasColumnName("log_by_username")
                    .HasColumnType("longtext");

                entity.Property(e => e.Materials)
                    .HasColumnName("materials")
                    .HasColumnType("longtext");
            });

            modelBuilder.Entity<DickAppointments>(entity =>
            {
                entity.HasKey(e => e.IdDickAppointment)
                    .HasName("PRIMARY");

                entity.ToTable("dick_appointments");

                entity.Property(e => e.IdDickAppointment)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("longtext")
                    .HasMaxLength(45);

                entity.Property(e => e.Length)
                    .IsRequired()
                    .HasColumnName("length")
                    .HasMaxLength(45);
                
                entity.Property(e => e.Circumference)
                    .IsRequired()
                    .HasColumnName("circumference")
                    .HasMaxLength(45);

                entity.Property(e => e.Width)
                    .IsRequired()
                    .HasColumnName("width")
                    .HasMaxLength(45);

                entity.Property(e => e.Date)
                    .IsRequired()
                    .HasColumnName("date");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
