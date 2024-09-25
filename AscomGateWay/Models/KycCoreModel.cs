
using AscomPayPG.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscomPayPG.Models
{
    public class KycCoreModel:BaseEntity
    {
        public string UserUid { get; set; }
        public string? UserCode { get; set; }
        public string? CountryCode { get; set; }
        public bool? HasIdVerified { get; set; }
        public bool? HasSubmittedId { get; set; }
        public bool? HasBVNVerified { get; set; }
        public bool? HasSubmittedBVN { get; set; }
        public bool? HasAdditionalDocVerified { get; set; }
        public bool? HasSubmittedAdditionalDoc { get; set; }
        public bool? HasCompanyVerified { get; set; }
        public bool? HasSubmittedCompany { get; set; }
        public bool? HasSanctionVerified { get; set; }
        public bool? HasCertified { get; set; }
        public bool? IsIdRejected { get; set; }
        public bool? IsCompanyRejected { get; set; }
        public bool? IsBVNRejected { get; set; }
        public bool? IsAdditionalDocRejected { get; set; }
        public string? RejectNote { get; set; }
    }
}
