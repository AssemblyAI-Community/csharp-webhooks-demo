using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    private static readonly string BASE_URL = "https://api.assemblyai.com/v2/";
    private static readonly string API_KEY = "API_KEY_HERE";
    private static readonly string FILE_NAME = "FILE_NAME_OR_PATH_HERE";
    private static readonly string WEBHOOK_URL = "NGROK_URL_HERE/api/webhook";

    static async Task Main(string[] args)
    {
        using HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("authorization", API_KEY);

        Console.WriteLine("Uploading...");
        var uploadStartTime = DateTime.Now;

        var audioUrl = await UploadFile(client, FILE_NAME);
        Console.WriteLine($"Uploaded file in {(DateTime.Now - uploadStartTime).TotalSeconds} seconds.");

        var json = new { audio_url = audioUrl, webhook_url = WEBHOOK_URL };

        var response = await PostAsync(client, "transcript", json);

        if (!response.ContainsKey("id"))
        {
            Console.WriteLine(JsonSerializer.Serialize(response));
        }
        else
        {
            var id = response["id"].ToString();
            Console.WriteLine($"Transcript ID: {id}");
        }
    }

    private static async Task<string> UploadFile(HttpClient client, string fileName)
    {
        using var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        var content = new StreamContent(fileStream);
        content.Headers.Add("Content-Type", "application/octet-stream");

        var response = await client.PostAsync(BASE_URL + "upload", content);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonResponse);
        return result["upload_url"];
    }

    private static async Task<Dictionary<string, object>> PostAsync(HttpClient client, string endpoint, object json)
    {
        var jsonContent = new StringContent(JsonSerializer.Serialize(json), System.Text.Encoding.UTF8, "application/json");
        var response = await client.PostAsync(BASE_URL + endpoint, jsonContent);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse);
    }
}