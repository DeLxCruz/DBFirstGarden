using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;

namespace Persistence.Data.Config
{
    public class DetallerPedidoConfig : IEntityTypeConfiguration<DetallePedido>
    {
        public void Configure(EntityTypeBuilder<DetallePedido> builder)
        {
            builder.HasKey(e => new { e.Id, e.CodigoProducto })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            builder.ToTable("detalle_pedido");

            builder.HasIndex(e => e.CodigoProducto, "codigo_producto");

            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.CodigoProducto)
                .HasMaxLength(15)
                .HasColumnName("codigo_producto");
            builder.Property(e => e.Cantidad).HasColumnName("cantidad");
            builder.Property(e => e.NumeroLinea).HasColumnName("numero_linea");
            builder.Property(e => e.PrecioUnidad)
                .HasPrecision(15, 2)
                .HasColumnName("precio_unidad");

            builder.HasOne(d => d.CodigoProductoNavigation).WithMany(p => p.DetallePedidos)
                .HasForeignKey(d => d.CodigoProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("detalle_pedido_ibfk_2");

            builder.HasOne(d => d.IdNavigation).WithMany(p => p.DetallePedidos)
                .HasForeignKey(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("detalle_pedido_ibfk_1");
        }
    }
}