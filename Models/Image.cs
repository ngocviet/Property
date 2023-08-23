using System;
using System.Collections.Generic;

namespace Property.Models;

public partial class Image
{
    public int ImageId { get; set; }

    public int? RealtyId { get; set; }

    public string? ImageUrl { get; set; }

    public bool? Status { get; set; }

    public virtual Realty? Realty { get; set; }
}
