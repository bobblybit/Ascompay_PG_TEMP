using AscomPayPG.Models;
using AscomPayPG.Models.DTO;
using Microsoft.EntityFrameworkCore;
namespace AscomPayPG.Data
{

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Transactions>()
                    .Property(t => t.Amount)
                    .HasColumnType("decimal(18,2)");

            base.OnModelCreating(builder);
        }

        public DbSet<Transactions> Transactions { get; set; }
        public DbSet<TransactionType> TransactionType { get; set; }
        public DbSet<PaymentGateway> PaymentGateways { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ClientRequest> ClientRequests { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<UserWallet> UserWallets { get; set; }
         public DbSet<Bank> Banks { get; set; }
        public DbSet<Webhook> Webhook { get; set; }
        public DbSet<UserExternalWallet> UserExternalWallets { get; set; }

        //public virtual DbSet<TransactionKey> TransactionKeys { get; set; }

        //public virtual DbSet<TransactionsLog> TransactionsLogs { get; set; }
        //public virtual DbSet<TransactionPin> TransactionPins { get; set; }


        //public virtual DbSet<UserActivityHistory> UserActivityHistories { get; set; }

        //public virtual DbSet<UserImage> UserImages { get; set; }

        public DbSet<UserKyc> UserKycs { get; set; }

        //public  DbSet<OTP> OTPs { get; set; }
        //public  DbSet<TransactionToken> TransactionToken { get; set; }
        //public  DbSet<KycCoreModel> KycCores { get; set; }
        //public  DbSet<FeedbackModel> UserFeedbacks { get; set; }
        //public  DbSet<TerminalModel> Terminals { get; set; }
        //public  DbSet<WalletAgentModel> WalletAgents { get; set; }
        //public DbSet<Beneficairy> Beneficairies { get; set; }
        //public DbSet<WalletType> WalletTypes { get; set; }
        //public DbSet<UserPasswordHistory> UserPasswordHistories { get; set; }
        public DbSet<ExternalIntegrationLog> ExternalIntegrationLogs { get; set; }

        public DbSet<UserDeviceInformation> UserDeviceInformation { get; set; }
        public DbSet<UserDeviceAuthentication> UserDeviceAuthentication { get; set; }


    }
}