using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace FocusFlow.Client.Auth;

// Tells Blazor who is logged in, based on the JWT in TokenStore.
// <AuthorizeView> and [CascadingParameter] Task<AuthenticationState> both read from here.
public class JwtAuthStateProvider(TokenStore tokenStore) : AuthenticationStateProvider
{
    private static readonly AuthenticationState Anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await tokenStore.GetAsync();
        if (string.IsNullOrWhiteSpace(token))
            return Anonymous;

        var claims = ReadClaims(token);
        if (claims is null || IsExpired(claims))
        {
            await tokenStore.RemoveAsync();
            return Anonymous;
        }

        var identity = new ClaimsIdentity(claims, authenticationType: "jwt", nameType: "name", roleType: "role");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task LoginAsync(string token)
    {
        await tokenStore.SetAsync(token);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task LogoutAsync()
    {
        await tokenStore.RemoveAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
    }

    // A JWT is header.payload.signature. The payload is base64url-encoded JSON.
    // We only read it here; the API is what checks the signature.
    private static List<Claim>? ReadClaims(string token)
    {
        var parts = token.Split('.');
        if (parts.Length != 3)
            return null;

        try
        {
            var payload = parts[1].Replace('-', '+').Replace('_', '/');
            payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(payload));

            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.EnumerateObject()
                .Select(p => new Claim(p.Name, p.Value.ToString()))
                .ToList();
        }
        catch (Exception ex) when (ex is FormatException or JsonException)
        {
            return null;
        }
    }

    private static bool IsExpired(List<Claim> claims)
    {
        var exp = claims.FirstOrDefault(c => c.Type == "exp")?.Value;
        return !long.TryParse(exp, out var seconds)
            || DateTimeOffset.FromUnixTimeSeconds(seconds) <= DateTimeOffset.UtcNow;
    }
}
