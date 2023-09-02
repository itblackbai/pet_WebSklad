using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using webSklad.Interfaces;
using webSklad.Models;

namespace webSklad.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRepository(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _roleManager = roleManager;
        }

        public async Task AddUserToRoleAsync(User user, string role)
        {
            await _userManager.AddToRoleAsync(user, role);

        }

        public async Task AddUserToRolesAsync(User user, IEnumerable<string> roles)
        {
            await _userManager.AddToRolesAsync(user, roles);
        }

        public async Task<IdentityResult> CreateUserAsync(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            return result;
        }

        public async Task<IdentityResult> DeleteUserAsync(User user)
        {
            var result = await _userManager.DeleteAsync(user);
            return result;
        }

        public async Task<User> GetCurrentyUser()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null || !httpContext.User.Identity.IsAuthenticated)
            {
                return null;
            }

            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }

            var currentUser = await _userManager.FindByIdAsync(userId);
            return currentUser;
        }

        public async Task<List<IdentityRole>> GetEditableRolesAsync(List<string> roleNames)
        {
            var allRoles = _roleManager.Roles.ToList();

            var editableRoles = allRoles
                .Where(role => !roleNames.Contains(role.Name))
                .ToList();

            return editableRoles;
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user;
        }

        public async Task<List<string>> GetUserRolesAsync(User user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            return userRoles.ToList();
        }

        public async Task<List<User>> GetWorkersAsync()
        {
            var workers = _userManager.Users.ToList();
            return workers;
        }

        public async Task RemoveUserFromRolesAsync(User user, IEnumerable<string> roles)
        {
            await _userManager.RemoveFromRolesAsync(user, roles);
        }

        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            var result = await _userManager.UpdateAsync(user);
            return result;
        }

        public async Task<string> GetCurrentUserIdAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null || !httpContext.User.Identity.IsAuthenticated)
            {
                return null;
            }

            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userId;
        }

        public async Task<User> GetCurrentUserCreatedByUserAsync()
        {
            var currentUser = await GetCurrentyUser();
            if (currentUser == null)
            {
                return null;
            }

            var createdByUserId = currentUser.CreatedByUserId;
            if (string.IsNullOrEmpty(createdByUserId))
            {
                return null;
            }

            var createdByUser = await GetUserByIdAsync(createdByUserId);
            return createdByUser;
        }


    }
}
