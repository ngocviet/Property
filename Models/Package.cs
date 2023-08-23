using System;
using System.Collections.Generic;

namespace Property.Models;

public partial class Package
{
    public int PackageId { get; set; }

    public string? PackageName { get; set; }

    public decimal? Price { get; set; }

    public int? NumberPost { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
