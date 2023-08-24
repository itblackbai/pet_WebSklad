using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using webSklad.Data;
using webSklad.Interfaces;
using webSklad.Models;

namespace webSklad.Repository
{
    public class SettingsRepository : ISettingsRepository
    {
        private readonly WebSkladContext _context;

        public SettingsRepository(WebSkladContext context)
        {
            _context = context;
        }

        public List<User> GetWorkersForCurrentUser(string currentUserId)
        {
            return _context.Users
                .Where(u => u.CreatedByUserId == currentUserId)
                .ToList();
        }

        public List<ShopPostInfo> GetShopsForCurrentUser(string currentUserId)
        {
            return _context.ShopPostInfos
                .Include(s => s.FOPS)
                .Include(s => s.SalesRepresentatives)
                .Where(s => s.UserId == currentUserId)
                .ToList();
        }
    }
}
