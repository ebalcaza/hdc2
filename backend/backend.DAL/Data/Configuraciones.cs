using backend.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.DAL.Data;

// Las tablas catálogo (ROL, TIPO_EMISION, ...) se cargan a mano: sus ids no se generan.

public class RolConfig : IEntityTypeConfiguration<Rol>
{
    public void Configure(EntityTypeBuilder<Rol> b)
    {
        b.ToTable("ROL");
        b.HasKey(x => x.IdRol);
        b.Property(x => x.IdRol).HasColumnName("ID_ROL").ValueGeneratedNever();
        b.Property(x => x.Nombre).HasColumnName("NOMBRE").HasMaxLength(50).IsUnicode(false).IsRequired();
    }
}

public class PermisoConfig : IEntityTypeConfiguration<Permiso>
{
    public void Configure(EntityTypeBuilder<Permiso> b)
    {
        b.ToTable("PERMISOS");
        b.HasKey(x => x.IdPermiso);
        b.Property(x => x.IdPermiso).HasColumnName("ID_PERMISO").ValueGeneratedNever();
        b.Property(x => x.IdRol).HasColumnName("ID_ROL");
        b.Property(x => x.Nombre).HasColumnName("NOMBRE").HasMaxLength(150).IsUnicode(false).IsRequired();
        b.HasOne(x => x.Rol).WithMany(r => r.Permisos).HasForeignKey(x => x.IdRol).OnDelete(DeleteBehavior.Restrict);
    }
}

public class TipoEmisionConfig : IEntityTypeConfiguration<TipoEmision>
{
    public void Configure(EntityTypeBuilder<TipoEmision> b)
    {
        b.ToTable("TIPO_EMISION");
        b.HasKey(x => x.IdTipoEmision);
        b.Property(x => x.IdTipoEmision).HasColumnName("ID_TIPO_EMISION").ValueGeneratedNever();
        b.Property(x => x.Nombre).HasColumnName("NOMBRE").HasMaxLength(20).IsUnicode(false).IsRequired();
    }
}

public class UnidadMedidaConfig : IEntityTypeConfiguration<UnidadMedida>
{
    public void Configure(EntityTypeBuilder<UnidadMedida> b)
    {
        b.ToTable("UNIDAD_MEDIDA");
        b.HasKey(x => x.IdUnidad);
        b.Property(x => x.IdUnidad).HasColumnName("ID_UNIDAD").ValueGeneratedNever();
        b.Property(x => x.Nombre).HasColumnName("NOMBRE").HasMaxLength(20).IsUnicode(false).IsRequired();
    }
}

public class AlcanceConfig : IEntityTypeConfiguration<Alcance>
{
    public void Configure(EntityTypeBuilder<Alcance> b)
    {
        b.ToTable("ALCANCE");
        b.HasKey(x => x.IdAlcance);
        b.Property(x => x.IdAlcance).HasColumnName("ID_ALCANCE").ValueGeneratedNever();
        b.Property(x => x.Nombre).HasColumnName("NOMBRE").HasMaxLength(50).IsUnicode(false).IsRequired();
    }
}

public class UsuarioConfig : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> b)
    {
        b.ToTable("USUARIOS");
        b.HasKey(x => x.IdUsuario);
        b.Property(x => x.IdUsuario).HasColumnName("ID_USUARIO")
            .ValueGeneratedOnAdd().HasDefaultValueSql("NEXT VALUE FOR dbo.SEQ_USUARIOS");
        b.Property(x => x.IdRol).HasColumnName("ID_ROL");
        b.Property(x => x.Email).HasColumnName("EMAIL").HasMaxLength(100).IsUnicode(false).IsRequired();
        b.Property(x => x.Contrasena).HasColumnName("CONTRASENA").HasMaxLength(250).IsUnicode(false).IsRequired();
        b.Property(x => x.Nombre).HasColumnName("NOMBRE").HasMaxLength(100).IsUnicode(false).IsRequired();
        b.Property(x => x.Apellido).HasColumnName("APELLIDO").HasMaxLength(200).IsUnicode(false);
        b.Property(x => x.Telefono).HasColumnName("TELEFONO").HasMaxLength(12).IsUnicode(false);
        b.Property(x => x.FechaNacimiento).HasColumnName("FECHA_NACIMIENTO").HasColumnType("smalldatetime");
        b.Property(x => x.FechaAlta).HasColumnName("FECHA_ALTA").HasColumnType("datetime");
        b.Property(x => x.FechaBaja).HasColumnName("FECHA_BAJA").HasColumnType("datetime");
        b.Property(x => x.AutorUltimaModificacion).HasColumnName("AUTOR_ULTIMA_MODIFICACION");
        b.Property(x => x.FechaUltimaModificacion).HasColumnName("FECHA_ULTIMA_MODIFICACION").HasColumnType("datetime");

        b.HasIndex(x => x.Email).IsUnique();
        b.HasOne(x => x.Rol).WithMany(r => r.Usuarios).HasForeignKey(x => x.IdRol).OnDelete(DeleteBehavior.Restrict);
    }
}

