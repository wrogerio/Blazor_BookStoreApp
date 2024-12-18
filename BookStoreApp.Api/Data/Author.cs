﻿using System;
using System.Collections.Generic;

namespace BookStoreApp.Api.Data;

public partial class Author
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Bio { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}