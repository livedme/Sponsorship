using System.Net.Http.Headers;
using System.Net.Http.Json;
using Sponsorship.BlazorUI.Models;

namespace Sponsorship.BlazorUI.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private readonly TokenService _tokenService;

    public ApiService(HttpClient http, TokenService tokenService)
    {
        _http = http;
        _tokenService = tokenService;
    }

    private async Task SetAuthHeaderAsync()
    {
        var token = await _tokenService.GetTokenAsync();
        _http.DefaultRequestHeaders.Authorization =
            string.IsNullOrEmpty(token) ? null : new AuthenticationHeaderValue("Bearer", token);
    }

    // ─── Auth ───────────────────────────────────────────────────────────────

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", request);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<LoginResponse>();
    }

    // ─── Sponsorship Types ───────────────────────────────────────────────────

    public async Task<List<SponsorshipTypeDto>> GetSponsorshipTypesAsync()
    {
        await SetAuthHeaderAsync();
        return await _http.GetFromJsonAsync<List<SponsorshipTypeDto>>("api/sponsorship-types")
               ?? new List<SponsorshipTypeDto>();
    }

    // ─── Requests ────────────────────────────────────────────────────────────

    public async Task<List<SponsorshipRequestDto>> GetMyRequestsAsync()
    {
        await SetAuthHeaderAsync();
        return await _http.GetFromJsonAsync<List<SponsorshipRequestDto>>("api/requests")
               ?? new List<SponsorshipRequestDto>();
    }

    public async Task<SponsorshipRequestDto?> GetRequestByIdAsync(string id)
    {
        await SetAuthHeaderAsync();
        return await _http.GetFromJsonAsync<SponsorshipRequestDto>($"api/requests/{id}");
    }

    public string? LastError { get; private set; }

    public async Task<SponsorshipRequestDto?> CreateRequestAsync(CreateSponsorshipRequestDto dto)
    {
        await SetAuthHeaderAsync();
        var response = await _http.PostAsJsonAsync("api/requests", dto);
        if (!response.IsSuccessStatusCode)
        {
            LastError = await response.Content.ReadAsStringAsync();
            return null;
        }
        LastError = null;
        return await response.Content.ReadFromJsonAsync<SponsorshipRequestDto>();
    }

    public async Task<SponsorshipRequestDto?> UpdateRequestAsync(string id, UpdateSponsorshipRequestDto dto)
    {
        await SetAuthHeaderAsync();
        var response = await _http.PutAsJsonAsync($"api/requests/{id}", dto);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<SponsorshipRequestDto>();
    }

    public async Task<bool> SubmitRequestAsync(string id)
    {
        await SetAuthHeaderAsync();
        var response = await _http.PostAsync($"api/requests/{id}/submit", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> CancelRequestAsync(string id, string? remarks)
    {
        await SetAuthHeaderAsync();
        var response = await _http.PostAsJsonAsync($"api/requests/{id}/cancel", new ActionRemarksDto { Remarks = remarks });
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ManagerApproveAsync(string id, string? remarks)
    {
        await SetAuthHeaderAsync();
        var response = await _http.PostAsJsonAsync($"api/requests/{id}/manager-approve", new ActionRemarksDto { Remarks = remarks });
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ManagerRejectAsync(string id, string? remarks)
    {
        await SetAuthHeaderAsync();
        var response = await _http.PostAsJsonAsync($"api/requests/{id}/manager-reject", new ActionRemarksDto { Remarks = remarks });
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> FinanceApproveAsync(string id, string? remarks)
    {
        await SetAuthHeaderAsync();
        var response = await _http.PostAsJsonAsync($"api/requests/{id}/finance-approve", new ActionRemarksDto { Remarks = remarks });
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> FinanceRejectAsync(string id, string? remarks)
    {
        await SetAuthHeaderAsync();
        var response = await _http.PostAsJsonAsync($"api/requests/{id}/finance-reject", new ActionRemarksDto { Remarks = remarks });
        return response.IsSuccessStatusCode;
    }

    // ─── Admin ───────────────────────────────────────────────────────────────

    public async Task<List<SponsorshipRequestDto>> AdminGetAllRequestsAsync()
    {
        await SetAuthHeaderAsync();
        return await _http.GetFromJsonAsync<List<SponsorshipRequestDto>>("api/admin/requests")
               ?? new List<SponsorshipRequestDto>();
    }

    public async Task<List<WorkflowHistoryDto>> AdminGetWorkflowHistoryAsync()
    {
        await SetAuthHeaderAsync();
        return await _http.GetFromJsonAsync<List<WorkflowHistoryDto>>("api/admin/workflow-history")
               ?? new List<WorkflowHistoryDto>();
    }

    public async Task<List<SponsorshipTypeDto>> AdminGetSponsorshipTypesAsync()
    {
        await SetAuthHeaderAsync();
        return await _http.GetFromJsonAsync<List<SponsorshipTypeDto>>("api/admin/sponsorship-types")
               ?? new List<SponsorshipTypeDto>();
    }

    public async Task<SponsorshipTypeDto?> AdminCreateSponsorshipTypeAsync(CreateSponsorshipTypeDto dto)
    {
        await SetAuthHeaderAsync();
        var response = await _http.PostAsJsonAsync("api/admin/sponsorship-types", dto);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<SponsorshipTypeDto>();
    }

    public async Task<bool> AdminUpdateSponsorshipTypeAsync(int id, CreateSponsorshipTypeDto dto)
    {
        await SetAuthHeaderAsync();
        var response = await _http.PutAsJsonAsync($"api/admin/sponsorship-types/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> AdminDeleteSponsorshipTypeAsync(int id)
    {
        await SetAuthHeaderAsync();
        var response = await _http.DeleteAsync($"api/admin/sponsorship-types/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<string?> GetErrorMessageAsync(HttpResponseMessage response)
    {
        try { return await response.Content.ReadAsStringAsync(); } catch { return null; }
    }
}
