using System;
using System.Collections.Generic;

namespace Zainah_Nahool.Models;

public partial class Payment
{
    public int Id { get; set; }

    public int? OrderId { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string? PaymentStatus { get; set; }

    public string? TransactionId { get; set; }

    public decimal Amount { get; set; }

    public DateTime? PaymentDate { get; set; }

    public virtual Order? Order { get; set; }
}
