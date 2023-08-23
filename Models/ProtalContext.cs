using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Property.Models;

public partial class ProtalContext : DbContext
{
    public ProtalContext()
    {
    }

    public ProtalContext(DbContextOptions<ProtalContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<CategoryDetail> CategoryDetails { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Listing> Listings { get; set; }

    public virtual DbSet<Package> Packages { get; set; }

    public virtual DbSet<Realty> Realties { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-FQQHC09\\SQLEXPRESS;Database=protal;Trusted_Connection=True;MultipleActiveResultSets=true;trustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Accounts__349DA586F4B7E0A2");

            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.Avatar).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PackageId).HasColumnName("PackageID");
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Status).HasDefaultValueSql("((0))");
            entity.Property(e => e.Username).HasMaxLength(100);

            entity.HasOne(d => d.Package).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.PackageId)
                .HasConstraintName("FK__Accounts__Packag__4D94879B");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A2B7441A925");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.Status).HasDefaultValueSql("((0))");
        });

        modelBuilder.Entity<CategoryDetail>(entity =>
        {
            entity.HasKey(e => e.CategoryDetailId).HasName("PK__Category__6E94BCB2B5AD44B4");

            entity.Property(e => e.CategoryDetailId).HasColumnName("CategoryDetailID");
            entity.Property(e => e.CategoryDetailName).HasMaxLength(100);
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Status).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.Category).WithMany(p => p.CategoryDetails)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__CategoryD__Categ__398D8EEE");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__Images__7516F4ECD3633BC5");

            entity.Property(e => e.ImageId).HasColumnName("ImageID");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(200)
                .HasColumnName("ImageURL");
            entity.Property(e => e.RealtyId).HasColumnName("RealtyID");
            entity.Property(e => e.Status).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.Realty).WithMany(p => p.Images)
                .HasForeignKey(d => d.RealtyId)
                .HasConstraintName("FK__Images__Property__47DBAE45");
        });

        modelBuilder.Entity<Listing>(entity =>
        {
            entity.HasKey(e => e.ListingId).HasName("PK__Listings__BF3EBEF0C95E862A");

            entity.Property(e => e.ListingId).HasColumnName("ListingID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Duration).HasDefaultValueSql("((1))");
            entity.Property(e => e.RealtyId).HasColumnName("RealtyID");
            entity.Property(e => e.Status).HasDefaultValueSql("((0))");
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Account).WithMany(p => p.Listings)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__Listings__Accoun__571DF1D5");

            entity.HasOne(d => d.Realty).WithMany(p => p.Listings)
                .HasForeignKey(d => d.RealtyId)
                .HasConstraintName("FK__Listings__Proper__5629CD9C");
        });

        modelBuilder.Entity<Package>(entity =>
        {
            entity.HasKey(e => e.PackageId).HasName("PK__Packages__322035ECB2ADDF4B");

            entity.Property(e => e.PackageId).HasColumnName("PackageID");
            entity.Property(e => e.PackageName).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Realty>(entity =>
        {
            entity.HasKey(e => e.RealtyId).HasName("PK__Properti__70C9A755A6F308F8");

            entity.Property(e => e.RealtyId).HasColumnName("RealtyID");
            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.Area).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CategoryDetailId).HasColumnName("CategoryDetailID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.HouseDirection).HasMaxLength(200);
            entity.Property(e => e.Interior).HasMaxLength(200);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Status).HasDefaultValueSql("((0))");
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.CategoryDetail).WithMany(p => p.Realties)
                .HasForeignKey(d => d.CategoryDetailId)
                .HasConstraintName("FK__Realties__Catego__73BA3083");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Transact__55433A4B38CD089F");

            entity.Property(e => e.TransactionId).HasColumnName("TransactionID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.PackageId).HasColumnName("PackageID");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.Account).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__Transacti__Accou__5AEE82B9");

            entity.HasOne(d => d.Package).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.PackageId)
                .HasConstraintName("FK__Transacti__Packa__5BE2A6F2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
