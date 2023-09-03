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

        public async Task<Invoice> GetInvoiceByIdAsync(int invoiceId, string userId)
        {
            return await _context.Invoices
                .Include(i => i.PostInfo)
                .Include(i => i.PostFop)
                .Include(i => i.PostSR)
                .Include(i => i.ShopInfo)
                .Include(i => i.ShopFop)
                .FirstOrDefaultAsync(i => i.Id == invoiceId && i.UserInvoiceId == userId);
        }


        public async Task<Invoice> GetInvoiceOrderByIdAsync(string userId)
        {
            return await _context.Invoices
                .Include(i => i.PostInfo)
                .Include(i => i.PostFop)
                .Include(i => i.PostSR)
                .Include(i => i.ShopInfo)
                .Include(i => i.ShopFop)
                .OrderByDescending(i => i.Id)
                .FirstOrDefaultAsync(i => i.UserInvoiceId == userId);
        }


    }
}
