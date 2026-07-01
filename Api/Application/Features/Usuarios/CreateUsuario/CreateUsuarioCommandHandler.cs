using Api.Domain.Entities;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Persistence;
using MediatR;

namespace Api.Application.Features.Usuarios.CreateUsuario;

public class CreateUsuarioCommandHandler : IRequestHandler<CreateUsuarioCommand, Guid>
{
    private readonly ApplicationDbContext _context;

    public CreateUsuarioCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateUsuarioCommand request, CancellationToken cancellationToken)
    {
        var email = Email.From(request.Email);

        var usuario = new Usuario(
            Guid.NewGuid(),
            request.Nombre,
            request.Apellido,
            email
        );

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync(cancellationToken);

        return usuario.Id;
    }
}
