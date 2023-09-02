using Microsoft.AspNetCore.Identity;
using webSklad.Data;
using webSklad.Interfaces;
using webSklad.Models.ViewModels;
using webSklad.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

namespace webSklad.Repository
{
    public class FopRepository : IFopRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly WebSkladContext _context;

        public FopRepository(UserManager<User> userManager, WebSkladContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public Task<bool> CreateFop(Fop fop)
        {
            _context.FOPS.Add(fop);
            return Save();
        }

        public async Task<bool> DeleteFop(int fopId)
        {
            var fopToDelete = await _context.FOPS.FindAsync(fopId);
            if (fopToDelete != null)
            {
                _context.FOPS.Remove(fopToDelete);
                return await Save();
            }
            return false; 
        }

        public async Task<Fop> GetFopById(int fopId)
        {
             return await _context.FOPS.FindAsync(fopId);

            
        }

        public async Task<ShopPostInfo> GetShopPostInfoWithFOPByIdAsync(CreateFOPViewModel model)
        {
            var shopPostInfo = await _context.ShopPostInfos
                .Include(s => s.FOPS)
                .FirstOrDefaultAsync(s => s.Id == model.ShopPostInfoId
                );

            return shopPostInfo;
        }

        public async Task<List<Fop>> LoadFopsForPostAsync(int shopPostInfoId)
        {
            var fops = await _context.FOPS
                 .Where(f => f.ShopPostInfoId == shopPostInfoId)
                 .ToListAsync();

            return fops;
        }

        public async Task<List<Fop>> LoadFopsForShopAsync(int shopInfoId)
        {
            var fops = await _context.FOPS
                .Where(f => f.ShopPostInfoId == shopInfoId)
                .ToListAsync();

            return fops;
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }

        public async Task<bool> UpdateFop(Fop fop)
        {
            _context.FOPS.Update(fop);
            return await Save();
        }
    }
}
