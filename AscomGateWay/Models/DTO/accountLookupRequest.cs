namespace AscomPayPG.Models.DTO;

public class accountLookupRequest
{
    public string bank_code { get; set; }
    public string account_number { get; set; }
}


public class AccountInquery
{
    public string bank_code { get; set; }
    public string account_number { get; set; }
    public string source_account_number { get; set; }
}

public class AccountLookUpResponse
{
    public bool IsSuccessful { get; set; }
    public string Message { get; set; }
    public int ResponseCode { get; set; }
    public LookUpResponseData Data { get; set; } = new LookUpResponseData();
    public string LRId { get; set; }
}

public class LookUpResponseData
{
    public string account_name { get; set; }
    public string account_number { get; set; }
}

public class AccountVerificationDTO
{
    public string bank { get; set; }
    public string number { get; set; }
    public string senderaccountnumber { get; set; }
}
public class Customer
{
    public AccountVerificationDTO account { get; set; } = new AccountVerificationDTO();
}

public class Root9PSBAccVerificationPayLoad
{
    public Customer customer { get; set; } = new Customer();
}
