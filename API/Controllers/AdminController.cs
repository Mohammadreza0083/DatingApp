﻿using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

/// <summary>
/// Controller for administrative operations
/// Handles user role management and photo moderation
/// </summary>
public class AdminController(UserManager<AppUsers> userManager, 
    IUnitOfWork repo, IPhotoServices photoServices) : BaseApiController
{
    /// <summary>
    /// Retrieves all users with their assigned roles
    /// Requires admin role
    /// </summary>
    /// <returns>List of users with their roles</returns>
    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRole()
    {
        var users = await userManager.Users
            .OrderBy(x => x.UserName)
            .Select(x => new
            {
                x.Id,
                Username = x.UserName,
                Roles = x.UserRoles.Select(r => r.Roles.Name).ToList()
            })
            .ToListAsync();
        return Ok(users);
    }

    /// <summary>
    /// Updates the roles assigned to a user
    /// Requires admin role
    /// </summary>
    /// <param name="username">Username of the user to update</param>
    /// <param name="roles">Comma-separated list of roles to assign</param>
    /// <returns>Updated list of user roles</returns>
    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("edit-roles/{username}")]
    public async Task<ActionResult> EditRoles(string username, string roles)
    {
        if (string.IsNullOrEmpty(roles))
        {
            return BadRequest("You must select at least one role");
        }
        var selectedRoles = roles.Split(",").ToArray();
        var user = await userManager.FindByNameAsync(username);
        if (user is null)
        {
            return BadRequest("User not found");
        }
        var userRoles = await userManager.GetRolesAsync(user);
        var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
        if (!result.Succeeded)
        {
            return BadRequest("Failed to add to roles");
        }
        result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
        if (!result.Succeeded)
        {
            return BadRequest("Failed to remove from roles");
        }
        return Ok(await userManager.GetRolesAsync(user));
    }

    /// <summary>
    /// Retrieves photos pending moderation
    /// Requires moderator role
    /// </summary>
    /// <returns>List of unapproved photos</returns>
    [Authorize(Policy = "ModeratorPhotoRole")]
    [HttpGet("photos-to-moderate")]
    public async Task<ActionResult> GetPhotosForModeration()
    {
        var photos = await repo.PhotoRepository.GetUnapprovedPhotosAsync();
        return Ok(photos);
    }

    /// <summary>
    /// Approves a photo for display
    /// Requires moderator role
    /// </summary>
    /// <param name="photoId">ID of the photo to approve</param>
    /// <returns>NoContent if successful, BadRequest if failed</returns>
    [Authorize(Policy = "ModeratorPhotoRole")]
    [HttpPost("approve-photo/{photoId}")]
    public async Task<ActionResult> ApprovePhoto(int photoId)
    {
        var photo = await repo.PhotoRepository.GetPhotoByIdAsync(photoId);
        if (photo == null)
        {
            return NotFound("Photo not found");
        }
        photo.IsApproved = true;
        var user = await repo.UserRepository.GetUserByPhotoIdAsync(photoId);
        if (user is null)
        {
            return BadRequest("User not found");
        }

        if (!user.Photos.Any(p => p.IsMain))
        {
            photo.IsMain = true;
        }
        if (await repo.Complete())
        {
            return NoContent();
        }
        return BadRequest("Failed to approve photo");
    }

    /// <summary>
    /// Rejects a photo and removes it from storage
    /// Requires moderator role
    /// </summary>
    /// <param name="photoId">ID of the photo to reject</param>
    /// <returns>NoContent if successful, BadRequest if failed</returns>
    [Authorize(Policy = "ModeratorPhotoRole")]
    [HttpPost("reject-photo/{photoId}")]
    public async Task<ActionResult> RejectPhoto(int photoId)
    {
        var photo = await repo.PhotoRepository.GetPhotoByIdAsync(photoId);
        if (photo == null)
        {
            return NotFound("Photo not found");
        }
        if (photo.PublicId is not null)
        {
            var result = await photoServices.DeletePhotoAsync(photo.PublicId);
            if (result.Result is "ok")
            {
                repo.PhotoRepository.RemovePhoto(photo);
            }
        }
        else
        {
            repo.PhotoRepository.RemovePhoto(photo);
        }
        if (await repo.Complete())
        {
            return NoContent();
        }
        return BadRequest("Failed to reject photo");
    }
}