using Api.Application.Features.Usuarios.UpdateUsuario;
using Api.Tests.Factories;
using Api.Tests.Infrastructure;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Api.Tests.Features.Usuarios;

public class UpdateUsuarioTests : TestBase
{
    [Test]
    public async Task ShouldUpdateUsuario_WhenCommandIsValid()
    {
        // Arrange
        var usuario = UsuarioFactory.CreateFakeEntity();
        DbContext.Usuarios.Add(usuario);
        await DbContext.SaveChangesAsync();

        var command = new UpdateUsuarioCommand(
            usuario.Id,
            "NuevoNombre",
            "NuevoApellido",
            "nuevo.email@example.com"
        );

        // Act
        await Mediator.Send(command);

        // Assert
        var updatedUsuario = await DbContext.Usuarios.FirstOrDefaultAsync(u => u.Id == usuario.Id);
        updatedUsuario.ShouldNotBeNull();
        updatedUsuario.Nombre.ShouldBe("NuevoNombre");
        updatedUsuario.Apellido.ShouldBe("NuevoApellido");
        updatedUsuario.Email.Value.ShouldBe("nuevo.email@example.com");
    }

    [Test]
    public async Task ShouldThrowKeyNotFoundException_WhenIdDoesNotExist()
    {
        // Arrange
        var command = new UpdateUsuarioCommand(
            Guid.NewGuid(),
            "Nombre",
            "Apellido",
            "test@example.com"
        );

        // Act & Assert
        await Should.ThrowAsync<KeyNotFoundException>(async () => await Mediator.Send(command));
    }

    [Test]
    public async Task ShouldThrowValidationException_WhenNombreIsEmpty()
    {
        // Arrange
        var command = new UpdateUsuarioCommand(Guid.NewGuid(), "", "Apellido", "test@example.com");

        // Act & Assert
        var exception = await Should.ThrowAsync<ValidationException>(async () => await Mediator.Send(command));
        exception.Errors.ShouldContain(x => x.PropertyName == nameof(UpdateUsuarioCommand.Nombre));
    }

    [Test]
    public async Task ShouldThrowValidationException_WhenNombreIsTooLong()
    {
        // Arrange
        var longName = new string('a', 101);
        var command = new UpdateUsuarioCommand(Guid.NewGuid(), longName, "Apellido", "test@example.com");

        // Act & Assert
        var exception = await Should.ThrowAsync<ValidationException>(async () => await Mediator.Send(command));
        exception.Errors.ShouldContain(x => x.PropertyName == nameof(UpdateUsuarioCommand.Nombre));
    }

    [Test]
    public async Task ShouldThrowValidationException_WhenApellidoIsEmpty()
    {
        // Arrange
        var command = new UpdateUsuarioCommand(Guid.NewGuid(), "Nombre", "", "test@example.com");

        // Act & Assert
        var exception = await Should.ThrowAsync<ValidationException>(async () => await Mediator.Send(command));
        exception.Errors.ShouldContain(x => x.PropertyName == nameof(UpdateUsuarioCommand.Apellido));
    }

    [Test]
    public async Task ShouldThrowValidationException_WhenApellidoIsTooLong()
    {
        // Arrange
        var longLastName = new string('b', 101);
        var command = new UpdateUsuarioCommand(Guid.NewGuid(), "Nombre", longLastName, "test@example.com");

        // Act & Assert
        var exception = await Should.ThrowAsync<ValidationException>(async () => await Mediator.Send(command));
        exception.Errors.ShouldContain(x => x.PropertyName == nameof(UpdateUsuarioCommand.Apellido));
    }

    [Test]
    [TestCase("invalidemail")]
    [TestCase("invalid@.com")]
    public async Task ShouldThrowValidationException_WhenEmailIsInvalidFormat(string invalidEmail)
    {
        // Arrange
        var command = new UpdateUsuarioCommand(Guid.NewGuid(), "Nombre", "Apellido", invalidEmail);

        // Act & Assert
        var exception = await Should.ThrowAsync<ValidationException>(async () => await Mediator.Send(command));
        exception.Errors.ShouldContain(x => x.PropertyName == nameof(UpdateUsuarioCommand.Email));
    }
}
