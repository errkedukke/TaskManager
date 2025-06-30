using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Features.TaskItem;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TaskItemsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TaskItemDto>>> GetTaskItems()
    {
        return new List<TaskItemDto>();
    }
}
