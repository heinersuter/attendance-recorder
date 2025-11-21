using Microsoft.AspNetCore.Mvc.Testing;
using NSwag;
using NSwag.CodeGeneration.TypeScript;
using Shouldly;

namespace AttendanceRecorder.WebApi.IntegrationTests;

public class ApiClientTests
{
    private HttpClient _client = null!;
    private WebApplicationFactory<Program> _factory = null!;

    [SetUp]
    public void Setup()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task TypeScriptApiClient_NewlyGenerated_MatchesExistingFileAsync()
    {
        var response = await _client.GetAsync("/swagger/v1/swagger.json");
        response.EnsureSuccessStatusCode();
        var swaggerJson = await response.Content.ReadAsStringAsync();
        var document = await OpenApiDocument.FromJsonAsync(swaggerJson);
        var settings = new TypeScriptClientGeneratorSettings
        {
            ClassName = "ApiClient", GenerateOptionalParameters = false,
        };
        var generator = new TypeScriptClientGenerator(document, settings);
        var code = generator.GenerateFile();

        const string generatedClientFile = "../../../../attendance-recorder-react/src/ApiClient.Generated.ts";
        var existingCode = await File.ReadAllTextAsync(generatedClientFile);
        await File.WriteAllTextAsync(generatedClientFile, code);

        code.ShouldBe(existingCode);
    }
}