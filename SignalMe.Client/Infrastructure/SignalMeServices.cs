
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using SignalMe.Client.Models;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http.Json;


namespace SignalMe.Client.Infrastructure
{
    public class SignalMeServices
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;
        private readonly AuthenticationStateProvider _autStateProvider;

        public SignalMeServices(
            HttpClient httpClient,
            NavigationManager navigationManager,
            AuthenticationStateProvider autStateProvider)
        {
            _httpClient = httpClient;
            _navigationManager = navigationManager;
            _autStateProvider = autStateProvider;
        }

        public async Task<List<AppUser>> GetUserList()
        {
            try
            {
                var users = await _httpClient.GetFromJsonAsync<List<AppUser>>("api/user/getAppUserList");
                if (users != null)
                {
                    return users;
                }
                else
                {
                    return new List<AppUser>();
                    // Return an empty list if users is null
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request error: {ex.Message}");
            }
            catch (NotSupportedException ex)
            {
                Console.WriteLine($"Content type error: {ex.Message}");
            }
            return new List<AppUser>();
        }
    }
}
