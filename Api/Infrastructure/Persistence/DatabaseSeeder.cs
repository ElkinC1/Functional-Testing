using Api.Domain.Entities;
using Api.Domain.ValueObjects;
using Bogus;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (await context.Usuarios.AnyAsync())
        {
            return;
        }

        var faker = new Faker<Usuario>("es").CustomInstantiator(f =>
        {
            var nombre = f.Name.FirstName();
            var apellido = f.Name.LastName();
            var emailStr = f.Internet.Email(nombre, apellido);

            return new Usuario(Guid.NewGuid(), nombre, apellido, Email.From(emailStr));
        });

        var usuarios = faker.Generate(50);

        context.Usuarios.AddRange(usuarios);
        await context.SaveChangesAsync();
    }
}
