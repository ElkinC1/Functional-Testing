using Api.Domain.ValueObjects;
using Api.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Features.Usuarios.UpdateUsuario;

public record UpdateUsuarioCommand(Guid Id, string Nombre, string Apellido, string Email) : IRequest;

public class UpdateUsuarioCommandValidator : AbstractValidator<UpdateUsuarioCommand>
{
    public UpdateUsuarioCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID de usuario no puede estar vacío.");

        RuleFor(x => x.Nombre)
            .NotEmpty()
            .WithMessage("El nombre no puede estar vacío.")
            .MaximumLength(100)
            .WithMessage("El nombre no puede superar los 100 caracteres.");

        RuleFor(x => x.Apellido)
            .NotEmpty()
            .WithMessage("El apellido no puede estar vacío.")
            .MaximumLength(100)
            .WithMessage("El apellido no puede superar los 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("El email no puede estar vacío.")
            .Must(email =>
            {
                try
                {
                    _ = Email.From(email);
                    return true;
                }
                catch (Vogen.ValueObjectValidationException)
                {
                    return false;
                }
            })
            .WithMessage("El formato del email no es válido.");
    }
}

public class UpdateUsuarioCommandHandler : IRequestHandler<UpdateUsuarioCommand>
{
    private readonly ApplicationDbContext _context;

    public UpdateUsuarioCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateUsuarioCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        if (usuario == null)
        {
            throw new KeyNotFoundException($"Usuario con ID {request.Id} no fue encontrado.");
        }

        var email = Email.From(request.Email);
        usuario.UpdateProfile(request.Nombre, request.Apellido, email);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
