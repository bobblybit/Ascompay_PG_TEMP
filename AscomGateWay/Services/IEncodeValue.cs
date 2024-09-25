using AscomPayPG.Data;
using AscomPayPG.Models;
using AscomPayPG.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace AscomPayPG.Services
{
    public interface IEncodeValue
    {
        public Task<ResponseMessage> decrypt(string key, int Type, Guid Token);

    }
}