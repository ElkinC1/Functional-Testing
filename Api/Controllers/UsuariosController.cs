using Api.Application.Features.Usuarios.CreateUsuario;
using Api.Application.Features.Usuarios.GetUsuarios;
using Api.Application.Features.Usuarios.GetUsuarioById;
using Api.Application.Features.Usuarios.UpdateUsuario;
using Api.Application.Features.Usuarios.DeleteUsuario;
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

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UsuarioDto>> GetById(Guid id)
    {
        var query = new GetUsuarioByIdQuery(id);
        var usuario = await Mediator.Send(query);
        if (usuario == null)
        {
            return NotFound();
        }
        return Ok(usuario);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateUsuarioCommand command)
    {
        var usuarioId = await Mediator.Send(command);
        return Ok(usuarioId);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, UpdateUsuarioCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("El ID de la ruta no coincide con el ID del cuerpo.");
        }

        try
        {
            await Mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            await Mediator.Send(new DeleteUsuarioCommand(id));
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
