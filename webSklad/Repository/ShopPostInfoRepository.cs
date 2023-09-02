using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using webSklad.Data;
using webSklad.Interfaces;
using webSklad.Models;

namespace webSklad.Repository
{
    public class ShopPostInfoRepository : IShopPostInfoRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly WebSkladContext _context;

        public ShopPostInfoRepository(UserManager<User> userManager, WebSkladContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public  Task<bool> CreateShopPostInfo(ShopPostInfo shopPostInfo)
        {
            _context.ShopPostInfos.Add(shopPostInfo);
            return Save();
        }

        public async Task<ShopPostInfo> GetCurrentryShopPostInfo()
        {
            var shopPostInfo = await _context.ShopPostInfos
                 .FirstOrDefaultAsync();

            return shopPostInfo;
        }

        public async Task<ShopPostInfo> GetShopPostInfoById(int id)
        {
            return await _context.ShopPostInfos.FirstOrDefaultAsync(s => s.Id == id);
        }


        public async Task<bool> Save()
        {
             var saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }

        public async Task<bool> SaveAsync()
        {
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteShopPostInfo(int id)
        {
            var shopPostInfoToDelete = await _context.ShopPostInfos
                .Include(s => s.FOPS)
                .Include(s => s.SalesRepresentatives)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shopPostInfoToDelete != null)
            {
                _context.Remove(shopPostInfoToDelete);

               
                foreach (var fop in shopPostInfoToDelete.FOPS.ToList())
                {
                    _context.FOPS.Remove(fop);
                }

                foreach (var sr in shopPostInfoToDelete.SalesRepresentatives.ToList())
                {
                    _context.SalesRepresentatives.Remove(sr);
                }

                return await Save();
            }

            return false; 
        }

       

        public async Task<bool> UpdateShopPostInfo(ShopPostInfo shopPost)
        {
            _context.ShopPostInfos.Update(shopPost);
            return await Save();
        }
    }
}
