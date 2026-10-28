using Microsoft.JSInterop;

namespace FocusFlow.Client.Auth;

// Keeps the JWT in the browser's localStorage so a page refresh keeps you logged in
public class TokenStore(IJSRuntime js)
{
    private const string Key = "focusflow.token";

    public ValueTask<string?> GetAsync() => js.InvokeAsync<string?>("localStorage.getItem", Key);

    public ValueTask SetAsync(string token) => js.InvokeVoidAsync("localStorage.setItem", Key, token);

    public ValueTask RemoveAsync() => js.InvokeVoidAsync("localStorage.removeItem", Key);
}
