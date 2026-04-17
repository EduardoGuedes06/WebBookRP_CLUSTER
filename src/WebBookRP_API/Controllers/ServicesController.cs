using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBookRP_API.DTOs;
using WebBookRP_API.Interfaces;

namespace WebBookRP_API.Controllers;

[ApiController]
[Route("services")]
public class ServicesController(IServiceItemService serviceItemService) : ControllerBase
{
    private readonly IServiceItemService _serviceItemService = serviceItemService;

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<ServiceResponseDto>>> GetActive()
    {
        return Ok(await _serviceItemService.GetActivePublicAsync());
    }

    [HttpGet("all")]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<ServiceResponseDto>>> GetAll()
    {
        return Ok(await _serviceItemService.GetAllForAdminAsync());
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<ServiceResponseDto>> GetById(Guid id)
    {
        var item = await _serviceItemService.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ServiceResponseDto>> Create([FromBody] ServiceCreateRequestDto request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var created = await _serviceItemService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<ServiceResponseDto>> Update(Guid id, [FromBody] ServiceUpdateRequestDto request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var updated = await _serviceItemService.UpdateAsync(id, request);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        var ok = await _serviceItemService.DeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }
}
