using Api.Application.Features.Usuarios.CreateUsuario;
using Api.Domain.Entities;
using Api.Domain.ValueObjects;
using Bogus;

namespace Api.Tests.Factories;

public static class UsuarioFactory
{
    public static CreateUsuarioCommand CreateFakeCommand()
    {
        var faker = new Faker<CreateUsuarioCommand>().CustomInstantiator(
            f => new CreateUsuarioCommand(f.Name.FirstName(), f.Name.LastName(), f.Internet.Email())
        );

        return faker.Generate();
    }

    public static CreateUsuarioCommand CreateInvalidCommand()
    {
        var faker = new Faker<CreateUsuarioCommand>().CustomInstantiator(
            f => new CreateUsuarioCommand("", f.Name.LastName(), "correo-invalido")
        );

        return faker.Generate();
    }

    public static Usuario CreateFakeEntity()
    {
        var faker = new Faker();
        return new Usuario(
            Guid.NewGuid(),
            faker.Name.FirstName(),
            faker.Name.LastName(),
            Email.From(faker.Internet.Email())
        );
    }
}
