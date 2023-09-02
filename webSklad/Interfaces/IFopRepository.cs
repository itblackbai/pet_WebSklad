using webSklad.Models.ViewModels;
using webSklad.Models;

namespace webSklad.Interfaces
{
    public interface IFopRepository
    {
        Task<ShopPostInfo> GetShopPostInfoWithFOPByIdAsync(CreateFOPViewModel model);
        Task<bool> Save();

        Task<bool> CreateFop(Fop fop);

        Task<Fop> GetFopById(int fopId);
        Task<bool> DeleteFop(int fopId);

        Task<bool> UpdateFop(Fop fop);

        Task<List<Fop>> LoadFopsForShopAsync(int shopInfoId);

        Task<List<Fop>> LoadFopsForPostAsync(int shopPostInfoId);

    }
}
