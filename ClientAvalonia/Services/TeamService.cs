using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Shared.Dto;

public class TeamService(HttpClient httpClient, string apiBaseUrl)
{
    private readonly string _baseUrl = $"{apiBaseUrl}/api/team";

    // GET: api/Team
    public async Task<IEnumerable<TeamDto>> GetTeamsAsync(bool nested = false)
    {
        var teams = await httpClient.GetFromJsonAsync<IEnumerable<TeamDto>>($"{_baseUrl}?nested={nested}");
        return teams ?? Enumerable.Empty<TeamDto>();
    }

    // GET: api/Team/{id}
    public async Task<TeamDto?> GetTeamByIdAsync(Guid id, bool nested = false)
    {
        try
        {
            return await httpClient.GetFromJsonAsync<TeamDto>($"{_baseUrl}/{id}?nested={nested}");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    // POST: api/Team
    public async Task<TeamDto> CreateTeamAsync(TeamDto team)
    {
        var response = await httpClient.PostAsJsonAsync(_baseUrl, team);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<TeamDto>()
                   ?? throw new InvalidOperationException("Failed to deserialize the created team");
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            string errorMessage = await response.Content.ReadAsStringAsync();
            throw new ArgumentException($"Validation failed: {errorMessage}");
        }
        else
        {
            response.EnsureSuccessStatusCode();
            throw new HttpRequestException($"Generic error: {response.ReasonPhrase}");
        }
    }

    // PUT: api/Team
    public async Task<TeamDto> UpdateTeamAsync(TeamDto team)
    {
        var response = await httpClient.PutAsJsonAsync(_baseUrl, team);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TeamDto>()
               ?? throw new InvalidOperationException("Failed to deserialize the updated team");
    }

    // DELETE: api/Team/{id}
    public async Task DeleteTeamAsync(Guid id)
    {
        var response = await httpClient.DeleteAsync($"{_baseUrl}/{id}");
        response.EnsureSuccessStatusCode();
    }
}