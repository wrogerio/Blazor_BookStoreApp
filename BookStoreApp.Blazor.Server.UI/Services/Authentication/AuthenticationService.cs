using Blazored.LocalStorage;
using BookStoreApp.Blazor.Server.UI.Providers;
using BookStoreApp.Blazor.Server.UI.Services.Base;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;

namespace BookStoreApp.Blazor.Server.UI.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IClient _client;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _stateProvider;

        public AuthenticationService(IClient client, ILocalStorageService localStorage, AuthenticationStateProvider stateProvider)
        {
            _client = client;
            _localStorage = localStorage;
            _stateProvider = stateProvider;
        }

        public async Task<bool> AuthenticateAsync(LoginUserDto loginModel)
        {
            var res = await _client.LoginAsync(loginModel);
            if(res != null)
            {
                // store token
                await _localStorage.SetItemAsync("token", res.Token);
                await _localStorage.SetItemAsync("email", res.Email);
                await _localStorage.SetItemAsync("userid", res.UserId);

                // change auth stage of app
                await ((ApiAuthenticationStateProvider)_stateProvider).LoggedIn();

                return true;
            }
            
            return false;   
        }

        public async Task LogoutAsync()
        {
            await ((ApiAuthenticationStateProvider)_stateProvider).LoggedOut();
        }
    }
}
