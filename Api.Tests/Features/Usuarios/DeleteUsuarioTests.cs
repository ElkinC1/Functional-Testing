using Api.Application.Features.Usuarios.DeleteUsuario;
using Api.Tests.Factories;
using Api.Tests.Infrastructure;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Api.Tests.Features.Usuarios;

public class DeleteUsuarioTests : TestBase
{
    [Test]
    public async Task ShouldSoftDeleteUsuario_WhenIdExists()
    {
        // Arrange
        var usuario = UsuarioFactory.CreateFakeEntity();
        DbContext.Usuarios.Add(usuario);
        await DbContext.SaveChangesAsync();

        var command = new DeleteUsuarioCommand(usuario.Id);

        // Act
        await Mediator.Send(command);

        // Assert
        // 1. The user should not be accessible in standard queries because of the global HasQueryFilter
        var standardFetch = await DbContext.Usuarios.FirstOrDefaultAsync(u => u.Id == usuario.Id);
        standardFetch.ShouldBeNull();

        // 2. The user should still exist physically in the DB, but with IsDeleted = true
        var directFetch = await DbContext.Usuarios
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == usuario.Id);

        directFetch.ShouldNotBeNull();
        directFetch.IsDeleted.ShouldBeTrue();
    }

    [Test]
    public async Task ShouldThrowKeyNotFoundException_WhenIdDoesNotExist()
    {
        // Arrange
        var command = new DeleteUsuarioCommand(Guid.NewGuid());

        // Act & Assert
        await Should.ThrowAsync<KeyNotFoundException>(async () => await Mediator.Send(command));
    }

    [Test]
    public async Task ShouldThrowValidationException_WhenIdIsEmpty()
    {
        // Arrange
        var command = new DeleteUsuarioCommand(Guid.Empty);

        // Act & Assert
        var exception = await Should.ThrowAsync<ValidationException>(async () => await Mediator.Send(command));
        exception.Errors.ShouldContain(x => x.PropertyName == nameof(DeleteUsuarioCommand.Id));
    }
}
