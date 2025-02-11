using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Splat;

namespace ClientAvalonia.Services;

using System.Net.Http.Json;
using Shared.Dto;

public class UserService(HttpClient httpClient, string apiBaseUrl)
{
    private readonly string _baseUrl = $"{apiBaseUrl}/api/user";

    
    public async Task<IEnumerable<UserDto>> GetUsersAsync(bool nested = false)
    {
        var users = await httpClient.GetFromJsonAsync<IEnumerable<UserDto>>($"{_baseUrl}?nested={nested}");
        return users ?? Enumerable.Empty<UserDto>();
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id, bool nested = false)
    {
        try
        {
            return await httpClient.GetFromJsonAsync<UserDto>($"{_baseUrl}/{id}?nested={nested}");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<UserDto> CreateUserAsync(UserDto user)
    {
        var response = await httpClient.PostAsJsonAsync(_baseUrl, user);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserDto>() 
               ?? throw new InvalidOperationException("Failed to deserialize the created user");
    }

    public async Task<UserDto> UpdateUserAsync(UserDto user)
    {
        var response = await httpClient.PutAsJsonAsync(_baseUrl, user);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserDto>() 
               ?? throw new InvalidOperationException("Failed to deserialize the updated user");
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var response = await httpClient.DeleteAsync($"{_baseUrl}/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<UserTokenDto> LoginAsync(LoginRequestDto loginRequest)
    {
        var response = await httpClient.PostAsJsonAsync($"{_baseUrl}/login", loginRequest);
        response.EnsureSuccessStatusCode();
        UserTokenDto token = await response.Content.ReadFromJsonAsync<UserTokenDto>() ?? throw new InvalidOperationException("Failed to retrieve the user token");
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
        return token;
    }
    
}