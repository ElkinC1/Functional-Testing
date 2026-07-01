using Api.Application.Features.Usuarios.GetUsuarioById;
using Api.Tests.Factories;
using Api.Tests.Infrastructure;
using FluentValidation;
using Shouldly;

namespace Api.Tests.Features.Usuarios;

public class GetUsuarioByIdTests : TestBase
{
    [Test]
    public async Task ShouldReturnUsuario_WhenIdExists()
    {
        // Arrange
        var usuario = UsuarioFactory.CreateFakeEntity();
        DbContext.Usuarios.Add(usuario);
        await DbContext.SaveChangesAsync();

        var query = new GetUsuarioByIdQuery(usuario.Id);

        // Act
        var result = await Mediator.Send(query);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(usuario.Id);
        result.Nombre.ShouldBe(usuario.Nombre);
        result.Apellido.ShouldBe(usuario.Apellido);
        result.Email.ShouldBe(usuario.Email.Value);
    }

    [Test]
    public async Task ShouldReturnNull_WhenIdDoesNotExist()
    {
        // Arrange
        var query = new GetUsuarioByIdQuery(Guid.NewGuid());

        // Act
        var result = await Mediator.Send(query);

        // Assert
        result.ShouldBeNull();
    }

    [Test]
    public async Task ShouldThrowValidationException_WhenIdIsEmpty()
    {
        // Arrange
        var query = new GetUsuarioByIdQuery(Guid.Empty);

        // Act & Assert
        var exception = await Should.ThrowAsync<ValidationException>(async () => await Mediator.Send(query));
        exception.Errors.ShouldContain(x => x.PropertyName == nameof(GetUsuarioByIdQuery.Id));
    }
}
