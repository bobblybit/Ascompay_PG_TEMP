
using AscomPayPG.Models.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AscomPayPG.Models;

public class TransactionPin:BaseEntity
{
    [Key]
    public long TransactionPinId { get; set; }
    public string SecQuestionOne { get; set; }
    public string? SecQuestionTwo { get; set; }
    public string? SecQuestionThree { get; set; }
    public byte[]? SecurityAnswerOne { get; set; }
    public byte[]? SecurityAnswerTwo { get; set; }
    public byte[]? SecurityAnswerThree { get; set; }
    public byte[]? SecurityAnswerOneSalt { get; set; }
    public byte[]? SecurityAnswerTwoSalt { get; set; }
    public byte[]? SecurityAnswerThreeSalt { get; set; }
    public byte[]? Pin { get; set; }
    public byte[]? PinSalt { get; set; }
    public string UserUid { get; set; }
}
