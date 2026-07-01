using Api.Application.Features.Usuarios.CreateUsuario;
using Api.Application.Features.Usuarios.GetUsuarios;
using Api.Domain.Entities;
using Api.Domain.ValueObjects;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Api.Tests;

public class UsuarioFeaturesTests : TestBase
{
    [Test]
    public async Task ShouldCreateUsuario_WhenCommandIsValid()
    {
        var command = new CreateUsuarioCommand("Juan", "Perez", "juan.perez@example.com");

        var id = await Mediator.Send(command);

        Assert.That(id, Is.Not.EqualTo(Guid.Empty));

        var usuario = await DbContext.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
        Assert.That(usuario, Is.Not.Null);
        Assert.That(usuario.Nombre, Is.EqualTo("Juan"));
        Assert.That(usuario.Apellido, Is.EqualTo("Perez"));
        Assert.That(usuario.Email.Value, Is.EqualTo("juan.perez@example.com"));
    }

    [Test]
    public void ShouldThrowValidationException_WhenCommandIsInvalid()
    {
        var command = new CreateUsuarioCommand("", "Perez", "correo-invalido");

        Assert.ThrowsAsync<ValidationException>(async () => await Mediator.Send(command));
    }

    [Test]
    public async Task ShouldReturnPaginatedUsuarios_WhenQueryIsSent()
    {
        var u1 = new Usuario(Guid.NewGuid(), "Elkin", "Castillo", Email.From("elkin@example.com"));
        var u2 = new Usuario(Guid.NewGuid(), "Maria", "Gomez", Email.From("maria@example.com"));
        var u3 = new Usuario(Guid.NewGuid(), "Carlos", "Lopez", Email.From("carlos@example.com"));

        DbContext.Usuarios.AddRange(u1, u2, u3);
        await DbContext.SaveChangesAsync();

        var query = new GetUsuariosQuery(PageNumber: 1, PageSize: 2);

        var result = await Mediator.Send(query);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.TotalCount, Is.EqualTo(3));
        Assert.That(result.Items.Count, Is.EqualTo(2));
        Assert.That(result.TotalPages, Is.EqualTo(2));
        Assert.That(result.HasNextPage, Is.True);
        Assert.That(result.HasPreviousPage, Is.False);
    }
}
