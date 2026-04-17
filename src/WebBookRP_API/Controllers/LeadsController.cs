using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBookRP_API.DTOs;
using WebBookRP_API.Interfaces;

namespace WebBookRP_API.Controllers;

[ApiController]
[Route("leads")]
public class LeadsController(ILeadService leadService) : ControllerBase
{
    private readonly ILeadService _leadService = leadService;

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<LeadResponseDto>> Create([FromBody] LeadCreatePublicRequestDto request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var created = await _leadService.CreatePublicAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<LeadsPagedResponseDto>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromQuery] string? search = null)
    {
        return Ok(await _leadService.GetPagedAsync(page, pageSize, status, search));
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<LeadResponseDto>> GetById(int id)
    {
        var lead = await _leadService.GetByIdAsync(id);
        return lead is null ? NotFound() : Ok(lead);
    }

    [HttpPatch("{id:int}/status")]
    [Authorize]
    public async Task<IActionResult> PatchStatus(int id, [FromBody] LeadStatusPatchRequestDto request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var ok = await _leadService.UpdateStatusAsync(id, request.Status);
            return ok ? NoContent() : NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:int}/notes")]
    [Authorize]
    public async Task<IActionResult> PatchNotes(int id, [FromBody] LeadNotesPatchRequestDto request)
    {
        var ok = await _leadService.UpdateNotesAsync(id, request.Notes);
        return ok ? NoContent() : NotFound();
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<ActionResult<LeadResponseDto>> Update(int id, [FromBody] LeadUpdateRequestDto request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var updated = await _leadService.UpdateAsync(id, request);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _leadService.DeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }
}
