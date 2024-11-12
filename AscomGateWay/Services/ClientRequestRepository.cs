using AscomPayPG.Data;
using AscomPayPG.Data.Enum;
using AscomPayPG.Models;
using AscomPayPG.Models.DTO;
using AscomPayPG.Models.Shared;
using Microsoft.EntityFrameworkCore;

namespace AscomPayPG.Services
{
    public class ClientRequestRepository : IClientRequestRepository<ClientRequest>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ClientRequest> _logger;
        private readonly IConfiguration _configuration;

        public ClientRequestRepository(
            ILogger<ClientRequest> logger,
            AppDbContext context,
            IConfiguration configuration
        )
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }
        public async Task<ClientRequest> Create(ClientRequest item)
        {
            try
            {
                _context.Entry<ClientRequest>(item).State = EntityState.Added;
                await _context.SaveChangesAsync();
               
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
            }

            return item;
        }

        public async Task<bool> Delete(long itemId)
        {
            var item = await GetOne(itemId);
            if (item == null) return false;
            _context.ClientRequests.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ClientRequest>> GetAll()
        {
            var all = await _context.ClientRequests.OrderByDescending(x => x.CreatedAt).ToListAsync();
            return all;
        }

        public async Task<User> GetUser(string uid)
        {
            var a = await _context.Users.Where(x => x.UserUid == Guid.Parse(uid)).FirstOrDefaultAsync();
            return a;
        }

        public async Task<ClientRequest> GetOne(long itemId)
        {
            var item = await _context.ClientRequests.FirstOrDefaultAsync(x => x.ClientRequestId == itemId);
            return item;
        }

        public async Task<bool> Update(ClientRequest item, long itemId)
        {
            var dbItem = await GetOne(itemId);
            if (dbItem == null) return false;
            _context.Entry<ClientRequest>(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ClientRequestResponse> GetPaginatedAll(int page = 1)
        {
            var result = new ClientRequestResponse();
            var pageSize = Convert.ToInt32(_configuration["App:QueryPageSize"]);

            var totalTransactionsCount = await _context.Transactions.CountAsync();

            result.Page = page--;
            result.PageSize = pageSize;
            result.TotalPages = totalTransactionsCount > pageSize ? (int)totalTransactionsCount / pageSize : totalTransactionsCount;
            result.Transactions = await _context.ClientRequests.OrderByDescending(x => x.CreatedAt)
                                                        .Skip(page * pageSize).Take(pageSize)
                                                        .ToListAsync();
            return result;
        }

        public async Task<ClientRequest> GetClientReference(string reference)
        {
            var item = await _context.ClientRequests.FirstOrDefaultAsync(x => x.Reference == reference);
            return item;
        }

        public async Task<Account> GetUserAccount(string destination)
                 => await _context.Accounts.FirstOrDefaultAsync(x => x.AccountNumber == destination || x.AccountId == Convert.ToInt64(destination));

        public async Task<Account> GetAccount(string destination)
                => await _context.Accounts.FirstOrDefaultAsync(x => x.AccountNumber == destination);

        public async Task<Account> GetUserAccountByUserUid(string userUid)
                => await _context.Accounts.FirstOrDefaultAsync(x => x.UserUid == Guid.Parse(userUid));

        public async Task<UserWallet> GetUserWallet(string destination)
        {
            var a = await _context.UserWallets.Where(x => x.WalletUID == Guid.Parse(destination)).FirstOrDefaultAsync();
            return a;
        }

        public async Task<UserWallet> GetWalletById(string Id) 
            => await _context.UserWallets.FirstOrDefaultAsync(x => x.WalletUID.ToString() == Id);

        public async Task<bool> UpdateWallet(UserWallet model)
        {
            _context.UserWallets.Update(model);
            return _context.SaveChanges()> 0;
        }

        public async Task<bool> UpdateExternalWallet(UserExternalWallet model)
        {
            _context.UserExternalWallets.Update(model);
            return _context.SaveChanges() > 0;
        }

        public async Task<bool> UpdateAccount(Account model)
        {
            _context.Accounts.Update(model);
            return _context.SaveChanges() > 0;
        }

        public async Task<bool> AddTransaction(Transactions model)
        {
            _context.Transactions.Update(model);
            return _context.SaveChanges() > 0;
        }

        public async Task<bool> RegisterTransaction(
                                                  decimal netAmount,
                                                  decimal providerCharges,
                                                  decimal marchantCharges,
                                                  string recieverName,
                                                  User sender,
                                                  string transactionReference,
                                                  decimal amount,
                                                  string decription,
                                                  string trasactionType,
                                                  decimal vat,
                                                  decimal charges,
                                                  string provider,
                                                  string senderAccount = "",
                                                  string recieverAccount = "",
                                                  string senderWallet = "",
                                                  string recieverWallet = "",
                                                  bool setAsCompleted = false
                                                  )
        {
            var newTransaction = new Transactions()
            {
                UserId = sender != null ? sender.UserId : null,
                RequestTransactionId = transactionReference,
                UserUID = sender != null ? sender.UserUid : null,
                Status = setAsCompleted ?   PaymentStatus.Completed.ToString() : PaymentStatus.Pending.ToString(),
                StatusId = 1,
                Timestamp = DateTime.Now,
                AccessToken = Guid.NewGuid().ToString(),
                Amount = amount,
                Email = sender != null ? sender.Email : "",
                Description = decription,
                SourceAccount = senderAccount,
                DestinationAccount = recieverAccount,
                SourceWallet = senderWallet,
                DestinationWallet = recieverWallet,
                TransactionType = trasactionType,
                PaymentAction = provider,
                PaymentProvider = provider,
                BankCode = (int)BankCodes.Ascom,
                Currency = "NGN",
                T_Vat = vat,
                T_Charge = charges,
                SenderName = sender != null ? $"{sender.FirstName} " +
                $"{sender.MiddleName} {sender.LastName}" : string.Empty,
                RecieverName =  recieverName,
                NetAmount= netAmount,
                T_Marchant_Charges= marchantCharges,
                T_Provider_Charges= providerCharges,
            };

            try
            {
                 await _context.Transactions.AddAsync(newTransaction);
                return await _context.SaveChangesAsync() > 0;

            }
            catch (Exception EX)
            {

                throw;
            }
        }

        public async Task<bool> UpdateTransactionStatusByReference(string referecneId, string newStatus)
        {
            try
            {
                var transaction = await _context.Transactions.FirstOrDefaultAsync(trans => trans.RequestTransactionId == referecneId);
                if (transaction == null)
                    return false;
                transaction.Status = newStatus;
                transaction.UpdatedAt = DateTime.Now;
                 _context.Transactions.Update(transaction);
                if (await _context.SaveChangesAsync() > 0)
                    return true;
                return false;

            }
            catch (Exception EX)
            {

                throw;
            }
        }
    }
}