using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entities;

namespace Persistence.Data.Config
{
    public class EmpleadoConfig : IEntityTypeConfiguration<Empleado>
    {
        public void Configure(EntityTypeBuilder<Empleado> builder)
        {
            builder.HasKey(e => e.Id).HasName("PRIMARY");

            builder.ToTable("empleado");

            builder.HasIndex(e => e.CodigoJefe, "codigo_jefe");

            builder.HasIndex(e => e.CodigoOficina, "codigo_oficina");

            builder.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            builder.Property(e => e.Apellido1)
                .HasMaxLength(50)
                .HasColumnName("apellido1");
            builder.Property(e => e.Apellido2)
                .HasMaxLength(50)
                .HasColumnName("apellido2");
            builder.Property(e => e.CodigoJefe).HasColumnName("codigo_jefe");
            builder.Property(e => e.CodigoOficina)
                .HasMaxLength(10)
                .HasColumnName("codigo_oficina");
            builder.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            builder.Property(e => e.Extension)
                .HasMaxLength(10)
                .HasColumnName("extension");
            builder.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            builder.Property(e => e.Puesto)
                .HasMaxLength(50)
                .HasColumnName("puesto");

            builder.HasOne(d => d.CodigoJefeNavigation).WithMany(p => p.InverseCodigoJefeNavigation)
                .HasForeignKey(d => d.CodigoJefe)
                .HasConstraintName("empleado_ibfk_2");

            builder.HasOne(d => d.CodigoOficinaNavigation).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.CodigoOficina)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("empleado_ibfk_1");
        }
    }
}