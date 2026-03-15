using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.DTOs.Boards;

namespace TaskFlow.API.Controllers;

[ApiController]
[Route("api/boards")]
[Authorize]
public class BoardController : ControllerBase
{
    private readonly IBoardService _boardService;
    
    public BoardController(IBoardService boardService)
    {
        _boardService = boardService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetMyBoards()
        => Ok(await _boardService.GetMyBoardsAsync());
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBoardDetail(Guid id)
        => Ok(await _boardService.GetBoardDetailAsync(id));
    
    [HttpPost]
    public async Task<IActionResult> CreateBoard(CreateBoardRequest request)
    {
        var board = await _boardService.CreateBoardAsync(request);
        return CreatedAtAction(nameof(GetBoardDetail), new { id = board.Id }, board);
    }
    
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateBoard(Guid id, UpdateBoardRequest request)
    {
        await _boardService.UpdateBoardAsync(id, request);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBoard(Guid id)
    {
        await _boardService.DeleteBoardAsync(id);
        return NoContent();
    }
    
    [HttpGet("{id}/activities")]
    public async Task<IActionResult> GetActivities(Guid id, [FromQuery] int limit = 50)
        => Ok(await _boardService.GetActivityLogsAsync(id, limit));
    
    [HttpGet("{id}/members")]
    public async Task<IActionResult> GetMembers(Guid id)
        => Ok(await _boardService.GetMembersAsync(id));

    [HttpPost("{id}/members")]
    public async Task<IActionResult> InviteMember(Guid id, InviteMemberRequest request)
    {
        await _boardService.InviteMemberAsync(id, request);
        return NoContent();
    }

    [HttpDelete("{id}/members/{userId}")]
    public async Task<IActionResult> RemoveMember(Guid id, Guid userId)
    {
        await _boardService.RemoveMemberAsync(id, userId);
        return NoContent();
    }
}