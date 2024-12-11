using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.Api.Data;

public partial class BookStoreContext : IdentityDbContext<ApiUser>
{
    public BookStoreContext()
    {
    }

    public BookStoreContext(DbContextOptions<BookStoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Author>(entity =>
        {
            entity.Property(e => e.FirstName).HasMaxLength(60);
            entity.Property(e => e.LastName).HasMaxLength(60);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.Property(e => e.Isbn).HasMaxLength(20);
            entity.Property(e => e.Summary).HasMaxLength(60);
            entity.Property(e => e.Title).HasMaxLength(30);

            entity.HasOne(d => d.Author).WithMany(p => p.Books).HasForeignKey(d => d.AuthorId);
        });

        var roleAdm = new IdentityRole { Id = "062BE8C7-CBB8-4E29-97F9-F91A4A53F118", Name = "Admin", NormalizedName = "ADMIN" };
        var roleUser = new IdentityRole { Id = "86EEC2F2-B47E-4C12-ABAA-619CAAB2533D", Name = "User", NormalizedName = "USER" };
        var passwordHasher = new PasswordHasher<ApiUser>();
        var userAdm = new ApiUser
        {
            Id = "D04CB72E-3C41-4520-AEBC-2021807A3F40",
            Email = "admin@admin.com",
            NormalizedEmail = "admin@admin.com".ToUpper(),
            UserName = "admin",
            NormalizedUserName = "admin".ToUpper(),
            EmailConfirmed = true,
            Apelido = "Admin",
            PasswordHash = passwordHasher.HashPassword(null!, "adm123456")
        };
        var userUser = new ApiUser
        {
            Id = "21F01427-2622-43C8-B5F3-BBD58DA190C2",
            Email = "user@user.com",
            NormalizedEmail = "user@user.com".ToUpper(),
            UserName = "user",
            NormalizedUserName = "user".ToUpper(),
            EmailConfirmed = true,
            Apelido = "User",
            PasswordHash = passwordHasher.HashPassword(null!, "usr123456")
        };
        var admAsign = new IdentityUserRole<string> { RoleId = roleAdm.Id, UserId = userAdm.Id };
        var userAsign = new IdentityUserRole<string> { RoleId = roleUser.Id, UserId = userUser.Id };

        modelBuilder.Entity<IdentityRole>().HasData(roleAdm, roleUser);
        modelBuilder.Entity<ApiUser>().HasData(userAdm, userUser);
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(admAsign, userAsign);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
