﻿using API.DTO;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class UserRepository(UserManager<AppUsers> manager, DataContext context, IMapper mapper): IUserRepository
{
    /// <summary>
    /// Unnecessary method
    /// EntityFramework flow data if anything change
    /// </summary>
    /// <param name="user"></param>
    public void Update(AppUsers user)
    {
        context.Entry(user).State = EntityState.Modified;
    }
    /// <summary>
    /// Get all user in database
    /// Include users photo because EntityFramework is lazy and cant find users photo
    /// </summary>
    /// <returns>A IEnumerable list of users in database</returns>
    public async Task<IEnumerable<AppUsers>> GetUsersAsync()
    {
        List<AppUsers> users = await context.Users
            .Include(u => u.Photos)
            .ToListAsync();
        return users;
    }

    /// <summary>
    /// Find user by id
    /// use Linq (FirstOrDefaultAsync)
    /// Include user photo because EntityFramework is lazy and cant find user photo
    /// Async Method
    /// </summary>
    /// <param name="id"></param>
    /// <returns>
    /// User is find
    /// Null is user notfound
    /// </returns>
    public async Task<AppUsers?> GetUserByIdAsync(int id)
    {
        AppUsers? user = await context.Users
            .Include(u => u.Photos)
            .FirstOrDefaultAsync(u => u.Id == id);
        return user;
    }

    /// <summary>
    /// Find user by username
    /// Use Linq (SingleOrDefaultAsync) we have unique username in DB
    /// Include user photo because EntityFramework is lazy and cant find user photo
    /// Async Method
    /// </summary>
    /// <param name="username"></param>
    /// <returns>
    /// User if found
    /// Null if notfound
    /// </returns>
    public async Task<AppUsers?> GetUserByUsernameAsync(string username)
    {
        AppUsers? user = await context.Users
            .Include(u => u.Photos)
            .SingleOrDefaultAsync(u => u.NormalizedUserName == username.ToUpper());
        return user;
    }

    /// <summary>
    /// Map users to members
    /// </summary>
    /// <returns>list of MemberDto or Null</returns>
    public async Task<PagedList<MembersDto>> GetAllMembersAsync(UserParams userParams)
    {
        var query = context.Users.AsQueryable();
        
        query = query.Where(u=> u.UserName != userParams.CurrentUsername);

        if (userParams.Gender != null)
        {
            query = query.Where(u => u.Gender == userParams.Gender);
        }
        
        var minDob = DateOnly.FromDateTime(DateTime.Now.AddYears(-userParams.MaxAge - 1));
        
        var maxDob = DateOnly.FromDateTime(DateTime.Now.AddYears(-userParams.MinAge));
        
        query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
        
        query = userParams.OrderBy switch
        {
            "created" => query.OrderByDescending(u => u.Created),
            _ => query.OrderByDescending(u => u.LastActivity)
        };
        
        return await PagedList<MembersDto>.CreateAsync(query.ProjectTo<MembersDto>(mapper.ConfigurationProvider),
            userParams.PageNum, userParams.PageSize);
    }

    /// <summary>
    /// Map user to MembersDto 
    /// </summary>
    /// <param name="username"></param>
    /// <returns>MemberDto or Null</returns>
    public async Task<MembersDto?> GetMemberAsync(string username)
    {
        MembersDto? membersDto = await context.Users
            .Where(u => u.NormalizedUserName == username.ToUpper())
            .ProjectTo<MembersDto>(mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
        return membersDto;
    }

    public async Task<AppUsers?> AddUserAsync(RegisterDto registerDto)
    {
        
        // Check if user with the same username already exists
        if (await context.Users.SingleOrDefaultAsync(u => u.NormalizedUserName == registerDto.Username.ToUpper()) 
            is not null)
        {
            throw new Exception("Username is taken");
        }
        var user = mapper.Map<AppUsers>(registerDto);
        user.UserName = registerDto.Username.ToLower();
        var result = await manager.CreateAsync(user, registerDto.Password);
        if (result.Succeeded)
        {
            return user;
        }
        foreach (var error in result.Errors)
        {
            throw new Exception(error.Description);
        }
        return null;
    }
}