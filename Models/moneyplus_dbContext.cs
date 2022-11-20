using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AlicjowyBackendv3.Models
{
    public partial class moneyplus_dbContext : DbContext
    {
        public moneyplus_dbContext()
        {
        }

        public moneyplus_dbContext(DbContextOptions<moneyplus_dbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Expense> Expenses { get; set; } = null!;
        public virtual DbSet<Receipt> Receipts { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserCategory> UserCategories { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseNpgsql("Host=moneyplus-server.postgres.database.azure.com;Database=moneyplus_db;Username=krzysztof_golusinski@moneyplus-server;Password=Am22Kg23");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("pg_buffercache")
                .HasPostgresExtension("pg_stat_statements");

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("categories");

                entity.Property(e => e.id).HasColumnName("category_id");

                entity.Property(e => e.categoryName)
                    .HasMaxLength(50)
                    .HasColumnName("category_name");

                entity.Property(e => e.color)
                    .HasMaxLength(50)
                    .HasColumnName("color");

                entity.Property(e => e.iconName)
                    .HasMaxLength(50)
                    .HasColumnName("icon_name");

                entity.Property(e => e.typeOfCategory)
                    .HasMaxLength(50)
                    .HasColumnName("type_of_category");
            });

            modelBuilder.Entity<Expense>(entity =>
            {
                entity.HasKey(e => e.id)
                    .HasName("expenses_pkey");

                entity.ToTable("expenses");

                entity.Property(e => e.id)
                    .HasMaxLength(40)
                    .HasColumnName("expense_guid");

                entity.Property(e => e.categoryId).HasColumnName("category_id");

                entity.Property(e => e.creationDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("creation_date");

                entity.Property(e => e.name)
                    .HasMaxLength(50)
                    .HasColumnName("expense_name");

                entity.Property(e => e.value)
                    .HasColumnType("money")
                    .HasColumnName("expense_value");

                entity.Property(e => e.userGuid)
                    .HasMaxLength(40)
                    .HasColumnName("user_guid");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Expenses)
                    .HasForeignKey(d => d.categoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("expenses_category_id_fkey");

                entity.HasOne(d => d.UserGu)
                    .WithMany(p => p.Expenses)
                    .HasForeignKey(d => d.userGuid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("expenses_user_guid_fkey");
            });

            modelBuilder.Entity<Receipt>(entity =>
            {
                entity.HasKey(e => e.id)
                    .HasName("receipts_pkey");

                entity.ToTable("receipts");

                entity.Property(e => e.id)
                    .HasMaxLength(40)
                    .HasColumnName("receipts_guid");

                entity.Property(e => e.categoryId).HasColumnName("category_id");

                entity.Property(e => e.creationDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("creation_date");

                entity.Property(e => e.name)
                    .HasMaxLength(50)
                    .HasColumnName("receipts_name");

                entity.Property(e => e.value)
                    .HasColumnType("money")
                    .HasColumnName("receipts_value");

                entity.Property(e => e.userGuid)
                    .HasMaxLength(40)
                    .HasColumnName("user_guid");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Receipts)
                    .HasForeignKey(d => d.categoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("receipts_category_id_fkey");

                entity.HasOne(d => d.UserGu)
                    .WithMany(p => p.Receipts)
                    .HasForeignKey(d => d.userGuid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("receipts_user_guid_fkey");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.id)
                    .HasName("users_pkey");

                entity.ToTable("users");

                entity.HasIndex(e => e.email, "users_email_key")
                    .IsUnique();

                entity.Property(e => e.id)
                    .HasMaxLength(40)
                    .HasColumnName("user_guid");

                entity.Property(e => e.age).HasColumnName("age");

                entity.Property(e => e.email)
                    .HasMaxLength(50)
                    .HasColumnName("email");

                entity.Property(e => e.firstName)
                    .HasMaxLength(50)
                    .HasColumnName("first_name");

                entity.Property(e => e.lastName)
                    .HasMaxLength(50)
                    .HasColumnName("last_name");

                entity.Property(e => e.passwordHash)
                    .HasMaxLength(100)
                    .HasColumnName("password_hash");

                entity.Property(e => e.passwordSalt)
                    .HasMaxLength(200)
                    .HasColumnName("password_salt");

                entity.Property(e => e.refreshToken)
                    .HasMaxLength(150)
                    .HasColumnName("refresh_token");

                entity.Property(e => e.tokenExpires)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("token_expires");
            });

            modelBuilder.Entity<UserCategory>(entity =>
            {
                entity.HasKey(e => e.id)
                    .HasName("user_category_pkey");

                entity.ToTable("user_category");

                entity.Property(e => e.id)
                    .HasMaxLength(40)
                    .HasColumnName("user_category_guid");

                entity.Property(e => e.categoryId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("category_id");

                entity.Property(e => e.userGuid)
                    .HasMaxLength(40)
                    .HasColumnName("user_guid");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.UserCategories)
                    .HasForeignKey(d => d.categoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_category_category_id_fkey");

                entity.HasOne(d => d.UserGu)
                    .WithMany(p => p.UserCategories)
                    .HasForeignKey(d => d.userGuid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_category_user_guid_fkey");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
