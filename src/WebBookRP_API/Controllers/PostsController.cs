using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBookRP_API.DTOs;
using WebBookRP_API.Interfaces;

namespace WebBookRP_API.Controllers;

[ApiController]
[Route("posts")]
public class PostsController(IPostService postService) : ControllerBase
{
    private readonly IPostService _postService = postService;

    private bool IsAuthenticated() => User.Identity?.IsAuthenticated == true;

    private string? CurrentUserId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<PostListItemResponseDto>>> GetAll([FromQuery] string? status)
    {
        return Ok(await _postService.ListAsync(status, IsAuthenticated()));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<PostDetailResponseDto>> GetById(int id, [FromQuery] string? status)
    {
        var post = await _postService.GetByIdAsync(id, status, IsAuthenticated());
        return post is null ? NotFound() : Ok(post);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<PostDetailResponseDto>> Create([FromBody] PostCreateRequestDto request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var created = await _postService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<ActionResult<PostDetailResponseDto>> Update(int id, [FromBody] PostUpdateRequestDto request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var updated = await _postService.UpdateAsync(id, request);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _postService.DeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }

    [HttpPost("{id:int}/likes/toggle")]
    [AllowAnonymous]
    public async Task<ActionResult<LikeToggleResponseDto>> ToggleLike(int id)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var likes = await _postService.ToggleLikeAsync(id, ip);
        if (likes is null)
            return NotFound();

        return Ok(new LikeToggleResponseDto { LikesCount = likes.Value });
    }

    [HttpPost("{id:int}/comments")]
    [AllowAnonymous]
    public async Task<ActionResult<CommentResponseDto>> AddComment(int id, [FromBody] CommentCreateRequestDto request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var userId = IsAuthenticated() ? CurrentUserId() : null;
            var created = await _postService.AddCommentAsync(id, request, userId);
            return created is null ? NotFound() : Ok(created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{postId:int}/comments/{commentId:int}/author-like")]
    [Authorize]
    public async Task<IActionResult> PatchAuthorLike(
        int postId,
        int commentId,
        [FromBody] AuthorLikePatchRequestDto request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var ok = await _postService.SetAuthorLikeAsync(postId, commentId, request.AuthorLike);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{postId:int}/comments/{commentId:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteComment(int postId, int commentId)
    {
        var ok = await _postService.DeleteCommentAsync(postId, commentId);
        return ok ? NoContent() : NotFound();
    }
}
