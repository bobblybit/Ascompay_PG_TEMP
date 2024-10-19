﻿// <auto-generated />
using System;
using AscomPayPG.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AscomPayPG.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("AscomPayPG.Models.Bank", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"), 1L, 1);

                    b.Property<string>("Bank_name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Banks", (string)null);
                });

            modelBuilder.Entity("AscomPayPG.Models.ClientRequest", b =>
                {
                    b.Property<long>("ClientRequestId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ClientRequestId"), 1L, 1);

                    b.Property<string>("CallbackURL")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Reference")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.Property<string>("StatusDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("Uid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("ClientRequestId");

                    b.ToTable("ClientRequests", (string)null);
                });

            modelBuilder.Entity("AscomPayPG.Models.DTO.Account", b =>
                {
                    b.Property<long>("AccountId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("AccountId"), 1L, 1);

                    b.Property<string>("AccountName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AccountNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("AccountOpeningDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Bvn")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("CurrentBalance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<bool?>("IsPrimary")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<decimal?>("LegerBalance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Nin")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("PrevioseBalance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<long?>("UserId")
                        .HasColumnType("bigint");

                    b.Property<Guid?>("UserUid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("AccountId");

                    b.HasIndex("UserId");

                    b.ToTable("Accounts", (string)null);
                });

            modelBuilder.Entity("AscomPayPG.Models.DTO.TransactionsLog", b =>
                {
                    b.Property<long>("LogId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("LogId"), 1L, 1);

                    b.Property<string>("Action")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Details")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeprecated")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("LogCategory")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Timestamp")
                        .HasColumnType("datetime2");

                    b.Property<long>("TransactionId")
                        .HasColumnType("bigint");

                    b.Property<long?>("UserId")
                        .HasColumnType("bigint");

                    b.Property<Guid?>("UserUid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("LogId");

                    b.HasIndex("TransactionId");

                    b.HasIndex("UserId");

                    b.ToTable("TransactionsLog", (string)null);
                });

            modelBuilder.Entity("AscomPayPG.Models.DTO.TransactionType", b =>
                {
                    b.Property<int>("SN")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SN"), 1L, 1);

                    b.Property<bool>("Add_Vat")
                        .HasColumnType("bit");

                    b.Property<bool>("By_Amount")
                        .HasColumnType("bit");

                    b.Property<bool>("By_Percent")
                        .HasColumnType("bit");

                    b.Property<int?>("T_Action")
                        .HasColumnType("int");

                    b.Property<decimal>("T_Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int?>("T_Channel")
                        .HasColumnType("int");

                    b.Property<decimal>("T_Percentage")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("T_Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("T_Vat")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("TiD")
                        .HasColumnType("int");

                    b.Property<string>("Ttype")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SN");

                    b.ToTable("TransactionType", (string)null);
                });

            modelBuilder.Entity("AscomPayPG.Models.DTO.User", b =>
                {
                    b.Property<long>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("UserId"), 1L, 1);

                    b.Property<int?>("Accountstatus")
                        .HasColumnType("int");

                    b.Property<Guid?>("ActivationCode")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CompanyCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CurrentDevice")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CurrentMacAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Ip")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsAgent")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeprecated")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsEmailVerified")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsKycVerified")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsNotificationEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<int>("LoginAttempted")
                        .HasColumnType("int");

                    b.Property<DateTime>("LoginTimestamp")
                        .HasColumnType("datetime2");

                    b.Property<string>("MiddleName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("OnlineStaus")
                        .HasColumnType("bit");

                    b.Property<byte[]>("PasswordHash")
                        .HasColumnType("varbinary(max)");

                    b.Property<byte[]>("PasswordSalt")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("RegistrationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ResetPasswordCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("RoleId")
                        .HasColumnType("int");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UserUid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("AscomPayPG.Models.DTO.UserExternalWallet", b =>
                {
                    b.Property<long>("ExternalWalletId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ExternalWalletId"), 1L, 1);

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeprecated")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserUId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("availableBalance")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("bvn")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("firstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("freezeStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("lastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ledgerBalance")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("lienStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("maximumBalance")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("maximumDeposit")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("nuban")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("number")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("phoneNo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("pndstatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("productCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("tier")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("totalWalletBalance")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ExternalWalletId");

                    b.ToTable("UserExternalWallets", (string)null);
                });

            modelBuilder.Entity("AscomPayPG.Models.DTO.UserWallet", b =>
                {
                    b.Property<long>("WalletId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("WalletId"), 1L, 1);

                    b.Property<decimal?>("CurrentBalance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeprecated")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<decimal?>("PrevioseBalance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<Guid?>("UserUid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("WalletName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("WalletTypeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("WalletUID")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("WalletId");

                    b.HasIndex("UserId");

                    b.ToTable("UserWallets", (string)null);
                });

            modelBuilder.Entity("AscomPayPG.Models.ExternalIntegrationLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<string>("RequestPayload")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("RequestTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Response")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ResponseTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Service")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Vendor")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ExternalIntegrationLogs", (string)null);
                });

            modelBuilder.Entity("AscomPayPG.Models.PaymentGateway", b =>
                {
                    b.Property<long>("PaymentGatewayId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("PaymentGatewayId"), 1L, 1);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PayUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("QueryUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("PaymentGatewayId");

                    b.ToTable("PaymentGateways", (string)null);
                });

            modelBuilder.Entity("AscomPayPG.Models.Transactions", b =>
                {
                    b.Property<long>("TransactionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("TransactionId"), 1L, 1);

                    b.Property<string>("AccessToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("BankCode")
                        .HasColumnType("int");

                    b.Property<string>("CallbackURL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DestinationAccount")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DestinationWallet")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PaymentAction")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("PaymentGatewayId")
                        .HasColumnType("bigint");

                    b.Property<string>("RequestTransactionId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SourceAccount")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SourceWallet")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("StatusId")
                        .HasColumnType("int");

                    b.Property<decimal>("T_Charge")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("T_Vat")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.Property<string>("TransactionCategory")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("TransactionStatus")
                        .HasColumnType("bit");

                    b.Property<string>("TransactionType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<long?>("UserId")
                        .HasColumnType("bigint");

                    b.Property<Guid?>("UserUID")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("TransactionId");

                    b.HasIndex("PaymentGatewayId");

                    b.ToTable("Transactions", (string)null);
                });

            modelBuilder.Entity("AscomPayPG.Models.UserDeviceAuthentication", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AuthenticationKey")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeprecated")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserDeviceAuthentication", (string)null);
                });

            modelBuilder.Entity("AscomPayPG.Models.UserDeviceInformation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<string>("DeviceModel")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IpAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeprecated")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPrimaryDevice")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("MacAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserDeviceInformation", (string)null);
                });

            modelBuilder.Entity("AscomPayPG.Models.UserKyc", b =>
                {
                    b.Property<long>("Kycid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Kycid"), 1L, 1);

                    b.Property<string>("BackImage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CompanyName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<string>("DocumentNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DocumentType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ExpiryDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("FrontImage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("IsVerified")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("SelfieImage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("UserId")
                        .HasColumnType("bigint");

                    b.Property<Guid?>("UserUid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Kycid");

                    b.ToTable("UserKycs", (string)null);
                });

            modelBuilder.Entity("AscomPayPG.Models.Webhook", b =>
                {
                    b.Property<long>("WebhookId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("WebhookId"), 1L, 1);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("EventType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Reference")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RequestString")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Service")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("Uid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Vendor")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("WebhookId");

                    b.ToTable("Webhook", (string)null);
                });

            modelBuilder.Entity("TransactionsUser", b =>
                {
                    b.Property<long>("TransactionsTransactionId")
                        .HasColumnType("bigint");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("TransactionsTransactionId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("TransactionsUser", (string)null);
                });

            modelBuilder.Entity("AscomPayPG.Models.DTO.Account", b =>
                {
                    b.HasOne("AscomPayPG.Models.DTO.User", null)
                        .WithMany("Accounts")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("AscomPayPG.Models.DTO.TransactionsLog", b =>
                {
                    b.HasOne("AscomPayPG.Models.Transactions", "Transaction")
                        .WithMany("TransactionsLogs")
                        .HasForeignKey("TransactionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AscomPayPG.Models.DTO.User", null)
                        .WithMany("TransactionsLogs")
                        .HasForeignKey("UserId");

                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("AscomPayPG.Models.DTO.UserWallet", b =>
                {
                    b.HasOne("AscomPayPG.Models.DTO.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("AscomPayPG.Models.Transactions", b =>
                {
                    b.HasOne("AscomPayPG.Models.PaymentGateway", "PaymentGateway")
                        .WithMany()
                        .HasForeignKey("PaymentGatewayId");

                    b.Navigation("PaymentGateway");
                });

            modelBuilder.Entity("AscomPayPG.Models.UserDeviceAuthentication", b =>
                {
                    b.HasOne("AscomPayPG.Models.DTO.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("AscomPayPG.Models.UserDeviceInformation", b =>
                {
                    b.HasOne("AscomPayPG.Models.DTO.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("TransactionsUser", b =>
                {
                    b.HasOne("AscomPayPG.Models.Transactions", null)
                        .WithMany()
                        .HasForeignKey("TransactionsTransactionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AscomPayPG.Models.DTO.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AscomPayPG.Models.DTO.User", b =>
                {
                    b.Navigation("Accounts");

                    b.Navigation("TransactionsLogs");
                });

            modelBuilder.Entity("AscomPayPG.Models.Transactions", b =>
                {
                    b.Navigation("TransactionsLogs");
                });
#pragma warning restore 612, 618
        }
    }
}
