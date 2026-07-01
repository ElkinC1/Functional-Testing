using Api.Application.Features.Usuarios.CreateUsuario;
using Api.Tests.Factories;
using Api.Tests.Infrastructure;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Api.Tests.Features.Usuarios;

public class CreateUsuarioTests : TestBase
{
    [Test]
    public async Task ShouldCreateUsuario_WhenCommandIsValid()
    {
        // Arrange
        var command = UsuarioFactory.CreateFakeCommand();

        // Act
        var id = await Mediator.Send(command);

        // Assert
        id.ShouldNotBe(Guid.Empty);

        var usuario = await DbContext.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
        usuario.ShouldNotBeNull();
        usuario.Nombre.ShouldBe(command.Nombre);
        usuario.Apellido.ShouldBe(command.Apellido);
        usuario.Email.Value.ShouldBe(command.Email);
    }

    [Test]
    public async Task ShouldThrowValidationException_WhenNombreIsEmpty()
    {
        // Arrange
        var command = new CreateUsuarioCommand("", "Perez", "juan.perez@example.com");

        // Act & Assert
        var exception = await Should.ThrowAsync<ValidationException>(async () =>
            await Mediator.Send(command)
        );
        exception.Errors.ShouldContain(x => x.PropertyName == nameof(CreateUsuarioCommand.Nombre));
    }

    [Test]
    public async Task ShouldThrowValidationException_WhenNombreIsTooLong()
    {
        // Arrange
        var longName = new string('a', 101);
        var command = new CreateUsuarioCommand(longName, "Perez", "juan.perez@example.com");

        // Act & Assert
        var exception = await Should.ThrowAsync<ValidationException>(async () =>
            await Mediator.Send(command)
        );
        exception.Errors.ShouldContain(x => x.PropertyName == nameof(CreateUsuarioCommand.Nombre));
    }

    [Test]
    public async Task ShouldThrowValidationException_WhenApellidoIsEmpty()
    {
        // Arrange
        var command = new CreateUsuarioCommand("Juan", "", "juan.perez@example.com");

        // Act & Assert
        var exception = await Should.ThrowAsync<ValidationException>(async () =>
            await Mediator.Send(command)
        );
        exception.Errors.ShouldContain(x =>
            x.PropertyName == nameof(CreateUsuarioCommand.Apellido)
        );
    }

    [Test]
    public async Task ShouldThrowValidationException_WhenApellidoIsTooLong()
    {
        // Arrange
        var longLastName = new string('b', 101);
        var command = new CreateUsuarioCommand("Juan", longLastName, "juan.perez@example.com");

        // Act & Assert
        var exception = await Should.ThrowAsync<ValidationException>(async () =>
            await Mediator.Send(command)
        );
        exception.Errors.ShouldContain(x =>
            x.PropertyName == nameof(CreateUsuarioCommand.Apellido)
        );
    }

    [Test]
    public async Task ShouldThrowValidationException_WhenEmailIsEmpty()
    {
        // Arrange
        var command = new CreateUsuarioCommand("Juan", "Perez", "");

        // Act & Assert
        var exception = await Should.ThrowAsync<ValidationException>(async () =>
            await Mediator.Send(command)
        );
        exception.Errors.ShouldContain(x => x.PropertyName == nameof(CreateUsuarioCommand.Email));
    }

    [Test]
    [TestCase("invalidemail")]
    [TestCase("invalid@email")]
    [TestCase("invalid@.com")]
    [TestCase("invalid @email.com")]
    public async Task ShouldThrowValidationException_WhenEmailIsInvalidFormat(string invalidEmail)
    {
        // Arrange
        var command = new CreateUsuarioCommand("Juan", "Perez", invalidEmail);

        // Act & Assert
        var exception = await Should.ThrowAsync<ValidationException>(async () =>
            await Mediator.Send(command)
        );
        exception.Errors.ShouldContain(x => x.PropertyName == nameof(CreateUsuarioCommand.Email));
    }
}
