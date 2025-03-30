using API.Entities;

namespace API.Interfaces;

public interface ITokenServices
{
    Task<string> CreateToken(AppUsers user);
}
