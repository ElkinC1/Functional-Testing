using Api.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Features.Usuarios.DeleteUsuario;

public record DeleteUsuarioCommand(Guid Id) : IRequest;

public class DeleteUsuarioCommandValidator : AbstractValidator<DeleteUsuarioCommand>
{
    public DeleteUsuarioCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID de usuario no puede estar vacío.");
    }
}

public class DeleteUsuarioCommandHandler : IRequestHandler<DeleteUsuarioCommand>
{
    private readonly ApplicationDbContext _context;

    public DeleteUsuarioCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteUsuarioCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        if (usuario == null)
        {
            throw new KeyNotFoundException($"Usuario con ID {request.Id} no fue encontrado.");
        }

        usuario.Delete();

        await _context.SaveChangesAsync(cancellationToken);
    }
}