public class OrganizacionConfig : IEntityTypeConfiguration<Organizacion>
{
    public void Configure(EntityTypeBuilder<Organizacion> b)
    {
        b.ToTable("ORGANIZACION");
        b.HasKey(x => x.IdUsuario);
        b.Property(x => x.IdUsuario).HasColumnName("ID_USUARIO").ValueGeneratedNever();
        b.Property(x => x.RazonSocial).HasColumnName("RAZON_SOCIAL").HasMaxLength(150).IsUnicode(false).IsRequired();
        b.Property(x => x.Cuit).HasColumnName("CUIT").HasMaxLength(20).IsUnicode(false).IsRequired();
        b.Property(x => x.Area).HasColumnName("AREA").HasMaxLength(100).IsUnicode(false).IsRequired();
        b.Property(x => x.Descripcion).HasColumnName("DESCRIPCION").HasMaxLength(500).IsUnicode(false);
        b.Property(x => x.CantidadMiembros).HasColumnName("CANTIDAD_MIEMBROS");

        b.HasOne(x => x.Usuario).WithOne(u => u.Organizacion)
            .HasForeignKey<Organizacion>(x => x.IdUsuario).OnDelete(DeleteBehavior.Restrict);
    }
}

public class IndividuoConfig : IEntityTypeConfiguration<Individuo>
{
    public void Configure(EntityTypeBuilder<Individuo> b)
    {
        b.ToTable("INDIVIDUO");
        b.HasKey(x => x.IdUsuario);
        b.Property(x => x.IdUsuario).HasColumnName("ID_USUARIO").ValueGeneratedNever();
        b.Property(x => x.IdOrganizacion).HasColumnName("ID_ORGANIZACION");
        b.Property(x => x.Dni).HasColumnName("DNI").HasMaxLength(15).IsUnicode(false);
        b.Property(x => x.Genero).HasColumnName("GENERO").HasMaxLength(20).IsUnicode(false);

        b.HasOne(x => x.Usuario).WithOne(u => u.Individuo)
            .HasForeignKey<Individuo>(x => x.IdUsuario).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Organizacion).WithMany(o => o.Miembros)
            .HasForeignKey(x => x.IdOrganizacion).OnDelete(DeleteBehavior.Restrict);
    }
}

public class DomicilioConfig : IEntityTypeConfiguration<Domicilio>
{
    public void Configure(EntityTypeBuilder<Domicilio> b)
    {
        b.ToTable("DOMICILIOS");
        b.HasKey(x => x.IdDomicilio);
        b.Property(x => x.IdDomicilio).HasColumnName("ID_DOMICILIO")
            .ValueGeneratedOnAdd().HasDefaultValueSql("NEXT VALUE FOR dbo.SEQ_DOMICILIOS");
        b.Property(x => x.IdUsuario).HasColumnName("ID_USUARIO");
        b.Property(x => x.Calle).HasColumnName("CALLE").HasMaxLength(100).IsUnicode(false);
        b.Property(x => x.Numero).HasColumnName("NUMERO").HasMaxLength(20).IsUnicode(false);
        b.Property(x => x.Ciudad).HasColumnName("CIUDAD").HasMaxLength(200).IsUnicode(false);
        b.Property(x => x.Provincia).HasColumnName("PROVINCIA").HasMaxLength(100).IsUnicode(false);
        b.Property(x => x.Distrito).HasColumnName("DISTRITO").HasMaxLength(150).IsUnicode(false);
        b.Property(x => x.Departamento).HasColumnName("DEPARTAMENTO").HasMaxLength(20).IsUnicode(false);
        b.Property(x => x.Piso).HasColumnName("PISO").HasMaxLength(20).IsUnicode(false);
        b.Property(x => x.Block).HasColumnName("BLOCK").HasMaxLength(20).IsUnicode(false);
        b.Property(x => x.Manzana).HasColumnName("MANZANA").HasMaxLength(20).IsUnicode(false);
        b.Property(x => x.CodigoPostal).HasColumnName("CODIGO_POSTAL").HasMaxLength(10).IsUnicode(false);
        b.Property(x => x.Telefono).HasColumnName("TELEFONO").HasMaxLength(20).IsUnicode(false);
        b.Property(x => x.FechaAlta).HasColumnName("FECHA_ALTA").HasColumnType("datetime");
        b.Property(x => x.FechaBaja).HasColumnName("FECHA_BAJA").HasColumnType("datetime");
        b.Property(x => x.FechaModificacion).HasColumnName("FECHA_MODIFICACION").HasColumnType("datetime");
        b.Property(x => x.AutorModificacion).HasColumnName("AUTOR_MODIFICACION");

        b.HasOne(x => x.Usuario).WithMany(u => u.Domicilios).HasForeignKey(x => x.IdUsuario).OnDelete(DeleteBehavior.Restrict);
    }
}

