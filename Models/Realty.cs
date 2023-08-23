using System;
using System.Collections.Generic;

namespace Property.Models;

public partial class Realty
{
    public int RealtyId { get; set; }

    public string? Title { get; set; }

    public string? Address { get; set; }

    public decimal? Price { get; set; }

    public int? Bedrooms { get; set; }

    public int? Bathrooms { get; set; }

    public string? HouseDirection { get; set; }

    public string? Interior { get; set; }

    public decimal? Area { get; set; }

    public bool? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CategoryDetailId { get; set; }

    public virtual CategoryDetail? CategoryDetail { get; set; }

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<Listing> Listings { get; set; } = new List<Listing>();
}
