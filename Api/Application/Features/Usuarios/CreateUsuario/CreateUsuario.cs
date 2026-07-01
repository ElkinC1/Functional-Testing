using Api.Domain.Entities;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Persistence;
using FluentValidation;
using MediatR;

namespace Api.Application.Features.Usuarios.CreateUsuario;

public record CreateUsuarioCommand(string Nombre, string Apellido, string Email) : IRequest<Guid>;

public class CreateUsuarioCommandValidator : AbstractValidator<CreateUsuarioCommand>
{
    public CreateUsuarioCommandValidator()
    {
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

public class CreateUsuarioCommandHandler : IRequestHandler<CreateUsuarioCommand, Guid>
{
    private readonly ApplicationDbContext _context;

    public CreateUsuarioCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(
        CreateUsuarioCommand request,
        CancellationToken cancellationToken
    )
    {
        var email = Email.From(request.Email);

        var usuario = new Usuario(Guid.NewGuid(), request.Nombre, request.Apellido, email);

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync(cancellationToken);

        return usuario.Id;
    }
}
