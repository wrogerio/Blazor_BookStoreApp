using Microsoft.AspNetCore.Identity;

namespace BookStoreApp.Api.Data
{
    public class ApiUser : IdentityUser
    {
        public string Apelido { get; set; }
    }
}
