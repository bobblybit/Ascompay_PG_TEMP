using AscomPayPG.Data;
using AscomPayPG.Models;
using AscomPayPG.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Polly;
using System;

namespace AscomPayPG.Services
{
    public class TransactionsRepository : ITransactionsRepository<Transactions>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<Transactions> _logger;
        private readonly IConfiguration _configuration;

        public TransactionsRepository(
            ILogger<Transactions> logger,
            AppDbContext context,
            IConfiguration configuration
        )
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }
        public async Task<Transactions> Create(Transactions item)
        {
            try
            {
                _context.Entry<Transactions>(item).State = EntityState.Added;
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex) { Console.WriteLine(ex); return null; }
        }

        public async Task<Tuple<bool, string, string>> UpdateUserAccount(long itemId)
        {

            bool result = false; // Replace with your actual result
            string destination = "Not Found"; // Replace with your actual destination
            string Message = string.Empty;

            try
            {

                var dbItem = await GetOne(itemId);
                if (dbItem == null) throw new Exception($"Invalid Transaction");

                var checkTransactionType = _context.TransactionType.FirstOrDefault(a => a.Ttype == dbItem.TransactionType);
                if (checkTransactionType == null) throw new Exception($"Invalid Transaction type {dbItem.TransactionType}");

                if (checkTransactionType.TiD == 1)
                {
                    var checkDestinationAccount = _context.Accounts.FirstOrDefault(a => a.AccountNumber == dbItem.DestinationAccount);
                    if (checkDestinationAccount == null) throw new Exception($"Invalid Destination Account Number {dbItem.DestinationAccount}");


                    checkDestinationAccount.PrevioseBalance = checkDestinationAccount.CurrentBalance;
                    checkDestinationAccount.CurrentBalance += dbItem.Amount;
                    checkDestinationAccount.LastUpdated = DateTime.Now;

                    _context.Attach(checkDestinationAccount);
                    _context.Accounts.Update(checkDestinationAccount);
                    var Isupdate = await _context.SaveChangesAsync();
                    if (Isupdate > 0)
                    {
                        result = true;
                        destination = dbItem.DestinationAccount;
                        Message = $"Transaction of {dbItem.Amount} to {dbItem.DestinationAccount} was Successfull";
                    }
                }
                else if (checkTransactionType.TiD == 2)
                {
                    Guid guid;
                    var walletUID = Guid.TryParse(dbItem.DestinationWallet, out guid) == true  ? Guid.Parse(dbItem.DestinationWallet) : Guid.NewGuid();
                    var checkDestinationWallet = _context.UserWallets.FirstOrDefault(a => a.WalletUID == walletUID);
                    if (checkDestinationWallet == null) throw new Exception($"Invalid Destination Wallet {dbItem.DestinationWallet}");


                    checkDestinationWallet.PrevioseBalance = checkDestinationWallet.CurrentBalance;
                    checkDestinationWallet.CurrentBalance += dbItem.Amount;
                    checkDestinationWallet.LastUpdated = DateTime.Now;


                    _context.Attach(checkDestinationWallet);
                    _context.UserWallets.Update(checkDestinationWallet);
                    var Isupdate = await _context.SaveChangesAsync();
                    if (Isupdate > 0)
                    {
                        result = true;
                        destination = dbItem.DestinationWallet;
                        Message = $"Transaction of {dbItem.Amount} to User Wallet {dbItem.DestinationWallet} was Successfull";
                    }
                }
                else { throw new Exception($"Invalid Transaction type"); }

            }
            catch (Exception ex) { Console.WriteLine(ex);}


            return Tuple.Create(result, destination, Message);
        }

        public async Task<bool> Delete(long itemId)
        {
            try
            {
                var item = await GetOne(itemId);
                if (item == null) return false;
                _context.Transactions.Remove(item);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) { Console.WriteLine(ex); return false; }
        }

        public async Task<IEnumerable<Transactions>> GetAll()
        {            
            try
            {
                var all = await _context.Transactions
                                    .Include(x => x.PaymentGateway)
                                    .OrderByDescending(x => x.Timestamp)
                                    .ToListAsync();
                return all;
            }
            catch (Exception ex) { Console.WriteLine(ex); return null; }
        }

        public async Task<Transactions> GetOne(long itemId)
        {            
            try
            {
                var item = await _context.Transactions
                                     .Include(x => x.PaymentGateway)
                                     .Where(x => x.TransactionId == itemId).FirstOrDefaultAsync();
                return item;
            }
            catch (Exception ex) { Console.WriteLine(ex); return null; }
        }


        public async Task<TransactionsResponse> GetPaginatedAll(int page = 1)
        {  
            try
            {
                var result = new TransactionsResponse();
                var pageSize = Convert.ToInt32(_configuration["App:QueryPageSize"]);

                _context.Database.OpenConnection();

                var totalTransactionsCount = await _context.Transactions.CountAsync();
                result.Page = page--;
                result.PageSize = pageSize;
                result.TotalPages = totalTransactionsCount > pageSize ? (int)totalTransactionsCount / pageSize : totalTransactionsCount;

                result.Transactions = await _context.Transactions.Include(x => x.PaymentGateway).OrderByDescending(x => x.Timestamp)
                                                            .Skip(page * pageSize).Take(pageSize)
                                                            .ToListAsync();
                return result;
            }
            catch (Exception ex) { Console.WriteLine(ex); return null; }
        }

        public async Task<TransactionsResponse> GetPaginatedAll(string uid, int page = 1)
        {
            try
            {
                var result = new TransactionsResponse();
                var pageSize = Convert.ToInt32(_configuration["App:QueryPageSize"]);

                _context.Database.OpenConnection();

                var totalTransactionsCount = await _context.Transactions.CountAsync();
                result.Page = page--;
                result.PageSize = pageSize;
                result.TotalPages = totalTransactionsCount > pageSize ? (int)totalTransactionsCount / pageSize : totalTransactionsCount;

                result.Transactions = await _context.Transactions.Where(a => a.UserUID == Guid.Parse(uid)).Include(x => x.PaymentGateway).OrderByDescending(x => x.Timestamp)
                                                            .Skip(page * pageSize).Take(pageSize)
                                                            .ToListAsync();
                return result;
            }
            catch (Exception ex) { Console.WriteLine(ex); return null; }
        }

        public async Task<Transactions> GetPayRequest(string requestId)
        {
            try
            {
                var item = await _context.Transactions.Include(x => x.PaymentGateway).FirstOrDefaultAsync(x => x.RequestTransactionId == requestId);
                return item;
            }
            catch (Exception ex) { Console.WriteLine(ex); return null; }
        }


        public async Task<bool> IsRequestTransactionIdAvailable(string requestId)
        {          

            try
            {
                var item = await _context.Transactions.Include(x => x.PaymentGateway).FirstOrDefaultAsync(x => x.RequestTransactionId == requestId);
                return item == null ? true : false;
            }
            catch (Exception ex) { Console.WriteLine(ex); return false; }
        }

        public async Task<bool> checkIfAccountExits(string accountNumber)
        {
            try
            {
                var item = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountNumber == accountNumber);
                return item == null ? true : false;
            }
            catch (Exception ex) { Console.WriteLine(ex); return false; }
        }

        public async Task<bool> Update(Transactions item, long itemId)
        {
            try
            {

                var dbItem = await GetOne(itemId);
                if (dbItem == null) return false;
                //_context.Entry<Transactions>(item).State = EntityState.Modified;
                _context.Attach(item);
                _context.Transactions.Update(item);
                var Isupdate = await _context.SaveChangesAsync();
                if (Isupdate > 0)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex) { Console.WriteLine(ex); return false; }
        }
    }
}