using Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Features.Usuarios.GetUsuarios;

public class GetUsuariosQueryHandler : IRequestHandler<GetUsuariosQuery, List<UsuarioDto>>
{
    private readonly ApplicationDbContext _context;

    public GetUsuariosQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<UsuarioDto>> Handle(GetUsuariosQuery request, CancellationToken cancellationToken)
    {
        return await _context.Usuarios
            .Select(u => new UsuarioDto(u.Id, u.Nombre, u.Apellido, u.Email.Value))
            .ToListAsync(cancellationToken);
    }
}
