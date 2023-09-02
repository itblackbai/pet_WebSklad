using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using webSklad.Data;
using webSklad.Interfaces;
using webSklad.Models;
using static System.Net.WebRequestMethods;

namespace webSklad.Repository
{
    public class InvoiceRepository : IInvoiceRepository
    {
       
        private readonly WebSkladContext _context;

        public InvoiceRepository (WebSkladContext context)
        {
            
            _context = context;
        }
        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }

        public async Task<bool> UpdateInvoice(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
            return await Save();
        }
    }
}
