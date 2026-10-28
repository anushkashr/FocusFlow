using System.Net;
using System.Net.Http.Json;
using FocusFlow.Client.Auth;
using FocusFlow.Shared.Auth;

namespace FocusFlow.Client.Services;

public class AuthApiClient(HttpClient http, JwtAuthStateProvider auth)
{
    // Returns null on success, otherwise an error message to show
    public async Task<string?> LoginAsync(LoginRequest request)
    {
        HttpResponseMessage response;
        try
        {
            response = await http.PostAsJsonAsync("api/auth/login", request);
        }
        catch (HttpRequestException)
        {
            return $"Can't reach the API at {http.BaseAddress}. Is FocusFlow.Api running?";
        }

        using (response)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return "Wrong username or password.";
            if (!response.IsSuccessStatusCode)
                return $"Login failed ({(int)response.StatusCode}). Try again.";

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (result is null)
                return "Login failed. Try again.";

            await auth.LoginAsync(result.AccessToken);
            return null;
        }
    }

    // JWTs are stateless, so logging out only means forgetting the token
    public Task LogoutAsync() => auth.LogoutAsync();
}
