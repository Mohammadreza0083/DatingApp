using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BuggyController(DataContext context) : BaseApiController
{
    [Authorize]
    [HttpGet("auth")]
    public ActionResult<string> GetAuth()
    {
        return "You are authorized";
    }

    [HttpGet("not-found")]
    public ActionResult<AppUsers> GetNotFound()
    {
        AppUsers? user = context.Users.Find(-1);
        if (user is null)
            return NotFound();
        return user;
    }

    [HttpGet("server-error")]
    public ActionResult<AppUsers> GetServerError()
    {
        AppUsers user = context.Users.Find(-1) ?? throw new Exception("Some server error");
        return user;
    }

    [HttpGet("bad-request")]
    public ActionResult<string> GetBadRequest()
    {
        return BadRequest("This was a bad request");
    }
}
