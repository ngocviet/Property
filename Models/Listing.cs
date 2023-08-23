using System;
using System.Collections.Generic;

namespace Property.Models;

public partial class Listing
{
    public int ListingId { get; set; }

    public int? RealtyId { get; set; }

    public int? AccountId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public int? Location { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? Status { get; set; }

    public int? ViewAmount { get; set; }

    public bool? Duration { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Realty? Realty { get; set; }
}
