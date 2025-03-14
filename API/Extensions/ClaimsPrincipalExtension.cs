﻿using System.Security.Claims;

namespace API.Extensions;

public static class ClaimsPrincipalExtension
{
    public static string GetUsername(this ClaimsPrincipal user)
    {
        var username = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new Exception("No user found");
        return username;
    }
}