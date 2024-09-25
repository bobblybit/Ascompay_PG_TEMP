using AscomPayPG.Data;
using AscomPayPG.Models;
using Microsoft.EntityFrameworkCore;

namespace AscomPayPG.Services
{
    public class PaymentGatewayRepository : IRepository<PaymentGateway>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PaymentGateway> _logger;

        public PaymentGatewayRepository(
            ILogger<PaymentGateway> logger,
            AppDbContext context
        )
        {
            _context = context;
            _logger = logger;
        }
        public async Task<PaymentGateway> Create(PaymentGateway item)
        {
            _context.Entry<PaymentGateway>(item).State = EntityState.Added;
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<bool> Delete(long itemId)
        {
            var item = await GetOne(itemId);
            if (item == null) return false;
            _context.PaymentGateways.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PaymentGateway>> GetAll()
        {
            try
            {
                _context.Database.OpenConnection();
                var all = await _context.PaymentGateways.OrderByDescending(x => x.CreatedAt).ToListAsync();
                return all;
            }
            catch (Exception ex) { Console.WriteLine(ex); return null; }
        }

        public async Task<PaymentGateway> GetOne(long itemId)
        {
            try
            {
                var item = await _context.PaymentGateways.FirstOrDefaultAsync(x => x.PaymentGatewayId == itemId);
                return item;
            }
            catch (Exception ex) { Console.WriteLine(ex); return null; }
        }

        public async Task<bool> Update(PaymentGateway item, long itemId)
        {
            try
            {

                var dbItem = await GetOne(itemId);
                if (dbItem == null) return false;
                _context.Entry<PaymentGateway>(item).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) { Console.WriteLine(ex); return false; }

        }
    }
}