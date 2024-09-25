using AscomPayPG.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AscomPayPG.Services
{
    public class SwaggerUrlProtectorMiddleware
    {
        private readonly RequestDelegate _next;
        private IConfiguration _configuration;

        public SwaggerUrlProtectorMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            this._next = next;
            _configuration = configuration;
        }


        public async Task InvokeAsync(HttpContext context)
        {

            var user = _configuration["SwaggerUserSetting:User"];
            var pass = _configuration["SwaggerUserSetting:Pass"];

            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                string authHeader = context.Request.Headers["Authorization"];
                if (authHeader != null && authHeader.StartsWith("Basic "))
                {
                    // Get the credentials from request header
                    var header = AuthenticationHeaderValue.Parse(authHeader);
                    var inBytes = Convert.FromBase64String(header.Parameter);
                    var credentials = Encoding.UTF8.GetString(inBytes).Split(':');
                    var username = credentials[0];
                    var password = credentials[1];
                    // validate credentials
                    if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                    {
                        var login = new ApiUserLogin
                        {
                            Username = username,
                            Password = password
                        };

                        if (login.Username.Equals(user) && login.Password.Equals(pass))
                        {
                            await _next.Invoke(context).ConfigureAwait(false);
                            return;
                        }
                    }
                }
                context.Response.Headers["WWW-Authenticate"] = "Basic";
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else
            {
                await _next.Invoke(context).ConfigureAwait(false);
            }
        }
    }

    #region

    //public async Task Invoke(HttpContext httpContext)
    //{
    //    if (httpContext.Request.Path.Value != null && httpContext.Request.Path.HasValue &&
    //        httpContext.Request.Path.Value.Contains("swagger.json", StringComparison.InvariantCultureIgnoreCase))
    //    {
    //        if (httpContext.User.Identity is { IsAuthenticated: false })
    //        {
    //            var originalStream = httpContext.Response.Body;
    //            using (var memStream = new MemoryStream())
    //            {
    //                //Change default unreadable stream with memory stream to be able to read the response afterwards
    //                httpContext.Response.Body = memStream;
    //                await _next(httpContext);
    //                var response = ProtectResponse(httpContext.Response);
    //                await originalStream.WriteAsync(response);
    //                httpContext.Response.Body = originalStream;
    //                return;
    //            }
    //        }
    //    }

    //    await _next(httpContext);
    //}

    //private byte[] ProtectResponse(HttpResponse response)
    //{
    //    response.Body.Position = 0;
    //    var sr = new StreamReader(response.Body);
    //    var json = sr.ReadToEnd();

    //    using var writer = new Utf8JsonWriter(response.Body);
    //    byte[] result;

    //    using (var memoryStream1 = new MemoryStream())
    //    {
    //        using (var utf8JsonWriter1 = new Utf8JsonWriter(memoryStream1))
    //        {
    //            using (var jsonDocument = JsonDocument.Parse(json))
    //            {
    //                utf8JsonWriter1.WriteStartObject();

    //                foreach (var element in jsonDocument.RootElement.EnumerateObject())
    //                {
    //                    if (element.Name == "paths")
    //                    {
    //                        utf8JsonWriter1.WritePropertyName(element.Name);
    //                        utf8JsonWriter1.WriteStartObject();
    //                        utf8JsonWriter1.WriteEndObject();
    //                    }
    //                    else
    //                    {
    //                        element.WriteTo(utf8JsonWriter1);
    //                    }
    //                }

    //                utf8JsonWriter1.WriteEndObject();
    //            }
    //        }

    //        result = memoryStream1.ToArray();
    //    }

    //    return result;
    //}

    #endregion

}
