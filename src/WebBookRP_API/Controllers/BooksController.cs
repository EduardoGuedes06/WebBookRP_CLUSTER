using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBookRP_API.DTOs;
using WebBookRP_API.Interfaces;

namespace WebBookRP_API.Controllers;

[ApiController]
[Route("books")]
public class BooksController(IBookService bookService) : ControllerBase
{
    private readonly IBookService _bookService = bookService;

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<BookResponseDto>>> GetAll()
    {
        return Ok(await _bookService.GetAllForAdminAsync());
    }

    [HttpGet("catalog")]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<BookResponseDto>>> GetCatalog()
    {
        return Ok(await _bookService.GetPublicCatalogAsync());
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<BookResponseDto>> GetById(Guid id)
    {
        var book = await _bookService.GetPublicByIdAsync(id);
        return book is null ? NotFound() : Ok(book);
    }

    [HttpGet("admin/{id:guid}")]
    [Authorize]
    public async Task<ActionResult<BookResponseDto>> GetByIdAdmin(Guid id)
    {
        var book = await _bookService.GetByIdForAdminAsync(id);
        return book is null ? NotFound() : Ok(book);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<BookResponseDto>> Create([FromBody] BookCreateRequestDto request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var created = await _bookService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<BookResponseDto>> Update(Guid id, [FromBody] BookUpdateRequestDto request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var updated = await _bookService.UpdateAsync(id, request);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        var ok = await _bookService.DeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }
}
