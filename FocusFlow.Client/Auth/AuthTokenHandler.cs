using System.Net.Http.Headers;

namespace FocusFlow.Client.Auth;

// Runs before every request from TaskApiClient and adds "Authorization: Bearer <token>".
// Pages never have to set the header themselves.
public class AuthTokenHandler(TokenStore tokenStore) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await tokenStore.GetAsync();
        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}
