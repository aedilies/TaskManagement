using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using TaskManagement.Application.DTOs.Auth;
using Xunit;

namespace TaskManagement.Tests.Integration;

public class TasksControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TasksControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetTasks_ReturnsOk()
    {
        var loginDto = new LoginDto { Email = "admin@example.com", Password = "123" };
        var loginContent = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");

        var loginResponse = await _client.PostAsync("/api/auth/login", loginContent);
        loginResponse.EnsureSuccessStatusCode();

        var loginJson = await loginResponse.Content.ReadAsStringAsync();
        var token = JsonDocument.Parse(loginJson).RootElement.GetProperty("token").GetString();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/tasks");
        response.EnsureSuccessStatusCode();
    }
}