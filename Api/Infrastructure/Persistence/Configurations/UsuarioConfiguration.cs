using Api.Domain.Entities;
using Api.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Infrastructure.Persistence.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Nombre).IsRequired().HasMaxLength(100);

        builder.Property(u => u.Apellido).IsRequired().HasMaxLength(100);

        builder
            .Property(u => u.Email)
            .HasConversion(email => email.Value, value => Email.From(value))
            .HasColumnName("Email")
            .IsRequired()
            .HasMaxLength(255);
    }
}
