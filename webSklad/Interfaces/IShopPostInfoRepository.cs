using webSklad.Models;

namespace webSklad.Interfaces
{
    public interface IShopPostInfoRepository
    {
        Task<ShopPostInfo> GetCurrentryShopPostInfo();

        Task<bool> Save();

        Task<bool> CreateShopPostInfo(ShopPostInfo shopPostInfo);

        Task<bool> SaveAsync();

        Task<ShopPostInfo> GetShopPostInfoById(int id);

        Task<bool> DeleteShopPostInfo(int id);

        Task<bool> UpdateShopPostInfo(ShopPostInfo shopPost);

    }
}
