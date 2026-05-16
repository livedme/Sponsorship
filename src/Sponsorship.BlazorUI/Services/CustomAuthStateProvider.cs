using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Components.Authorization;

namespace Sponsorship.BlazorUI.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly TokenService _tokenService;
    private static readonly AuthenticationState _anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));

    public CustomAuthStateProvider(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _tokenService.GetTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
            return _anonymous;

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            if (jwt.ValidTo < DateTime.UtcNow)
            {
                await _tokenService.ClearTokenAsync();
                return _anonymous;
            }

            var identity = new ClaimsIdentity(jwt.Claims, "jwt");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch
        {
            return _anonymous;
        }
    }

    public async Task NotifyLoginAsync(string token)
    {
        await _tokenService.SetTokenAsync(token);
        var state = await GetAuthenticationStateAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(state));
    }

    public async Task NotifyLogoutAsync()
    {
        await _tokenService.ClearTokenAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
    }
}
