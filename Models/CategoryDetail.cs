using System;
using System.Collections.Generic;

namespace Property.Models;

public partial class CategoryDetail
{
    public int CategoryDetailId { get; set; }

    public int? CategoryId { get; set; }

    public string? CategoryDetailName { get; set; }

    public string? Description { get; set; }

    public bool? Status { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Realty> Realties { get; set; } = new List<Realty>();
}
