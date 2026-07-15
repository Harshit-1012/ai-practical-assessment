using Microsoft.AspNetCore.Mvc;
using TicketSystem.Api.DTOs;
using TicketSystem.Api.Services;

namespace TicketSystem.Api.Controllers;

[ApiController]
[Route("api/tickets/{ticketId:int}/comments")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CommentResponseDto>>> GetComments(
        int ticketId,
        CancellationToken cancellationToken)
    {
        var comments = await _commentService.GetCommentsByTicketIdAsync(ticketId, cancellationToken);
        return Ok(comments);
    }

    [HttpPost]
    public async Task<ActionResult<CommentResponseDto>> CreateComment(
        int ticketId,
        [FromBody] CreateCommentDto dto,
        CancellationToken cancellationToken)
    {
        var comment = await _commentService.CreateCommentAsync(ticketId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetComments), new { ticketId }, comment);
    }
}
