using AscomPayPG.Models.DTO;

namespace AscomPayPG.Services
{
    public interface IEncodeValue
    {
        public Task<ResponseMessage> decrypt(string key, int Type, Guid Token);
    }
}