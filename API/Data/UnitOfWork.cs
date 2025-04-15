using API.Interfaces;

namespace API.Data;

/// <summary>
/// Unit of Work pattern implementation
/// Manages repositories and database transactions
/// </summary>
public class UnitOfWork(DataContext context, IUserRepository userRepository,
    IMessageRepository messageRepository, ILikesRepository likesRepository, 
    IPhotoRepository photoRepository) : IUnitOfWork
{
    /// <summary>
    /// Repository for user-related operations
    /// </summary>
    public IUserRepository UserRepository => userRepository;

    /// <summary>
    /// Repository for message-related operations
    /// </summary>
    public IMessageRepository MessageRepository => messageRepository;

    /// <summary>
    /// Repository for like-related operations
    /// </summary>
    public ILikesRepository LikesRepository => likesRepository;

    /// <summary>
    /// Repository for photo-related operations
    /// </summary>
    public IPhotoRepository PhotoRepository => photoRepository;

    /// <summary>
    /// Saves all changes made in this unit of work to the database
    /// </summary>
    /// <returns>True if changes were saved successfully, false otherwise</returns>
    public async Task<bool> Complete()
    {
        return await context.SaveChangesAsync() > 0;
    }

    /// <summary>
    /// Checks if there are any pending changes in the unit of work
    /// </summary>
    /// <returns>True if there are pending changes, false otherwise</returns>
    public bool HasChanges()
    {
        return context.ChangeTracker.HasChanges();
    }
}