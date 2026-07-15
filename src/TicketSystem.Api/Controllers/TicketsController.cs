using Microsoft.AspNetCore.Mvc;
using TicketSystem.Api.DTOs;
using TicketSystem.Api.Services;

namespace TicketSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TicketResponseDto>>> GetTickets(
        [FromQuery] string? status,
        [FromQuery] string? keyword,
        CancellationToken cancellationToken)
    {
        var tickets = await _ticketService.GetTicketsAsync(status, keyword, cancellationToken);
        return Ok(tickets);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TicketResponseDto>> GetTicket(int id, CancellationToken cancellationToken)
    {
        var ticket = await _ticketService.GetTicketByIdAsync(id, cancellationToken);
        return Ok(ticket);
    }

    [HttpPost]
    public async Task<ActionResult<TicketResponseDto>> CreateTicket(
        [FromBody] CreateTicketDto dto,
        CancellationToken cancellationToken)
    {
        var ticket = await _ticketService.CreateTicketAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, ticket);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TicketResponseDto>> UpdateTicket(
        int id,
        [FromBody] UpdateTicketDto dto,
        CancellationToken cancellationToken)
    {
        var ticket = await _ticketService.UpdateTicketAsync(id, dto, cancellationToken);
        return Ok(ticket);
    }

    [HttpPut("{id:int}/status")]
    public async Task<ActionResult<TicketResponseDto>> ChangeStatus(
        int id,
        [FromBody] ChangeTicketStatusDto dto,
        CancellationToken cancellationToken)
    {
        var ticket = await _ticketService.ChangeStatusAsync(id, dto, cancellationToken);
        return Ok(ticket);
    }
}
