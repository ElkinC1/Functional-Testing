using Api.Domain.Entities;
using Api.Domain.ValueObjects;
using Bogus;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Aseguramos que la base de datos existe
        await context.Database.EnsureCreatedAsync();

        // Si ya hay usuarios en la base de datos, no hacemos nada
        if (await context.Usuarios.AnyAsync())
        {
            return;
        }

        // Configurar Bogus para generar usuarios en español
        var faker = new Faker<Usuario>("es")
            .CustomInstantiator(f =>
            {
                var nombre = f.Name.FirstName();
                var apellido = f.Name.LastName();
                // Generamos un correo basado en el nombre y apellido
                var emailStr = f.Internet.Email(nombre, apellido);
                
                return new Usuario(
                    Guid.NewGuid(),
                    nombre,
                    apellido,
                    Email.From(emailStr)
                );
            });

        var usuarios = faker.Generate(50);

        context.Usuarios.AddRange(usuarios);
        await context.SaveChangesAsync();
    }
}
