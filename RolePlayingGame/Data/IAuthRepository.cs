using RolePlayingGame.Models;

namespace RolePlayingGame.Data
{
    public interface IAuthRepository
    {
        Task<ServiceResponse<string>> Login(string username, string password);
        Task<ServiceResponse<int>> Register(string username, string password);
        Task<bool> UserExists(string username);
    }
}
