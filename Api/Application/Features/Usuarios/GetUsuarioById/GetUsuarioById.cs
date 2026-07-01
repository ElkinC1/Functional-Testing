using Api.Application.Features.Usuarios.GetUsuarios;
using Api.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Features.Usuarios.GetUsuarioById;

public record GetUsuarioByIdQuery(Guid Id) : IRequest<UsuarioDto?>;

public class GetUsuarioByIdQueryValidator : AbstractValidator<GetUsuarioByIdQuery>
{
    public GetUsuarioByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID de usuario no puede estar vacío.");
    }
}

public class GetUsuarioByIdQueryHandler : IRequestHandler<GetUsuarioByIdQuery, UsuarioDto?>
{
    private readonly ApplicationDbContext _context;

    public GetUsuarioByIdQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UsuarioDto?> Handle(GetUsuarioByIdQuery request, CancellationToken cancellationToken)
    {
        var usuario = await _context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        if (usuario == null)
        {
            return null;
        }

        return new UsuarioDto(usuario.Id, usuario.Nombre, usuario.Apellido, usuario.Email.Value);
    }
}
