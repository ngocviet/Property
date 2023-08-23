using System;
using System.Collections.Generic;

namespace Property.Models;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public int? AccountId { get; set; }

    public int? PackageId { get; set; }

    public int? Amount { get; set; }

    public DateTime? PaymentDate { get; set; }

    public bool? Status { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Package? Package { get; set; }
}
