using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using SICProject.Models;

namespace SICProject.Models;

public partial class SicdbContext : DbContext
{
    public SicdbContext()
    {
    }

    public SicdbContext(DbContextOptions<SicdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Departmentmaster> Departmentmasters { get; set; }

    public virtual DbSet<Instrumentsmaster> Instrumentsmasters { get; set; }

    public virtual DbSet<Registrationmaster> Registrationmasters { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Departmentmaster>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("PRIMARY");

            entity.ToTable("departmentmaster");

            entity.Property(e => e.DepartmentId).HasColumnType("int(11)");
            entity.Property(e => e.DepartmentName).HasMaxLength(250);
            entity.Property(e => e.Remarks).HasMaxLength(250);
        });

        modelBuilder.Entity<Instrumentsmaster>(entity =>
        {
            entity.HasKey(e => e.InstrumentsId).HasName("PRIMARY");

            entity.ToTable("instrumentsmaster");

            entity.Property(e => e.InstrumentsId)
                .HasColumnType("int(11)")
                .HasColumnName("instrumentsId");
            entity.Property(e => e.InstrumentName)
                .HasMaxLength(250)
                .HasColumnName("instrumentName");
            entity.Property(e => e.Remarks)
                .HasMaxLength(250)
                .HasColumnName("remarks");
        });

        modelBuilder.Entity<Registrationmaster>(entity =>
        {
            entity.HasKey(e => e.RegistrationId).HasName("PRIMARY");

            entity.ToTable("registrationmaster");

            entity.Property(e => e.RegistrationId).HasColumnType("bigint(20)");
            entity.Property(e => e.DepartmentId).HasColumnType("int(11)");
            entity.Property(e => e.DepartmentName).HasMaxLength(250);
            entity.Property(e => e.Email).HasMaxLength(250);
            entity.Property(e => e.IsApproved).HasColumnType("bit(1)");
            entity.Property(e => e.MobileNumber).HasMaxLength(20);
            entity.Property(e => e.Password).HasMaxLength(45);
            entity.Property(e => e.Remarks).HasMaxLength(250);
            entity.Property(e => e.StudentName).HasMaxLength(250);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

public DbSet<SICProject.Models.DepartmentmasterVM> DepartmentmasterVM { get; set; } = default!;

public DbSet<SICProject.Models.RegistrationmasterVM> RegistrationmasterVM { get; set; } = default!;

public DbSet<SICProject.Models.loginVM> loginVM { get; set; } = default!;
}
