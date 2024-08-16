using Microsoft.AspNetCore.Mvc;
using AssemblyAI;
using AssemblyAI.Transcripts;

[Route("api/[controller]")]
[ApiController]
public class WebhookController(AssemblyAIClient client, ILogger<WebhookController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] TranscriptReadyNotification transcriptNotification)
    {
        var transcript = await client.Transcripts.GetAsync(transcriptNotification.TranscriptId);
        switch (transcriptNotification.Status)
        {
            case TranscriptReadyStatus.Error:
                logger.LogWarning(
                    """
                    Transcription error
                    - ID: {TranscriptId}
                    - Error: {transcript.Error}
                    """,
                    transcript.Id,
                    transcript.Error
                );
                return Ok();
            case TranscriptReadyStatus.Completed:
            {
                await System.IO.File.WriteAllTextAsync(
                    $"transcripts/{transcript.Id}.txt", 
                    transcript.Text
                );
                return Ok();
            }
            default:
                return BadRequest("Invalid status");
        }
    }
}
