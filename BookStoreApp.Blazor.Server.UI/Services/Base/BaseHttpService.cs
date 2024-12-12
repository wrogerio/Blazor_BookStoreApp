using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace BookStoreApp.Blazor.Server.UI.Services.Base;

public class BaseHttpService
{
    protected readonly IClient _client;
    private readonly ILocalStorageService _localStorage;

    public BaseHttpService(IClient client, ILocalStorageService localStorage)
    {
        _client = client;
        _localStorage = localStorage;
    }

    protected Response<Guid> ConvertApiExceptions<Guid>(ApiException apiException)
    {
        if(apiException.StatusCode >= 200 && apiException.StatusCode <= 299)
            return new Response<Guid> { Message = "Operation Reperted with success", Success = true, ValidationErrors = apiException.Message };
        if (apiException.StatusCode == 400)
            return new Response<Guid> { Message = "Validation errors have occurred", Success = false, ValidationErrors = apiException.Message };
        if (apiException.StatusCode == 404)
            return new Response<Guid> { Message = "The requested item could not be found.", Success = false, ValidationErrors = apiException.Message };

        return new Response<Guid> { Message = "An error has occurred", Success = false, ValidationErrors = apiException.Message };
    }

    protected async Task GetBearerToken()
    {
        var token = await _localStorage.GetItemAsync<string>("token");
        if (token != null)
            _client.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);


    }
}
