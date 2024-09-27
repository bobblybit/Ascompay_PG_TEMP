﻿// <auto-generated />
using System;
using AscomPayPG.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AscomPayPG.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20220819163314_InitDB")]
    partial class InitDB
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

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

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("ClientRequestId");

                    b.ToTable("ClientRequests");
                });

            modelBuilder.Entity("AscomPayPG.Models.PaymentGateway", b =>
                {
                    b.Property<long>("PaymentGatewayId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("PaymentGatewayId"), 1L, 1);

                    b.Property<string>("CallbackURL")
                        .HasColumnType("nvarchar(max)");

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

                    b.ToTable("PaymentGateways");
                });

            modelBuilder.Entity("AscomPayPG.Models.Transactions", b =>
                {
                    b.Property<long>("TransactionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("TransactionId"), 1L, 1);

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("CallbackURL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PaymentAction")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("PaymentGatewayId")
                        .HasColumnType("bigint");

                    b.Property<string>("RequestTransactionId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.Property<string>("StatusDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("TransactionId");

                    b.HasIndex("PaymentGatewayId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("AscomPayPG.Models.Transactions", b =>
                {
                    b.HasOne("AscomPayPG.Models.PaymentGateway", "PaymentGateway")
                        .WithMany()
                        .HasForeignKey("PaymentGatewayId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PaymentGateway");
                });
#pragma warning restore 612, 618
        }
    }
}