public class FuenteEmisionConfig : IEntityTypeConfiguration<FuenteEmision>
{
    public void Configure(EntityTypeBuilder<FuenteEmision> b)
    {
        b.ToTable("FUENTE_EMISION");
        b.HasKey(x => x.IdFuente);
        b.Property(x => x.IdFuente).HasColumnName("ID_FUENTE")
            .ValueGeneratedOnAdd().HasDefaultValueSql("NEXT VALUE FOR dbo.SEQ_FUENTES");
        b.Property(x => x.IdTipoEmision).HasColumnName("ID_TIPO_EMISION");
        b.Property(x => x.IdUnidad).HasColumnName("ID_UNIDAD");
        b.Property(x => x.IdAlcance).HasColumnName("ID_ALCANCE");
        b.Property(x => x.Nombre).HasColumnName("NOMBRE").HasMaxLength(100).IsUnicode(false).IsRequired();
        b.Property(x => x.ValorFactorEmision).HasColumnName("VALOR_FACTOR_EMISION").HasPrecision(18, 6);

        b.HasOne(x => x.TipoEmision).WithMany().HasForeignKey(x => x.IdTipoEmision).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Unidad).WithMany().HasForeignKey(x => x.IdUnidad).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Alcance).WithMany().HasForeignKey(x => x.IdAlcance).OnDelete(DeleteBehavior.Restrict);
    }
}

public class ReporteConfig : IEntityTypeConfiguration<Reporte>
{
    public void Configure(EntityTypeBuilder<Reporte> b)
    {
        b.ToTable("REPORTE");
        b.HasKey(x => x.IdReporte);
        b.Property(x => x.IdReporte).HasColumnName("ID_REPORTE").ValueGeneratedOnAdd();      // IDENTITY
        b.Property(x => x.IdUsuario).HasColumnName("ID_USUARIO");
        b.Property(x => x.FechaCreacion).HasColumnName("FECHA_CREACION").HasColumnType("datetime");
        b.Property(x => x.FechaModificacion).HasColumnName("FECHA_MODIFICACION").HasColumnType("datetime");
        b.Property(x => x.FechaBaja).HasColumnName("FECHA_BAJA").HasColumnType("datetime");
        b.Property(x => x.TotalCo2Kg).HasColumnName("TOTAL_CO2_KG").HasPrecision(18, 4);

        b.HasOne(x => x.Usuario).WithMany(u => u.Reportes).HasForeignKey(x => x.IdUsuario).OnDelete(DeleteBehavior.Restrict);
    }
}

public class RegistroConfig : IEntityTypeConfiguration<Registro>
{
    public void Configure(EntityTypeBuilder<Registro> b)
    {
        b.ToTable("REGISTROS");
        b.HasKey(x => x.IdRegistro);
        b.Property(x => x.IdRegistro).HasColumnName("ID_REGISTRO")
            .ValueGeneratedOnAdd().HasDefaultValueSql("NEXT VALUE FOR dbo.SEQ_REGISTROS");
        b.Property(x => x.IdReporte).HasColumnName("ID_REPORTE");
        b.Property(x => x.IdDomicilio).HasColumnName("ID_DOMICILIO");
        b.Property(x => x.IdFuente).HasColumnName("ID_FUENTE");
        b.Property(x => x.Valor).HasColumnName("VALOR").HasPrecision(18, 4);

        b.HasOne(x => x.Reporte).WithMany(r => r.Registros).HasForeignKey(x => x.IdReporte).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Domicilio).WithMany().HasForeignKey(x => x.IdDomicilio).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Fuente).WithMany().HasForeignKey(x => x.IdFuente).OnDelete(DeleteBehavior.Restrict);
    }
}
