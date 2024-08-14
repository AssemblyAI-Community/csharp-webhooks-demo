using AssemblyAI;
using AssemblyAI.Transcripts;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>()
    .Build();

var apiKey = config["AssemblyAI:ApiKey"] ?? throw new Exception("AssemblyAI:ApiKey is required");

const string fileName = "FILE_NAME_OR_PATH_HERE";
const string webhookUrl = "NGROK_URL_HERE/api/webhook";

var client = new AssemblyAIClient(apiKey);

Console.WriteLine("Uploading...");
var uploadStartTime = DateTime.Now;

var uploadedFile = await client.Files.UploadAsync(new FileInfo(fileName));
Console.WriteLine($"Uploaded file in {(DateTime.Now - uploadStartTime).TotalSeconds} seconds.");

var transcript = await client.Transcripts.SubmitAsync(new TranscriptParams
{
    AudioUrl = uploadedFile.UploadUrl,
    WebhookUrl = webhookUrl
});

Console.WriteLine($"Transcript ID: {transcript.Id}");