using AscomPayPG.Models.DTO;
using DB_MODALS.Entities;
using Microsoft.EntityFrameworkCore;

namespace AscomPayPG.Models;

public class AppdbContext : DbContext
{
	public virtual DbSet<Account> Accounts { get; set; }

	public virtual DbSet<ApiEndpoint> ApiEndpoints { get; set; }

	public virtual DbSet<EnKey> EnKeys { get; set; }

	public virtual DbSet<EncryptedPayment> EncryptedPayments { get; set; }

	public virtual DbSet<PaymentChannel> PaymentChannels { get; set; }

	public virtual DbSet<Role> Roles { get; set; }

	public virtual DbSet<TransactionModel> Transactions { get; set; }

	public virtual DbSet<TransactionKey> TransactionKeys { get; set; }

	public virtual DbSet<TransactionsLog> TransactionsLogs { get; set; }
	public virtual DbSet<TransactionPin> TransactionPins { get; set; }

	public virtual DbSet<User> Users { get; set; }

	public virtual DbSet<UserActivityHistory> UserActivityHistories { get; set; }

	public virtual DbSet<UserImage> UserImages { get; set; }

	public virtual DbSet<UserKyc> UserKycs { get; set; }

	public virtual DbSet<UserWallet> UserWallets { get; set; }

	public virtual DbSet<OTP> OTPs { get; set; }
    public virtual DbSet<KycCoreModel> KycCores { get; set; }
    public virtual DbSet<FeedbackModel> UserFeedbacks { get; set; }
    public virtual DbSet<TerminalModel> Terminals { get; set; }
    public virtual DbSet<WalletAgentModel> WalletAgents { get; set; }
	public DbSet<Beneficairy> Beneficairies { get; set; }
	public DbSet<Bank> Banks { get; set; }
    public DbSet<TransactionType> TransactionType { get; set; }
    public DbSet<UserPasswordHistory> UserPasswordHistories { get; set; }
    public DbSet<UserDeviceInformation> UserDeviceInformation { get; set; }
    public DbSet<UserDeviceAuthentication> UserDeviceAuthentication { get; set; }

    public AppdbContext()
	{
	}
    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    if (!optionsBuilder.IsConfigured)
    //    {
    //        IConfigurationRoot configuration = new ConfigurationBuilder()

    //           .AddJsonFile("appsettings.json")
    //           .Build();
    //        var connectionString = configuration.GetConnectionString("PolicyConnection");
    //        optionsBuilder.UseSqlServer(connectionString);
    //    }
    //}
    public AppdbContext(DbContextOptions<AppdbContext> options)
		: base(options)
	{
	}

