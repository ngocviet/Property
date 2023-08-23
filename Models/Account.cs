using System;
using System.Collections.Generic;

namespace Property.Models;

public partial class Account
{
    public int AccountId { get; set; }

    public int? PackageId { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? Name { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Avatar { get; set; }

    public string? Permissions { get; set; }

    public bool? Status { get; set; }

    public int? NumberPost { get; set; }

    public virtual ICollection<Listing> Listings { get; set; } = new List<Listing>();

    public virtual Package? Package { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
