﻿namespace AscomPayPG.Models.DTO
{
    public class ResponseMessage
    {
        public int? ResponseCode { get; set; } = 0;
        public bool? isOk { get; set; } = false;
        public string? Message { get; set; } = string.Empty;
    }
}