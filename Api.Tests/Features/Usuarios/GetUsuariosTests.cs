using Api.Application.Features.Usuarios.GetUsuarios;
using Api.Tests.Factories;
using Api.Tests.Infrastructure;
using FluentValidation;
using Shouldly;

namespace Api.Tests.Features.Usuarios;

public class GetUsuariosTests : TestBase
{
    [Test]
    public async Task ShouldReturnPaginatedUsuarios_WhenQueryIsSent()
    {
        // Arrange
        var u1 = UsuarioFactory.CreateFakeEntity();
        var u2 = UsuarioFactory.CreateFakeEntity();
        var u3 = UsuarioFactory.CreateFakeEntity();

        DbContext.Usuarios.AddRange(u1, u2, u3);
        await DbContext.SaveChangesAsync();

        var query = new GetUsuariosQuery(PageNumber: 1, PageSize: 2);

        // Act
        var result = await Mediator.Send(query);

        // Assert
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBe(3);
        result.Items.Count.ShouldBe(2);
        result.TotalPages.ShouldBe(2);
        result.HasNextPage.ShouldBeTrue();
        result.HasPreviousPage.ShouldBeFalse();
    }

    [Test]
    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(-100)]
    public async Task ShouldThrowValidationException_WhenPageNumberIsLessThanOne(
        int invalidPageNumber
    )
    {
        // Arrange
        var query = new GetUsuariosQuery(PageNumber: invalidPageNumber, PageSize: 10);

        // Act & Assert
        var exception = await Should.ThrowAsync<ValidationException>(async () =>
            await Mediator.Send(query)
        );
        exception.Errors.ShouldContain(x => x.PropertyName == nameof(GetUsuariosQuery.PageNumber));
    }

    [Test]
    [TestCase(0)]
    [TestCase(-5)]
    public async Task ShouldThrowValidationException_WhenPageSizeIsLessThanOne(int invalidPageSize)
    {
        // Arrange
        var query = new GetUsuariosQuery(PageNumber: 1, PageSize: invalidPageSize);

        // Act & Assert
        var exception = await Should.ThrowAsync<ValidationException>(async () =>
            await Mediator.Send(query)
        );
        exception.Errors.ShouldContain(x => x.PropertyName == nameof(GetUsuariosQuery.PageSize));
    }

    [Test]
    public async Task ShouldThrowValidationException_WhenPageSizeIsTooLarge()
    {
        // Arrange
        var query = new GetUsuariosQuery(PageNumber: 1, PageSize: 101);

        // Act & Assert
        var exception = await Should.ThrowAsync<ValidationException>(async () =>
            await Mediator.Send(query)
        );
        exception.Errors.ShouldContain(x => x.PropertyName == nameof(GetUsuariosQuery.PageSize));
    }

    [Test]
    public async Task ShouldReturnEmptyPage_WhenNoUsersExist()
    {
        // Arrange
        var query = new GetUsuariosQuery(PageNumber: 1, PageSize: 10);

        // Act
        var result = await Mediator.Send(query);

        // Assert
        result.ShouldNotBeNull();
        result.Items.ShouldBeEmpty();
        result.TotalCount.ShouldBe(0);
        result.TotalPages.ShouldBe(0);
        result.HasNextPage.ShouldBeFalse();
        result.HasPreviousPage.ShouldBeFalse();
    }

    [Test]
    public async Task ShouldReturnEmptyPage_WhenPageIsOutOfBounds()
    {
        // Arrange
        var u1 = UsuarioFactory.CreateFakeEntity();
        var u2 = UsuarioFactory.CreateFakeEntity();

        DbContext.Usuarios.AddRange(u1, u2);
        await DbContext.SaveChangesAsync();

        var query = new GetUsuariosQuery(PageNumber: 3, PageSize: 2); // Only 1 page exists

        // Act
        var result = await Mediator.Send(query);

        // Assert
        result.ShouldNotBeNull();
        result.Items.ShouldBeEmpty();
        result.TotalCount.ShouldBe(2);
        result.TotalPages.ShouldBe(1);
        result.HasNextPage.ShouldBeFalse();
        result.HasPreviousPage.ShouldBeTrue(); // Page 3 > Page 1, so has previous
    }
}
