using API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Base controller class for all API controllers
/// Provides common functionality and attributes for API endpoints
/// </summary>
[ServiceFilter(typeof(LogUserActivity))] // Automatically logs user activity for all derived controllers
[ApiController] // Enables API-specific behaviors like automatic model validation
[Route("api/[controller]")] // Sets the base route for all derived controllers
public class BaseApiController : ControllerBase;
