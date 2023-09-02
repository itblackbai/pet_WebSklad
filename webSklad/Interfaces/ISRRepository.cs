using webSklad.Models.ViewModels;
using webSklad.Models;

namespace webSklad.Interfaces
{
    public interface ISRRepository
    {
        Task<ShopPostInfo> GetShopPostInfoWithSRByIdAsync(CreateSRViewModel model);
        Task<bool> Save();

        Task<bool> CreateSR(SalesRepresentative sr);

        Task<SalesRepresentative> GetSRById(int srId);
        Task<bool> DeleteSR(int srId);

        Task<bool> UpdateSR(SalesRepresentative sr);

        Task<List<SalesRepresentative>> LoadSRsForPostAsync(int srId);

    }
}
