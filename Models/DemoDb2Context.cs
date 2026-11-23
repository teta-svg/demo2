using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace demo2.Models;

public partial class DemoDb2Context : DbContext
{
    public DemoDb2Context()
    {
    }

    public DemoDb2Context(DbContextOptions<DemoDb2Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Adress> Adresses { get; set; }

    public virtual DbSet<Caterory> Caterories { get; set; }

    public virtual DbSet<Manufacturer> Manufacturers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Person> People { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Sity> Sities { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=DESKTOP-9CLF3RH\\SQLEXPRESS;Database=DemoDb2;Trusted_Connection=true;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Adress>(entity =>
        {
            entity.HasKey(e => e.IdAdress);

            entity.ToTable("Adress");

            entity.Property(e => e.IdAdress)
                .ValueGeneratedNever()
                .HasColumnName("Id_Adress");
            entity.Property(e => e.IdSity).HasColumnName("Id_Sity");
            entity.Property(e => e.Street).HasMaxLength(255);

            entity.HasOne(d => d.IdSityNavigation).WithMany(p => p.Adresses)
                .HasForeignKey(d => d.IdSity)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Adress_Sity");
        });

        modelBuilder.Entity<Caterory>(entity =>
        {
            entity.HasKey(e => e.IdCategory);

            entity.ToTable("Caterory");

            entity.Property(e => e.IdCategory)
                .ValueGeneratedNever()
                .HasColumnName("Id_Category");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.HasKey(e => e.IdManufacturer);

            entity.ToTable("Manufacturer");

            entity.Property(e => e.IdManufacturer)
                .ValueGeneratedNever()
                .HasColumnName("Id_Manufacturer");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.IdOrder);

            entity.ToTable("Order");

            entity.Property(e => e.IdOrder)
                .ValueGeneratedNever()
                .HasColumnName("Id_Order");
            entity.Property(e => e.DeliveryDate).HasColumnName("Delivery_Date");
            entity.Property(e => e.IdAdress).HasColumnName("Id_Adress");
            entity.Property(e => e.IdPerson).HasColumnName("Id_Person");
            entity.Property(e => e.OrderDate).HasColumnName("Order_Date");
            entity.Property(e => e.OrderStatus)
                .HasMaxLength(255)
                .HasColumnName("Order_Status");

            entity.HasOne(d => d.IdAdressNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.IdAdress)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Adress");

            entity.HasOne(d => d.IdPersonNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.IdPerson)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Person");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.IdOrderItem);

            entity.ToTable("Order_Item");

            entity.Property(e => e.IdOrderItem)
                .ValueGeneratedNever()
                .HasColumnName("Id_Order_Item");
            entity.Property(e => e.IdOrder).HasColumnName("Id_Order");
            entity.Property(e => e.IdProduct).HasColumnName("Id_Product");

            entity.HasOne(d => d.IdOrderNavigation).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.IdOrder)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Item_Order");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.IdProduct)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Item_Product");
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.IdPerson);

            entity.ToTable("Person");

            entity.Property(e => e.IdPerson)
                .ValueGeneratedNever()
                .HasColumnName("Id_Person");
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .HasColumnName("First_Name");
            entity.Property(e => e.Patronymic).HasMaxLength(255);
            entity.Property(e => e.Surname).HasMaxLength(255);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.IdProduct);

            entity.ToTable("Product");

            entity.Property(e => e.IdProduct)
                .ValueGeneratedNever()
                .HasColumnName("ID_Product");
            entity.Property(e => e.Artikul).HasMaxLength(255);
            entity.Property(e => e.CountInStock).HasColumnName("Count_In_Stock");
            entity.Property(e => e.CurrentDiscount)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("Current_Discount");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IdCategory).HasColumnName("Id_Category");
            entity.Property(e => e.IdManufacturer).HasColumnName("Id_Manufacturer");
            entity.Property(e => e.IdSupplier).HasColumnName("Id_Supplier");
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Price).HasColumnType("money");
            entity.Property(e => e.UnitOfMeasurement)
                .HasMaxLength(255)
                .HasColumnName("Unit_Of_Measurement");

            entity.HasOne(d => d.IdCategoryNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.IdCategory)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Caterory");

            entity.HasOne(d => d.IdManufacturerNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.IdManufacturer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Manufacturer");

            entity.HasOne(d => d.IdSupplierNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.IdSupplier)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Supplier");
        });

        modelBuilder.Entity<Sity>(entity =>
        {
            entity.HasKey(e => e.IdSity);

            entity.ToTable("Sity");

            entity.Property(e => e.IdSity)
                .ValueGeneratedNever()
                .HasColumnName("Id_Sity");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.IdSupplier);

            entity.ToTable("Supplier");

            entity.Property(e => e.IdSupplier)
                .ValueGeneratedNever()
                .HasColumnName("Id_Supplier");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser);

            entity.ToTable("User");

            entity.Property(e => e.IdUser)
                .ValueGeneratedNever()
                .HasColumnName("Id_User");
            entity.Property(e => e.IdPerson).HasColumnName("Id_Person");
            entity.Property(e => e.IdUserRole).HasColumnName("Id_User_Role");
            entity.Property(e => e.Login).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);

            entity.HasOne(d => d.IdPersonNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdPerson)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Person");

            entity.HasOne(d => d.IdUserRoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdUserRole)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_User_Role");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.IdUserRole);

            entity.ToTable("User_Role");

            entity.Property(e => e.IdUserRole)
                .ValueGeneratedNever()
                .HasColumnName("Id_User_Role");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
