using Microsoft.AspNetCore.Identity;
using webSklad.Data;
using webSklad.Models.ViewModels;
using webSklad.Models;
using Microsoft.EntityFrameworkCore;
using webSklad.Interfaces;
using static System.Net.WebRequestMethods;

namespace webSklad.Repository
{
    public class SRRepository : ISRRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly WebSkladContext _context;

        public SRRepository(UserManager<User> userManager, WebSkladContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public Task<bool> CreateSR(SalesRepresentative sr)
        {
             _context.SalesRepresentatives.Add(sr);
            return Save();
        }

        public async Task<bool> DeleteSR(int srId)
        {
            var srToDelete = await _context.SalesRepresentatives.FindAsync(srId);
            if (srToDelete != null)
            {
                _context.SalesRepresentatives.Remove(srToDelete);
                return await Save();
            }
            return false;
        }

        public async Task<ShopPostInfo> GetShopPostInfoWithSRByIdAsync(CreateSRViewModel model)
        {
            var shopPostInfo = await _context.ShopPostInfos
                .Include(s => s.SalesRepresentatives)
                .FirstOrDefaultAsync(s => s.Id == model.ShopPostInfoId);

            return shopPostInfo;
        }

        public async Task<SalesRepresentative> GetSRById(int srId)
        {
            return await _context.SalesRepresentatives.FindAsync(srId);
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }

        public async Task<bool> UpdateSR(SalesRepresentative sr)
        {
            _context.SalesRepresentatives.Update(sr);
            return await Save();
        }

        public async Task<List<SalesRepresentative>> LoadSRsForPostAsync(int srId)
        {
            var fops = await _context.SalesRepresentatives
                .Where(f => f.ShopPostInfoId == srId)
                .ToListAsync();

            return fops;
        }

    }
}