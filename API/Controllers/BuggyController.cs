using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BuggyController(IUnitOfWork repo) : BaseApiController
{
    [Authorize]
    [HttpGet("auth")]
    public ActionResult<string> GetAuth()
    {
        return "You are authorized";
    }

    [HttpGet("not-found")]
    public async Task<ActionResult<AppUsers>> GetNotFound()
    {
        AppUsers? user = await repo.UserRepository.GetUserByIdAsync(-1);
        if (user is null)
            return NotFound();
        return user;
    }

    [HttpGet("server-error")]
    public async Task<ActionResult<AppUsers>> GetServerError()
    {
        AppUsers user = await repo.UserRepository.GetUserByIdAsync(-1) 
                        ?? throw new Exception("Some server error");
        return user;
    }

    [HttpGet("bad-request")]
    public ActionResult<string> GetBadRequest()
    {
        return BadRequest("This was a bad request");
    }
}
