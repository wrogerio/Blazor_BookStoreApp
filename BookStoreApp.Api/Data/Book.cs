﻿using System;
using System.Collections.Generic;

namespace BookStoreApp.Api.Data;

public partial class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int? Year { get; set; }

    public string Isbn { get; set; } = null!;

    public string Summary { get; set; } = null!;

    public string Image { get; set; } = null!;

    public double? Price { get; set; }

    public int? AuthorId { get; set; }

    public virtual Author? Author { get; set; }
}
