namespace API.Interfaces;

/// <summary>
/// Interface for the Unit of Work pattern, providing access to repositories and transaction management
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Gets the user repository
    /// </summary>
    IUserRepository UserRepository { get; }

    /// <summary>
    /// Gets the message repository
    /// </summary>
    IMessageRepository MessageRepository { get; }

    /// <summary>
    /// Gets the likes repository
    /// </summary>
    ILikesRepository LikesRepository { get; }

    /// <summary>
    /// Gets the photo repository
    /// </summary>
    IPhotoRepository PhotoRepository { get; }

    /// <summary>
    /// Saves all changes made in this unit of work
    /// </summary>
    /// <returns>True if changes were saved successfully, false otherwise</returns>
    Task<bool> Complete();

    /// <summary>
    /// Checks if there are any pending changes in this unit of work
    /// </summary>
    /// <returns>True if there are pending changes, false otherwise</returns>
    bool HasChanges();
}