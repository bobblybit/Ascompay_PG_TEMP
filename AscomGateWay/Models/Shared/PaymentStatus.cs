namespace AscomPayPG.Models.Shared
{

    public enum PaymentStatus
    {
        Init = 1,
        Pending,
        Declined,
        Approved,
        Completed,
        Failed,
        Successful
    }

    public enum ClientRequestEnum
    {
        Init = 1,
        Done,
    }
}

