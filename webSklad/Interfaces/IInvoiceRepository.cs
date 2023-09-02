using webSklad.Models;

namespace webSklad.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<bool> Save();
        Task<bool> UpdateInvoice(Invoice invoice);
    }
}
