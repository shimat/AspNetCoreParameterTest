using System.Net.Http.Json;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ParameterTestWebApplication.Tests;


public class MyControllerMockTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> factory;

    public MyControllerMockTests(WebApplicationFactory<Program> factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task FormSuccess()
    {
        var httpClient = factory.CreateClient();
        
        using var formDataContent = new MultipartFormDataContent();
        formDataContent.Add(new StringContent("12345"), "id");
        formDataContent.Add(new StringContent("Taro"), "first_name");
        formDataContent.Add(new StringContent("Yamada"), "last_name");

        using var response = await httpClient.PostAsync("/my/form", formDataContent).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Equal("FormRequest { Id = 12345, FirstName = Taro, LastName = Yamada }", responseContent);
    }

    [Fact]
    public async Task FormError400WhenNoId()
    {
        var httpClient = factory.CreateClient();

        using var formDataContent = new MultipartFormDataContent();
        formDataContent.Add(new StringContent("Taro"), "first_name");
        formDataContent.Add(new StringContent("Yamada"), "last_name");

        using var response = await httpClient.PostAsync("/my/form", formDataContent).ConfigureAwait(false);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        Assert.Contains("The Id field is required.", content);
    }

    [Fact]
    public async Task QuerySuccess()
    {
        var httpClient = factory.CreateClient();
        
        var parameters = new Dictionary<string, string>
        {
            { "id", "12345" },
            { "first_name", "Taro" },
            { "last_name", "Yamada" },
        };
        var query = await new FormUrlEncodedContent(parameters).ReadAsStringAsync();

        using var formDataContent = new MultipartFormDataContent();
        formDataContent.Add(new StringContent("12345"), "id");
        formDataContent.Add(new StringContent("Taro"), "first_name");
        formDataContent.Add(new StringContent("Yamada"), "last_name");

        using var response = await httpClient.GetAsync($"/my/query?{query}").ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Equal("QueryRequest{ Id = 12345, FirstName = Taro, LastName = Yamada }", responseContent);
    }
    
    [Fact]
    public async Task BodySuccess()
    {
        var httpClient = factory.CreateClient();

        var request = new
        {
            id = "12345",
            first_name = "Taro",
            last_name = "Yamada"
        };

        using var response = await httpClient.PostAsJsonAsync("/my/body", request).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Equal("BodyRequest { Id = 12345, FirstName = Taro, LastName = Yamada }", responseContent);
    }
}
