using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controller for testing error handling and authorization
/// Used for development and testing purposes only
/// </summary>
public class BuggyController(IUnitOfWork repo) : BaseApiController
{
    /// <summary>
    /// Tests authorization functionality
    /// Requires authenticated user
    /// </summary>
    /// <returns>Success message if authorized</returns>
    [Authorize]
    [HttpGet("auth")]
    public ActionResult<string> GetAuth()
    {
        return "You are authorized";
    }

    /// <summary>
    /// Tests NotFound response handling
    /// Always returns NotFound as it queries for a non-existent user
    /// </summary>
    /// <returns>NotFound response</returns>
    [HttpGet("not-found")]
    public async Task<ActionResult<AppUsers>> GetNotFound()
    {
        AppUsers? user = await repo.UserRepository.GetUserByIdAsync(-1);
        if (user is null)
            return NotFound();
        return user;
    }

    /// <summary>
    /// Tests server error handling
    /// Simulates an unhandled exception
    /// </summary>
    /// <returns>Throws an exception to test error handling</returns>
    [HttpGet("server-error")]
    public async Task<ActionResult<AppUsers>> GetServerError()
    {
        AppUsers user = await repo.UserRepository.GetUserByIdAsync(-1) 
                        ?? throw new Exception("Some server error");
        return user;
    }

    /// <summary>
    /// Tests bad request handling
    /// Returns a BadRequest response
    /// </summary>
    /// <returns>BadRequest response with message</returns>
    [HttpGet("bad-request")]
    public ActionResult<string> GetBadRequest()
    {
        return BadRequest("This was a bad request");
    }
}
