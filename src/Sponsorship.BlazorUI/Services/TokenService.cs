using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Sponsorship.BlazorUI.Services;

public class TokenService
{
    private readonly ProtectedLocalStorage _localStorage;
    private string? _cachedToken;

    public TokenService(ProtectedLocalStorage localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task SetTokenAsync(string token)
    {
        _cachedToken = token;
        await _localStorage.SetAsync("auth_token", token);
    }

    public async Task<string?> GetTokenAsync()
    {
        if (_cachedToken != null) return _cachedToken;
        try
        {
            var result = await _localStorage.GetAsync<string>("auth_token");
            _cachedToken = result.Success ? result.Value : null;
        }
        catch
        {
            _cachedToken = null;
        }
        return _cachedToken;
    }

    public async Task ClearTokenAsync()
    {
        _cachedToken = null;
        try { await _localStorage.DeleteAsync("auth_token"); } catch { }
    }
}
