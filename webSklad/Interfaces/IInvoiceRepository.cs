using webSklad.Models;

namespace webSklad.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<bool> Save();
        Task<bool> UpdateInvoice(Invoice invoice);

        Task<Invoice> GetInvoiceByIdAsync(int invoiceId, string userId);

        Task<Invoice> GetInvoiceOrderByIdAsync(string userId);
    }
}
