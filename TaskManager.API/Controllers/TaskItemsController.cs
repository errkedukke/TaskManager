using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using TaskManager.Application.Features.TaskItem;
using TaskManager.Application.Features.TaskItem.Commands.CreateTaskItem;
using TaskManager.Application.Features.TaskItem.Commands.DeleteTaskItem;
using TaskManager.Application.Features.TaskItem.Commands.UpdateTaskItem;
using TaskManager.Application.Features.TaskItem.Queries.GetTaskItem;
using TaskManager.Application.Features.TaskItem.Queries.GetTaskItems;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TaskItemsController : ControllerBase
{
    private readonly ILogger<TaskItemsController> _logger;
    private readonly IMediator _mediator;
    private readonly string _internalServerError = "An error occurred while processing your request.";

    public TaskItemsController(ILogger<TaskItemsController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves a task item by its unique identifier.
    /// </summary>
    /// <param name="id">The ID of the task item.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The task item, if found.</returns>
    [HttpGet("{id:guid}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(TaskItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TaskItemDto>> GetTaskItem(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetTaskItemQuery(id);

        try
        {
            var result = await _mediator.Send(query, cancellationToken);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, _internalServerError);
        }
    }

    /// <summary>
    /// Retrieves all task items.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to abort the request.</param>
    /// <returns>List of task items.</returns>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(List<TaskItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TaskItemDto>>> GetTaskItems(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _mediator.Send(new GetTaskItemsQuery(), cancellationToken);

            if (response == null || response.Count == 0)
            {
                return NoContent();
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, _internalServerError);
        }
    }

    /// <summary>
    /// Creates a new task item.
    /// </summary>
    /// <param name="command">Creates new TaskItem having Task details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Newly created task ID.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Guid>> CreateTask([FromBody] CreateTaskItemCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var id = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetTaskItem), new { id }, id);
        }
        catch (Exception ex)
        {

            _logger.LogError(ex.Message);
            return StatusCode(500, _internalServerError);
        }
    }

    /// <summary>
    /// Updates an existing task item.
    /// </summary>
    /// <param name="id">Task ID.</param>
    /// <param name="command">Updated TaskItem details and data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskItemCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest("ID in URL does not match ID in body.");
        }

        try
        {
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, _internalServerError);
        }
    }

    /// <summary>
    /// Deletes a task item.
    /// </summary>
    /// <param name="id">Task ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteTask(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _mediator.Send(new DeleteTaskItemCommand { Id = id }, cancellationToken);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, _internalServerError);
        }
    }
}
