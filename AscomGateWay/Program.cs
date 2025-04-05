using System.Text;
using AscomPayPG.Data;
using AscomPayPG.Data.Repository.Implementation;
using AscomPayPG.Data.Repository.Interface;
using AscomPayPG.Helpers;
using AscomPayPG.Models;
using AscomPayPG.Services;
using AscomPayPG.Services.Filters;
using AscomPayPG.Services.Gateways;
using AscomPayPG.Services.Gateways.Implementation;
using AscomPayPG.Services.Gateways.Interface;
using AscomPayPG.Services.Implementation;
using AscomPayPG.Services.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using transacrtionSettlement;

try
{
    var builder = WebApplication.CreateBuilder(args);

    var logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(builder.Configuration)
                    .Enrich.FromLogContext()
                      .WriteTo.File("C:\\AscompPay_PG_LOGS\\", rollingInterval: RollingInterval.Day)
                    .CreateLogger();

    ConfigurationHelper.InstantaiteConfiguration(builder.Configuration);
    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog(logger);


    //AppDB service
    var timeout = Convert.ToInt32(builder.Configuration["App:DBTimeOutInMinutes"]);

    Guid Token = Guid.Parse(builder.Configuration["App:decrpt:TokenCon"]);

    EncodeValue encode = new EncodeValue();

    var connection = await encode.decrypt(builder.Configuration.GetConnectionString("AppConnectionString"),1, Token);
    if(connection.isOk == false) throw new Exception($"Connection Failed : {connection.Message}");
//    var connection = builder.Configuration.GetConnectionString("AppConnectionString");

   /* builder.Services.AddDbContextPool<AppDbContext>(options =>
            options.UseSqlServer(connection.ToString(),
            opts => opts.CommandTimeout(timeout)
     ), (int)ServiceLifetime.Scoped);*/

    /*var connectionLog = await encode.decrypt(builder.Configuration.GetConnectionString("AppLogConnectionString"), 0, Token);

    if (connection.isOk == false) throw new Exception($"Connection Failed : {connection.Message}");*/

    builder.Services.AddDbContextPool<AppLogDBContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString(connection.Message),
            opts => opts.CommandTimeout(timeout)
     ), (int)ServiceLifetime.Scoped);

    builder.Services.AddHttpContextAccessor();

    builder.Services.AddSession(options =>
    {
        // Set session timeout to 30 minutes
        options.IdleTimeout = TimeSpan.FromMinutes(30);
    });

    // Add services to the container.
    builder.Services.AddScoped<IHelperService, HelperService>();
    builder.Services.AddScoped<IGTPay, GTPay>();
    builder.Services.AddScoped<IPaystack, Paystack>();
    builder.Services.AddScoped<IPaymentProcessor, PaymentProcessor>();
    builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    builder.Services.AddScoped<IEmailNotification, EmailNotification>();
    builder.Services.AddScoped<IWalletService, WalletService>();
    builder.Services.AddScoped<ITransactionHelper, TransactionHelper>();
    builder.Services.AddScoped<ISMSNotification, SMSNotification>();
    builder.Services.AddScoped<ITransactionService, TransactionService>();
    builder.Services.AddScoped<IOfflineSettlementService, OfflineSettlementService>();

    //add repositories
    builder.Services.AddScoped<ITransactionsRepository<Transactions>, TransactionsRepository>();
    builder.Services.AddScoped<IRepository<PaymentGateway>, PaymentGatewayRepository>();
    builder.Services.AddScoped<IExternalIntegrationLogRepository, ExternalIntegrationLogRepository>();
    builder.Services.AddScoped<IClientRequestRepository<ClientRequest>, ClientRequestRepository>();
    builder.Services.AddTransient<IAccountService, AccountService>();
    builder.Services.AddTransient<IEncodeValue, EncodeValue>();
    builder.Services.AddTransient<INotificationRepository, NotificationRepository>();
    builder.Services.AddTransient<IVasService, VasService>();
    builder.Services.AddTransient<I9psbVaS, Vas9PSB>();
    builder.Services.AddScoped<InternalTransactionValidator>(); // Or AddTransient based on your need
    builder.Services.AddScoped<ExternalTransactionValidator>(); // Or AddTransient based on your need
    builder.Services.AddScoped<VasTransactionValidator>(); // Or AddTransient based on your need



    // Add services to the container.
    builder.Services.AddControllersWithViews();
    builder.Services.AddControllers();

    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Ascom Payment Gateway/SERVICE API",
            Version = "v1",
            Description = "API (.NET 6)",
            Contact = new OpenApiContact()
            {
                Name = "Ascom Pay solution",
                Url = null,
                Email = ""
            }
        });

        options.DocumentFilter<CustomSwaggerFilter>();

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Name = "Authorization",
            Description = "Bearer Authentication with JWT Token",
            Type = SecuritySchemeType.Http
        });

        options.OperationFilter<SecurityRequirementsOperationFilter>();

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
      {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Id = "Bearer",
                Type = ReferenceType.SecurityScheme
            }
        },
        new List<string>()
      }
        });
    });

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateActor = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });



    builder.Services.AddAuthorization();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddRazorPages();
    builder.Services.AddMvcCore();

    //var devCorsPolicy = "devCorsPolicy";
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(builder =>
        {
            //builder.WithOrigins("http://localhost:800").AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            //builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost");
            //builder.SetIsOriginAllowed(origin => true);
        });
    });

    var app = builder.Build();

    app.UseMiddleware<SwaggerUrlProtectorMiddleware>();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseAuthorization();
        app.UseAuthentication();
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        app.UseSwagger();
        app.UseAuthorization();
        app.UseAuthentication();
        app.UseSwaggerUI();
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }



    app.UseStaticFiles();

    app.UseHttpsRedirection();

    app.UseAuthentication();

    app.UseRouting();
    app.UseCors();

    app.UseAuthorization();
    app.UseSession();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapRazorPages();
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
    });

    app.UseCookiePolicy();

    app.Run();
}
catch (Exception ex)
{

    throw;
}
