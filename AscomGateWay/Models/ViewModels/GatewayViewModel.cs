using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AscomPayPG.Models.ViewModels
{
    public class GatewayViewModel
    {
        public GatewayViewModel()
        {
            GatewayOptions = new List<SelectListItem>();
        }
        public GatewayViewModel(IEnumerable<PaymentGateway> paymentGateways)
        {
            GatewayOptions = new List<SelectListItem>();
            foreach (var item in paymentGateways.OrderBy(x => x.Name))
            {
                var t = item.Name;
                var v = item.PaymentGatewayId;
                GatewayOptions.Add(new SelectListItem { Value = v.ToString(), Text = t });
            }

        }
        public long TransactionId { get; set; }
        [Required]
        public long GatewayId { get; set; }
        public string? Reference { get; set; }
        public string? Email { get; set; }
        public string? Description { get; set; }
        public decimal TransactionsAmount { get; set; }
        public string? CallbackURL { get; set; }
        public IList<SelectListItem> GatewayOptions { get; set; }


    }
}