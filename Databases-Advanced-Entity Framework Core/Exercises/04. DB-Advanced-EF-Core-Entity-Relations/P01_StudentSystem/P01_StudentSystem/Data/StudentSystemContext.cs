using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data.Models;
using System;

namespace P01_StudentSystem.Data
{
    public class StudentSystemContext : DbContext
    {
        public StudentSystemContext()
        {
        }

        public StudentSystemContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<Homework> HomeworkSubmissions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.StudentId);

                entity.Property(e => e.Name)
                .IsUnicode()
                .HasMaxLength(100);

                entity.Property(e => e.PhoneNumber)
                .IsUnicode(false)
                .IsRequired(false)
                .HasColumnType("CHAR(10)");

                entity.Property(e => e.RegisteredOn)
                .HasColumnType("DATETIME2");

                //entity.Property(e => e.Birthday)
                //.IsRequired(false);
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.CourseId);

                entity.Property(e => e.Name)
                .IsUnicode()
                .HasMaxLength(80);

                entity.Property(e => e.Description)
                .IsUnicode()
                .IsRequired();
            });

            modelBuilder.Entity<Resource>(entity =>
            {
                entity.HasKey(e => e.ResourceId);

                entity.Property(e => e.Name)
                .IsUnicode()
                .HasMaxLength(50);

                entity.Property(e => e.Url)
                .IsUnicode(false);

                entity.HasOne(e => e.Course)
                .WithMany(c => c.Resources)
                .HasForeignKey(e => e.CourseId);
            });

            modelBuilder.Entity<Homework>(entity =>
            {
                entity.HasOne(e => e.Course)
                .WithMany(c => c.HomeworkSubmissions)
                .HasForeignKey(e => e.CourseId);

                entity.HasOne(e => e.Student)
                .WithMany(s => s.HomeworkSubmissions)
                .HasForeignKey(e => e.StudentId);
            });

            modelBuilder.Entity<StudentCourse>(entity =>
            {
                entity.HasKey(e => new { e.StudentId, e.CourseId });

                entity.HasOne(e => e.Student)
                .WithMany(s => s.CourseEnrollments)
                .HasForeignKey(e => e.StudentId);

                entity.HasOne(e => e.Course)
                .WithMany(c => c.StudentsEnrolled)
                .HasForeignKey(e => e.CourseId);

            });
        }


    }
}
