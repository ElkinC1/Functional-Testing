using Api.Application.Features.Usuarios.CreateUsuario;
using Api.Application.Features.Usuarios.GetUsuarios;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class UsuariosController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<UsuarioDto>>> GetAll(
        [FromQuery] GetUsuariosQuery query
    )
    {
        var usuarios = await Mediator.Send(query);
        return Ok(usuarios);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateUsuarioCommand command)
    {
        var usuarioId = await Mediator.Send(command);
        return Ok(usuarioId);
    }
}
