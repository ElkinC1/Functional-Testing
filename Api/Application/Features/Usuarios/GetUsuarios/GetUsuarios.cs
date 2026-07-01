using Api.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Features.Usuarios.GetUsuarios;

public record GetUsuariosQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PagedResult<UsuarioDto>>;

public record UsuarioDto(Guid Id, string Nombre, string Apellido, string Email);

public record PagedResult<T>(List<T> Items, int PageNumber, int PageSize, int TotalCount)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

public class GetUsuariosQueryValidator : AbstractValidator<GetUsuariosQuery>
{
    public GetUsuariosQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("El número de página debe ser mayor o igual a 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("El tamaño de página debe ser mayor o igual a 1.")
            .LessThanOrEqualTo(100).WithMessage("El tamaño de página no puede exceder 100.");
    }
}

public class GetUsuariosQueryHandler : IRequestHandler<GetUsuariosQuery, PagedResult<UsuarioDto>>
{
    private readonly ApplicationDbContext _context;

    public GetUsuariosQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<UsuarioDto>> Handle(GetUsuariosQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Usuarios.AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(u => new UsuarioDto(u.Id, u.Nombre, u.Apellido, u.Email.Value))
            .ToListAsync(cancellationToken);

        return new PagedResult<UsuarioDto>(items, request.PageNumber, request.PageSize, totalCount);
    }
}
