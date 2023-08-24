using Microsoft.AspNetCore.Identity;
using webSklad.Models;

namespace webSklad.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetCurrentyUser();
        Task<User> GetUserByIdAsync(string userId);
        Task<IdentityResult> CreateUserAsync(User user, string password);

        Task<IdentityResult> DeleteUserAsync(User user);
        Task AddUserToRoleAsync(User user, string role);

        Task<List<User>> GetWorkersAsync();

        Task<List<IdentityRole>> GetEditableRolesAsync(List<string> roleNames);

        Task<IdentityResult> UpdateUserAsync(User user);

        Task<List<string>> GetUserRolesAsync(User user);

        Task AddUserToRolesAsync(User user, IEnumerable<string> roles);
        Task RemoveUserFromRolesAsync(User user, IEnumerable<string> roles);

    }
}
