using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.DTOs.Columns;

namespace TaskFlow.API.Controllers;

[ApiController]
[Route("api/columns")]
[Authorize]
public class ColumnsController : ControllerBase
{
    private readonly IColumnService _columnService;

    public ColumnsController(IColumnService columnService) => _columnService = columnService;

    [HttpPost]
    public async Task<IActionResult> CreateColumn(CreateColumnRequest request)
        => Ok(await _columnService.CreateColumnAsync(request));

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateColumn(Guid id, UpdateColumnRequest request)
    {
        await _columnService.UpdateColumnAsync(id, request);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteColumn(Guid id)
    {
        await _columnService.DeleteColumnAsync(id);
        return NoContent();
    }
}