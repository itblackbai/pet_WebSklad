using Microsoft.EntityFrameworkCore;
using webSklad.Data;
using webSklad.Interfaces;
using webSklad.Models;

namespace webSklad.Repository
{
    public class CartRegisterRepository : ICartRegisterRepository
    {
        private readonly WebSkladContext _context;

        public CartRegisterRepository(WebSkladContext context)
        {
            _context = context;
        }

        public  Task<bool> AddCartRegister(int? cartRegisterId)
        {
            _context.AddAsync(cartRegisterId);
            return Save();
        }

        public Task<bool> AddCartRegisterCart(CartRegister cartRegisterId)
        {
            _context.AddAsync(cartRegisterId);
            return Save();
        }

        public async Task<CartRegister> GetCartRegisterById(int cartregisterid, string currentUser)
        {
            return await _context.CartRegisters
                .FirstOrDefaultAsync(i => i.Id == cartregisterid && i.UserProductId == currentUser);
        }

        public async Task<CartRegister> GetCartRegisterOrdering( string currentUser)
        {
            return await _context.CartRegisters
                .OrderByDescending(i => i.Id)
                .FirstOrDefaultAsync(i => i.UserProductId == currentUser);
        }

        public int GetLastCartRegisterId()
        {
            return  _context.CartRegisters
                .OrderByDescending(i => i.Id)
                .Select(i => i.Id)
                .FirstOrDefault();
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }
    }
}
