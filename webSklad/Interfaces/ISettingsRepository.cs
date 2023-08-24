using webSklad.Models;

namespace webSklad.Interfaces
{
   public interface ISettingsRepository
    {
        List<User> GetWorkersForCurrentUser(string currentUserId);
        List<ShopPostInfo> GetShopsForCurrentUser(string currentUserId);
    }
}
