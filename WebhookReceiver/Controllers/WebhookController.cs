using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

[Route("api/[controller]")]
[ApiController]
public class WebhookController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public WebhookController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] WebhookPayload payload)
    {
        if (payload.Status == "completed")
        {
            var transcriptText = await GetTranscriptText(payload.TranscriptId);
            System.IO.File.WriteAllText($"{payload.TranscriptId}.txt", transcriptText);
            return Ok();
        }
        else if (payload.Status == "error")
        {
            System.Console.WriteLine("Transcription error");
            return StatusCode(500, "Transcription error");
        }

        return BadRequest("Invalid status");
    }

    private async Task<string> GetTranscriptText(string transcriptId)
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("authorization", "API_KEY_HERE");

        var response = await client.GetAsync($"https://api.assemblyai.com/v2/transcript/{transcriptId}");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var transcriptResponse = JsonSerializer.Deserialize<TranscriptResponse>(jsonResponse);

        return transcriptResponse.Text;
    }

    public class TranscriptResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}

public class WebhookPayload
{
    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("transcript_id")]
    public string TranscriptId { get; set; }
}