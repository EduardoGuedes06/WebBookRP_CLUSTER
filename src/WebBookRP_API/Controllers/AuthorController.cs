using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBookRP_API.DTOs;
using WebBookRP_API.Interfaces;

namespace WebBookRP_API.Controllers;

[ApiController]
[Route("author")]
public class AuthorController(IAuthorService authorService) : ControllerBase
{
    private readonly IAuthorService _authorService = authorService;

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<AuthorResponseDto>> Get()
    {
        return Ok(await _authorService.GetAsync());
    }

    [HttpPut]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<AuthorResponseDto>> Update([FromBody] AuthorUpdateRequestDto request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        return Ok(await _authorService.UpdateAsync(request));
    }
}
