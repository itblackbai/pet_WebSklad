using webSklad.Models;

namespace webSklad.Interfaces
{
    public interface ICartRegisterRepository
    {
        int GetLastCartRegisterId();
        Task<bool> Save();

        Task<bool> AddCartRegister(int? cartRegisterId);

        Task<bool> AddCartRegisterCart(CartRegister cartRegisterId);

        Task<CartRegister> GetCartRegisterById(int cartregisterid, string currentUser);

        Task<CartRegister> GetCartRegisterOrdering( string currentUser);
    }
}
