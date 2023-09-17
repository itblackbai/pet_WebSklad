using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using webSklad.Data;
using webSklad.Interfaces;
using webSklad.Models;
using static System.Net.WebRequestMethods;
namespace webSklad.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly WebSkladContext _context;

        public CartRepository(WebSkladContext context)
        {

            _context = context;
        }

        public async Task<int> GetLastCartRegisterId()
        {
            return await _context.CartRegisters
                .OrderByDescending(i => i.Id)
                .Select(i => i.Id)
                .FirstOrDefaultAsync();
        }
        public void AddCartItem(Cart cartItem)
        {
             _context.CartItems.Add(cartItem);
        }

        public void SaveChangesAsync()
        {
             _context.SaveChangesAsync();
        }



        public async Task<List<IGrouping<int, Cart>>> GetCartItemsForUser(string userId, string type)
        {
            return await _context.CartItems
                .Include(c => c.Product)
                .Include(c => c.Invoice.PostFop)
                .Include(i => i.Invoice.User)
                .Include(c => c.Invoice.PostInfo)
                .Where(c => c.Invoice.UserInvoiceId == userId && c.TypeCart == type)
                .GroupBy(c => c.CartId)
                .ToListAsync();

          
        }

        public async Task<List<IGrouping<int, Cart>>> GetCartRegisterForUser(string userId, string type)
        {
            return await _context.CartItems
                .Include(c => c.Product)
                .Include(c => c.CartRegister)
                .Where(c => c.CartRegister.UserProductId == userId && c.TypeCart == type)
                .GroupBy(c => c.CartId)
                .ToListAsync();
        }

        public async Task DeleteCartRange(Cart cartItem)
        {
            _context.CartItems.RemoveRange(cartItem);
           
        }

        public async Task<List<Cart>> GetCartItemsByCartId(int cartId)
        {
            return await _context.CartItems
                .Where(c => c.CartId == cartId)
                .ToListAsync();
        }

        public async Task DeleteCartRange(List<Cart> cartItems)
        {
            _context.CartItems.RemoveRange(cartItems);
             SaveChangesAsync();
        }

        public async Task<Cart> OrdetingCart()
        {
            return await _context.CartItems.OrderByDescending(c => c.Id).FirstOrDefaultAsync();
        }

        public async Task<List<Cart>> GetCartItemsByUserAndDate(string userId, string type, DateTime? selectedDate)
        {
            var cartItemsQuery = _context.CartItems
                .Include(c => c.CartRegister)
                .Include(c => c.Product)
                .Where(c => c.CartRegister.UserProductId == userId && c.TypeCart == type);

            if (selectedDate.HasValue)
            {
                var dateToFilter = selectedDate.Value.Date;
                cartItemsQuery = cartItemsQuery.Where(c => c.CartRegister.CartRegisterDateTime.HasValue && c.CartRegister.CartRegisterDateTime.Value.Date == dateToFilter);
            }

            cartItemsQuery = cartItemsQuery.OrderBy(c => c.CartRegister.CartRegisterDateTime);

            return await cartItemsQuery.ToListAsync();
        }



    }
}
