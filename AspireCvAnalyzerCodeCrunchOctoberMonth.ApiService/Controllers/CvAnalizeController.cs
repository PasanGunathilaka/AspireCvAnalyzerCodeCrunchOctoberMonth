using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Azure.AI.OpenAI;
using Azure.Identity;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace AspireCvAnalyzerCodeCrunchOctoberMonth.ApiService.Controllers;
[ApiController]
[Route("api/[controller]")]
public class CvAnalyzeController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly string _openAiEndpoint = _configuration["AzureOpenAI:Endpoint"];
    private readonly string _apiKey = _configuration["AzureOpenAI:ApiKey"];
 
    public CvAnalyzeController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [HttpPost("analyze")]
    public async Task<IActionResult> AnalyzeCv([FromBody] CvAnalysisRequest request)
    {
        if (string.IsNullOrEmpty(request.CvText))
        {
            return BadRequest("CV text is required.");
        }

        var requestBody = new
        {
            prompt = request.CvText,
            max_tokens = 1000,
            temperature = 0.7
        };

        var jsonRequest = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        _httpClient.DefaultRequestHeaders.Add("api-key", _apiKey);

        var response = await _httpClient.PostAsync($"{_openAiEndpoint}v1/engines/davinci/completions", content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            return Ok(responseContent);
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, errorContent);
        }
    }
}