using webSklad.Models;

namespace webSklad.Interfaces
{
    public interface ICartRepository
    {
        Task<int> GetLastCartRegisterId();

        public void AddCartItem(Cart cartItem);
        public void SaveChangesAsync();

        Task<List<IGrouping<int, Cart>>> GetCartItemsForUser(string userId, string type);

        Task<List<IGrouping<int, Cart>>> GetCartRegisterForUser(string userId, string type);

        Task DeleteCartRange ( Cart cart );

        Task DeleteCartRange(List<Cart> cartItems);

        Task<List<Cart>> GetCartItemsByCartId(int cartId);

        Task<Cart> OrdetingCart();

        Task<List<Cart>> GetCartItemsByUserAndDate(string userId, string type, DateTime? selectedDate);



    }
}
