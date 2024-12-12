using Blazored.LocalStorage;
using BookStoreApp.Blazor.Server.UI.Services.Base;
using Microsoft.AspNetCore.Authorization;

namespace BookStoreApp.Blazor.Server.UI.Services;

public class AuthorService : BaseHttpService, IAuthorService
{
    private readonly IClient _client;
    public AuthorService(IClient client, ILocalStorageService localStorage) : base(client, localStorage)
    {
        _client = client;
    }

    public async Task<Response<List<AuthorReadOnlyDto>>> GetAuthors()
    {
        Response<List<AuthorReadOnlyDto>> response = new();

        try
        {
            await GetBearerToken();
            var data = await _client.AuthorsAllAsync();
            response = new Response<List<AuthorReadOnlyDto>> { 
                Data = data.ToList(),
                Success = true
            };
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            response.Success = false;
        }

        return response;
    }

    public async Task<Response<int>> Create(AuthorCreateDto author)
    {
        Response<int> response = new Response<int> { Success = true};

        try
        {
            await GetBearerToken();
            await _client.AuthorsPOSTAsync(author);
        }
        catch (ApiException exception)
        {
            response = ConvertApiExceptions<int>(exception);
        }

        return response;
    }
}
