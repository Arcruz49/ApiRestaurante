using System;
using System.Collections.Generic;
using APITeste.Models;
using Microsoft.EntityFrameworkCore;

namespace APITeste.Data;

public partial class DbRestauranteContext : DbContext
{
    public DbRestauranteContext()
    {
    }

    public DbRestauranteContext(DbContextOptions<DbRestauranteContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CadCardapio> CadCardapios { get; set; }

    public virtual DbSet<CadCliente> CadClientes { get; set; }

    public virtual DbSet<CadMesa> CadMesas { get; set; }

    public virtual DbSet<CadPedido> CadPedidos { get; set; }

    public virtual DbSet<CadPedidoPrato> CadPedidoPratos { get; set; }

    public virtual DbSet<CadReserva> CadReservas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=dbRestaurante;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CadCardapio>(entity =>
        {
            entity.HasKey(e => e.CdPrato).HasName("PK__cadCarda__DB888E17A1F0BE1A");

            entity.ToTable("cadCardapio");

            entity.Property(e => e.CdPrato).HasColumnName("cdPrato");
            entity.Property(e => e.DsPrato)
                .IsUnicode(false)
                .HasColumnName("dsPrato");
            entity.Property(e => e.DtCriacao)
                .HasColumnType("datetime")
                .HasColumnName("dtCriacao");
            entity.Property(e => e.NmPrato)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("nmPrato");
            entity.Property(e => e.Preco)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("preco");
        });

        modelBuilder.Entity<CadCliente>(entity =>
        {
            entity.HasKey(e => e.CdCliente).HasName("PK__cadClien__64864EC6D239D164");

            entity.ToTable("cadClientes");

            entity.Property(e => e.CdCliente).HasColumnName("cdCliente");
            entity.Property(e => e.DtCriacao)
                .HasColumnType("datetime")
                .HasColumnName("dtCriacao");
            entity.Property(e => e.NmCliente)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("nmCliente");
        });

        modelBuilder.Entity<CadMesa>(entity =>
        {
            entity.HasKey(e => e.CdMesa).HasName("PK__cadMesas__3D880CF5F23D9DCA");

            entity.ToTable("cadMesas");

            entity.Property(e => e.CdMesa).HasColumnName("cdMesa");
            entity.Property(e => e.DtCriacao)
                .HasColumnType("datetime")
                .HasColumnName("dtCriacao");
        });

        modelBuilder.Entity<CadPedido>(entity =>
        {
            entity.HasKey(e => e.CdPedido).HasName("PK__cadPedid__A65201CD647AF245");

            entity.ToTable("cadPedido");

            entity.Property(e => e.CdPedido).HasColumnName("cdPedido");
            entity.Property(e => e.CdCliente).HasColumnName("cdCliente");
            entity.Property(e => e.DtCriacao)
                .HasColumnType("datetime")
                .HasColumnName("dtCriacao");

            entity.HasOne(d => d.CdClienteNavigation).WithMany(p => p.CadPedidos)
                .HasForeignKey(d => d.CdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_cadPedido_cdCliente");
        });

        modelBuilder.Entity<CadPedidoPrato>(entity =>
        {
            entity.HasKey(e => e.CdPedidoPrato).HasName("PK__cadPedid__F16383FD72F298D7");

            entity.ToTable("cadPedidoPratos");

            entity.Property(e => e.CdPedidoPrato).HasColumnName("cdPedidoPrato");
            entity.Property(e => e.CdPedido).HasColumnName("cdPedido");
            entity.Property(e => e.CdPrato).HasColumnName("cdPrato");
            entity.Property(e => e.Quantidade)
                .HasDefaultValue(1)
                .HasColumnName("quantidade");

            entity.HasOne(d => d.CdPedidoNavigation).WithMany(p => p.CadPedidoPratos)
                .HasForeignKey(d => d.CdPedido)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_cadPedidoPratos_cdPedido");

            entity.HasOne(d => d.CdPratoNavigation).WithMany(p => p.CadPedidoPratos)
                .HasForeignKey(d => d.CdPrato)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_cadPedidoPratos_cdPrato");
        });

        modelBuilder.Entity<CadReserva>(entity =>
        {
            entity.HasKey(e => e.CdReserva).HasName("PK__cadReser__D7F8F80E17C4AB1D");

            entity.ToTable("cadReserva");

            entity.Property(e => e.CdReserva).HasColumnName("cdReserva");
            entity.Property(e => e.CdCliente).HasColumnName("cdCliente");
            entity.Property(e => e.CdMesa).HasColumnName("cdMesa");
            entity.Property(e => e.DtCriacao)
                .HasColumnType("datetime")
                .HasColumnName("dtCriacao");
            entity.Property(e => e.NmCliente)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("nmCliente");

            entity.HasOne(d => d.CdClienteNavigation).WithMany(p => p.CadReservas)
                .HasForeignKey(d => d.CdCliente)
                .HasConstraintName("FK_cadReserva_cdCliente");

            entity.HasOne(d => d.CdMesaNavigation).WithMany(p => p.CadReservas)
                .HasForeignKey(d => d.CdMesa)
                .HasConstraintName("FK_cadReserva_cdMesa");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
