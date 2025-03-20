using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Shared.Dto;

public class TaskService(HttpClient httpClient, string apiBaseUrl)
{
    private readonly string _baseUrl = $"{apiBaseUrl}/api/task";

    // GET: api/Task
    public async Task<IEnumerable<TaskDto>> GetTasksAsync(bool nested = false)
    {
        var tasks = await httpClient.GetFromJsonAsync<IEnumerable<TaskDto>>($"{_baseUrl}?nested={nested}");
        return tasks ?? Enumerable.Empty<TaskDto>();
    }

    // GET: api/Task/{id}
    public async Task<TaskDto?> GetTaskByIdAsync(Guid id, bool nested = false)
    {
        try
        {
            return await httpClient.GetFromJsonAsync<TaskDto>($"{_baseUrl}/{id}?nested={nested}");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    // POST: api/Task
    public async Task<TaskDto> CreateTaskAsync(TaskDto task)
    {
        var response = await httpClient.PostAsJsonAsync(_baseUrl, task);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<TaskDto>()
                   ?? throw new InvalidOperationException("Failed to deserialize the created task");
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

    // PUT: api/Task
    public async Task<TaskDto> UpdateTaskAsync(TaskDto task)
    {
        var response = await httpClient.PutAsJsonAsync(_baseUrl, task);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TaskDto>()
               ?? throw new InvalidOperationException("Failed to deserialize the updated task");
    }

    // DELETE: api/Task/{id}
    public async Task DeleteTaskAsync(Guid id)
    {
        var response = await httpClient.DeleteAsync($"{_baseUrl}/{id}");
        response.EnsureSuccessStatusCode();
    }
    
    // GET: api/Task/complete/id
    public async Task<TaskDto> CompleteTaskAsync(Guid id)
    {
        try
        {
            return await httpClient.GetFromJsonAsync<TaskDto>($"{_baseUrl}/complete/{id}")
                   ?? throw new InvalidOperationException("Failed to deserialize the created task");
            
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }
}