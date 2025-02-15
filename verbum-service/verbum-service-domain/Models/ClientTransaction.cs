using System;
using System.Collections.Generic;

namespace verbum_service_domain.Models;

public partial class ClientTransaction
{
    public Guid ClientId { get; set; }

    public string TransactionId { get; set; } = null!;

    public Guid Orderid { get; set; }

    public bool IsDeposit { get; set; }
}