	//protected override void OnModelCreating(ModelBuilder modelBuilder)
	//{
	//	modelBuilder.Entity(delegate(EntityTypeBuilder<Account> entity)
	//	{
	//		entity.HasKey((Account e) => e.AccountId).HasName("PK__Accounts__349DA586C2040977");
	//		entity.HasIndex((Account e) => e.AccountNumber, "UQ__Accounts__BE2ACD6F657CBB84").IsUnique();
	//		entity.Property((Account e) => e.AccountId).HasColumnName("AccountID");
	//		entity.Property((Account e) => e.AccountName).HasMaxLength(200);
	//		entity.Property((Account e) => e.AccountNumber).HasMaxLength(20).IsUnicode(unicode: false);
	//		entity.Property((Account e) => e.AccountOpeningDate).HasColumnType("datetime");
	//		entity.Property((Account e) => e.Bvn).HasMaxLength(20).IsUnicode(unicode: false)
	//			.HasColumnName("BVN");
	//		entity.Property((Account e) => e.CurrentBalance).HasColumnType("decimal(18, 2)");
	//		entity.Property((Account e) => e.DateOfBirth).HasColumnType("date");
	//		entity.Property((Account e) => e.LastUpdated).HasColumnType("datetime");
	//		entity.Property((Account e) => e.Nin).HasMaxLength(20).IsUnicode(unicode: false)
	//			.HasColumnName("NIN");
	//		entity.Property((Account e) => e.PrevioseBalance).HasColumnType("decimal(18, 2)");
	//		entity.Property((Account e) => e.UserId).HasColumnName("UserID");
	//		entity.Property((Account e) => e.UserUid).HasColumnName("UserUID");
	//		entity.HasOne((Account d) => d.User).WithMany((User p) => p.Accounts).HasForeignKey((Account d) => d.UserId)
	//			.HasConstraintName("FK__Accounts__UserID__6B24EA82");
	//	});
	//	modelBuilder.Entity(delegate(EntityTypeBuilder<ApiEndpoint> entity)
	//	{
	//		entity.HasKey((ApiEndpoint e) => e.ApiId).HasName("PK_PaymentEndpoints");
	//		entity.Property((ApiEndpoint e) => e.ApiId).HasColumnName("ApiID");
	//		entity.Property((ApiEndpoint e) => e.AdParameter1).HasMaxLength(1000);
	//		entity.Property((ApiEndpoint e) => e.AdParameter2).HasMaxLength(1000);
	//		entity.Property((ApiEndpoint e) => e.AdParameter3).HasMaxLength(1000);
	//		entity.Property((ApiEndpoint e) => e.AdParameter4).HasMaxLength(1000);
	//		entity.Property((ApiEndpoint e) => e.AdParameter5).HasMaxLength(1000);
	//		entity.Property((ApiEndpoint e) => e.ApiKey).HasMaxLength(1000);
	//		entity.Property((ApiEndpoint e) => e.ApiName).HasMaxLength(20).IsUnicode(unicode: false);
	//		entity.Property((ApiEndpoint e) => e.ApiUser).HasMaxLength(1000);
	//		entity.Property((ApiEndpoint e) => e.EndPoint).HasMaxLength(1000);
	//		entity.Property((ApiEndpoint e) => e.Method).HasMaxLength(20).IsUnicode(unicode: false);
	//	});
	//	modelBuilder.Entity(delegate(EntityTypeBuilder<EnKey> entity)
	//	{
	//		entity.HasKey((EnKey e) => e.Ukey);
	//		entity.ToTable("EnKey");
	//		entity.Property((EnKey e) => e.Ukey).ValueGeneratedNever().HasColumnName("UKey");
	//		entity.Property((EnKey e) => e.EnKey1).HasColumnName("EnKey");
	//		entity.Property((EnKey e) => e.Iv).HasColumnName("IV");
	//	});
	//	modelBuilder.Entity(delegate(EntityTypeBuilder<EncryptedPayment> entity)
	//	{
	//		entity.HasKey((EncryptedPayment e) => e.PaymentId).HasName("PK__Encrypte__9B556A5890AA3962");
	//		entity.Property((EncryptedPayment e) => e.PaymentId).HasColumnName("PaymentID");
	//		entity.Property((EncryptedPayment e) => e.EncryptedData).IsUnicode(unicode: false);
	//		entity.Property((EncryptedPayment e) => e.QrcodeUrl).HasMaxLength(255).IsUnicode(unicode: false)
	//			.HasColumnName("QRCodeURL");
	//		entity.Property((EncryptedPayment e) => e.UserId).HasColumnName("UserID");
	//		entity.HasOne((EncryptedPayment d) => d.User).WithMany((User p) => p.EncryptedPayments).HasForeignKey((EncryptedPayment d) => d.UserId)
	//			.HasConstraintName("FK__Encrypted__UserI__51300E55");
	//	});
	//	modelBuilder.Entity(delegate(EntityTypeBuilder<PaymentChannel> entity)
	//	{
	//		entity.HasKey((PaymentChannel e) => e.ChannelId).HasName("PK__PaymentC__38C3E8F406526ECE");
	//		entity.Property((PaymentChannel e) => e.ChannelId).HasColumnName("ChannelID");
	//		entity.Property((PaymentChannel e) => e.AccountDetails).HasMaxLength(500).IsUnicode(unicode: false);
	//		entity.Property((PaymentChannel e) => e.ChannelName).HasMaxLength(255).IsUnicode(unicode: false);
	//		entity.Property((PaymentChannel e) => e.ChannelType).HasMaxLength(20).IsUnicode(unicode: false);
	//		entity.Property((PaymentChannel e) => e.ChannelUid).HasColumnName("ChannelUID");
	//	});
	//	modelBuilder.Entity(delegate(EntityTypeBuilder<Role> entity)
	//	{
	//		entity.ToTable("Role");
	//		entity.Property((Role e) => e.RoleId).HasColumnName("RoleID");
	//		entity.Property((Role e) => e.RoleName).HasMaxLength(50);
	//		entity.Property((Role e) => e.RoleUid).HasColumnName("RoleUID");
	//	});
	//	modelBuilder.Entity(delegate(EntityTypeBuilder<TransactionModel> entity)
	//	{
	//		entity.HasKey((TransactionModel e) => e.TransactionId).HasName("PK__Transact__55433A4BB4B22EEB");
	//		entity.Property((TransactionModel e) => e.TransactionId).HasColumnName("TransactionID");
	//		entity.Property((TransactionModel e) => e.Amount).HasColumnType("decimal(18, 2)");
	//		entity.Property((TransactionModel e) => e.DestinationAccount).HasMaxLength(255).IsUnicode(unicode: false);
	//		entity.Property((TransactionModel e) => e.SourceAccount).HasMaxLength(255).IsUnicode(unicode: false);
	//		entity.Property((TransactionModel e) => e.Status).HasMaxLength(20).IsUnicode(unicode: false);
	//		entity.Property((TransactionModel e) => e.StatusId).HasColumnName("StatusID");
	//		entity.Property((TransactionModel e) => e.Timestamp).HasColumnType("datetime");
	//		entity.Property((TransactionModel e) => e.TransactionCategory).HasMaxLength(20).IsUnicode(unicode: false);
	//		entity.Property((TransactionModel e) => e.TransactionType).HasMaxLength(20).IsUnicode(unicode: false);
	//		entity.Property((TransactionModel e) => e.UserId).HasColumnName("UserID");
	//		entity.Property((TransactionModel e) => e.UserUid).HasColumnName("UserUID");
	//		entity.HasOne((TransactionModel d) => d.User).WithMany((User p) => p.Transactions).HasForeignKey((TransactionModel d) => d.UserId)
	//			.HasConstraintName("FK__Transacti__UserI__3A81B327");
	//	});
	//	modelBuilder.Entity(delegate(EntityTypeBuilder<TransactionKey> entity)
	//	{
	//		entity.HasKey((TransactionKey e) => e.KeyId);
	//		entity.ToTable("TransactionKey");
	//		entity.Property((TransactionKey e) => e.UserUid).HasColumnName("UserUID");
	//	});
	//	modelBuilder.Entity(delegate(EntityTypeBuilder<TransactionsLog> entity)
	//	{
	//		entity.HasKey((TransactionsLog e) => e.LogId).HasName("PK__Transact__5E5499A8D6FB0D23");
	//		entity.ToTable("TransactionsLog");
	//		entity.Property((TransactionsLog e) => e.LogId).HasColumnName("LogID");
	//		entity.Property((TransactionsLog e) => e.Action).HasMaxLength(20).IsUnicode(unicode: false);
	//		entity.Property((TransactionsLog e) => e.Details).HasColumnType("text");
	//		entity.Property((TransactionsLog e) => e.LogCategory).HasMaxLength(20).IsUnicode(unicode: false);
	//		entity.Property((TransactionsLog e) => e.Timestamp).HasColumnType("datetime");
	//		entity.Property((TransactionsLog e) => e.TransactionId).HasColumnName("TransactionID");
	//		entity.Property((TransactionsLog e) => e.UserId).HasColumnName("UserID");
	//		entity.Property((TransactionsLog e) => e.UserUid).HasColumnName("UserUID");
	//		entity.HasOne((TransactionsLog d) => d.Transaction).WithMany((TransactionModel p) => p.TransactionsLogs).HasForeignKey((TransactionsLog d) => d.TransactionId)
	//			.HasConstraintName("FK__Transacti__Trans__48CFD27E");
	//		entity.HasOne((TransactionsLog d) => d.User).WithMany((User p) => p.TransactionsLogs).HasForeignKey((TransactionsLog d) => d.UserId)
	//			.HasConstraintName("FK__Transacti__UserI__47DBAE45");
	//	});
	//	modelBuilder.Entity(delegate(EntityTypeBuilder<User> entity)
	//	{
	//		entity.HasKey((User e) => e.UserId).HasName("PK__Users__1788CCAC3784AC71");
	//		entity.HasIndex((User e) => e.Email, "UQ__Users__A9D10534454571F8").IsUnique();
	//		entity.Property((User e) => e.UserId).HasColumnName("UserID");
	//		entity.Property((User e) => e.Address).HasMaxLength(500).IsUnicode(unicode: false);
	//		entity.Property((User e) => e.CurrentDevice).HasMaxLength(100).HasColumnName("Current_Device");
	//		entity.Property((User e) => e.CurrentMacAddress).HasColumnName("Current_Mac_Address");
	//		entity.Property((User e) => e.DateOfBirth).HasColumnType("date");
	//		entity.Property((User e) => e.Email).HasMaxLength(255).IsUnicode(unicode: false);
	//		entity.Property((User e) => e.FirstName).HasMaxLength(100).IsUnicode(unicode: false);
	//		entity.Property((User e) => e.Ip).HasMaxLength(50).HasColumnName("IP");
	//		entity.Property((User e) => e.LastName).HasMaxLength(100).IsUnicode(unicode: false);
	//		entity.Property((User e) => e.LoginTimestamp).HasColumnType("datetime");
	//		entity.Property((User e) => e.MiddleName).HasMaxLength(100).IsUnicode(unicode: false);
	//		entity.Property((User e) => e.PhoneNumber).HasMaxLength(20).IsUnicode(unicode: false);
	//		entity.Property((User e) => e.RegistrationDate).HasColumnType("datetime");
	//		entity.Property((User e) => e.ResetPasswordCode).HasMaxLength(100);
	//		entity.Property((User e) => e.RoleId).HasColumnName("RoleID");
	//		entity.Property((User e) => e.UserType).HasMaxLength(20).IsUnicode(unicode: false);
	//		entity.Property((User e) => e.UserUid).HasColumnName("UserUID");
	//		entity.Property((User e) => e.Username).HasMaxLength(200);
	//	});
	//	modelBuilder.Entity(delegate(EntityTypeBuilder<UserActivityHistory> entity)
	//	{
	//		entity.HasKey((UserActivityHistory e) => e.ActivityId).HasName("PK__UserActi__45F4A7F19429FF68");
	//		entity.ToTable("UserActivityHistory");
	//		entity.Property((UserActivityHistory e) => e.ActivityId).HasColumnName("ActivityID");
	//		entity.Property((UserActivityHistory e) => e.ActivityDetails).HasColumnType("text");
	//		entity.Property((UserActivityHistory e) => e.ActivityType).HasMaxLength(50).IsUnicode(unicode: false);
	//		entity.Property((UserActivityHistory e) => e.ActivityUid).HasColumnName("ActivityUID");
	//		entity.Property((UserActivityHistory e) => e.DeviceName).HasMaxLength(100);
	//		entity.Property((UserActivityHistory e) => e.Ip).HasMaxLength(50).HasColumnName("IP");
	//		entity.Property((UserActivityHistory e) => e.MacAddress).HasColumnName("Mac_Address");
	//		entity.Property((UserActivityHistory e) => e.Timestamp).HasColumnType("datetime");
	//		entity.Property((UserActivityHistory e) => e.UserId).HasColumnName("UserID");
	//		entity.Property((UserActivityHistory e) => e.UserUid).HasColumnName("UserUID");
	//		entity.HasOne((UserActivityHistory d) => d.User).WithMany((User p) => p.UserActivityHistories).HasForeignKey((UserActivityHistory d) => d.UserId)
	//			.HasConstraintName("FK__UserActiv__UserI__4BAC3F29");
	//	});
	//	modelBuilder.Entity(delegate(EntityTypeBuilder<UserImage> entity)
	//	{
	//		entity.HasKey((UserImage e) => e.ImageId).HasName("PK__UserImag__7516F4ECA0D6AE2F");
	//		entity.Property((UserImage e) => e.ImageId).HasColumnName("ImageID");
	//		entity.Property((UserImage e) => e.UserId).HasColumnName("UserID");
	//		entity.HasOne((UserImage d) => d.UserUid).WithMany((User p) => p.UserImages).HasForeignKey((UserImage d) => d.UserId)
	//			.HasConstraintName("FK__UserImage__UserI__4222D4EF");
	//	});
	//	modelBuilder.Entity(delegate(EntityTypeBuilder<UserKyc> entity)
	//	{
	//		entity.HasKey((UserKyc e) => e.Kycid).HasName("PK__UserKYC__4ED3C7290BD4517C");
	//		entity.ToTable("UserKYC");
	//		entity.Property((UserKyc e) => e.Kycid).HasColumnName("KYCID");
	//		entity.Property((UserKyc e) => e.DocumentNumber).HasMaxLength(255).IsUnicode(unicode: false);
	//		entity.Property((UserKyc e) => e.DocumentType).HasMaxLength(50).IsUnicode(unicode: false);
	//		entity.Property((UserKyc e) => e.ExpiryDate).HasColumnType("date");
	//		entity.Property((UserKyc e) => e.UserId).HasColumnName("UserID");
	//		entity.Property((UserKyc e) => e.UserUid).HasColumnName("UserUID");
	//		entity.HasOne((UserKyc d) => d.User).WithMany((User p) => p.UserKycs).HasForeignKey((UserKyc d) => d.UserId)
	//			.HasConstraintName("FK__UserKYC__UserID__44FF419A");
	//	});
	//	modelBuilder.Entity(delegate(EntityTypeBuilder<UserWallet> entity)
	//	{
	//		entity.HasKey((UserWallet e) => e.WalletId).HasName("PK__UserWall__84D4F92E698A0F26");
	//		entity.Property((UserWallet e) => e.WalletId).HasColumnName("WalletID");
	//		entity.Property((UserWallet e) => e.CurrentBalance).HasColumnType("decimal(18, 2)");
	//		entity.Property((UserWallet e) => e.LastUpdated).HasColumnType("datetime");
	//		entity.Property((UserWallet e) => e.PrevioseBalance).HasColumnType("decimal(18, 2)");
	//		entity.Property((UserWallet e) => e.UserId).HasColumnName("UserID");
	//		entity.Property((UserWallet e) => e.UserUid).HasColumnName("UserUID");
	//		entity.Property((UserWallet e) => e.WalletName).HasMaxLength(20).IsUnicode(unicode: false);
	//		entity.Property((UserWallet e) => e.WalletType).HasMaxLength(20).IsUnicode(unicode: false);
	//		entity.HasOne((UserWallet d) => d.UserUid).WithMany((User p) => p.UserWallets).HasForeignKey((UserWallet d) => d.UserId)
	//			.HasConstraintName("FK__UserWalle__UserI__3D5E1FD2");
	//	});
	//}
}
