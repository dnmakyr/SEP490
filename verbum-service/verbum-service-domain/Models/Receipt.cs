using System;
using System.Collections.Generic;

namespace verbum_service_domain.Models;

/// <summary>
/// true is deposit, false is payment
/// </summary>
public partial class Receipt
{
    public Guid ReceiptId { get; set; }

    public DateTime? PayDate { get; set; }

    public Guid? OrderId { get; set; }

    public bool DepositeOrPayment { get; set; }

    public decimal? Amount { get; set; }

    public bool Done { get; set; }

    public string PaymentId { get; set; } = null!;

    public virtual Order? Order { get; set; }
}
