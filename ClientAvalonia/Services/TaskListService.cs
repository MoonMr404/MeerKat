using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Shared.Dto;

public class TaskListService(HttpClient httpClient, string apiBaseUrl)
{
    private readonly string _baseUrl = $"{apiBaseUrl}/api/tasklist";

    // GET: api/TaskList
    public async Task<IEnumerable<TaskListDto>> GetTaskListsAsync(bool nested = false)
    {
        var taskLists = await httpClient.GetFromJsonAsync<IEnumerable<TaskListDto>>($"{_baseUrl}?nested={nested}");
        return taskLists ?? Enumerable.Empty<TaskListDto>();
    }

    // GET: api/TaskList/{id}
    public async Task<TaskListDto?> GetTaskListByIdAsync(Guid id, bool nested = false)
    {
        try
        {
            return await httpClient.GetFromJsonAsync<TaskListDto>($"{_baseUrl}/{id}?nested={nested}");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }
    
    // GET: api/TaskList/team/{id}
    public async Task<IEnumerable<TaskListDto>> GetTaskListByTeamAsync(Guid teamId, bool nested = false)
    {
        try
        {
            return await httpClient.GetFromJsonAsync<IEnumerable<TaskListDto>>($"{_baseUrl}/team/{teamId}?nested={nested}");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    // POST: api/TaskList
    public async Task<TaskListDto> CreateTaskListAsync(TaskListDto taskList)
    {
        var response = await httpClient.PostAsJsonAsync(_baseUrl, taskList);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<TaskListDto>()
                   ?? throw new InvalidOperationException("Failed to deserialize the created task list");
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

    // PUT: api/TaskList
    public async Task<TaskListDto> UpdateTaskListAsync(TaskListDto taskList)
    {
        var response = await httpClient.PutAsJsonAsync(_baseUrl, taskList);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TaskListDto>()
               ?? throw new InvalidOperationException("Failed to deserialize the updated task list");
    }

    // DELETE: api/TaskList/{id}
    public async Task DeleteTaskListAsync(Guid id)
    {
        var response = await httpClient.DeleteAsync($"{_baseUrl}/{id}");
        response.EnsureSuccessStatusCode();
    }
}