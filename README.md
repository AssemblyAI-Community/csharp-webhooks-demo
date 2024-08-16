# AssemblyAI Webhooks C# Demo

## Setting up the Project

This project requires the [ngrok](https://formulae.brew.sh/cask/ngrok) library to be installed for creating a publicly-accessible webhook URL that forwards to your local machine. You'll also need to ensure you have the [dotnet](https://formulae.brew.sh/cask/dotnet) library installed, or another version of the .NET 8 CLI available to build and run this project.

## Running ngrok

To start, you'll need to create a publicly-accessible Webhook URL that our API can reach. Normally this would be your own Webhook server, but for the purposes of this demo, we're using Ngrok to set up forwarding to our `localhost` domain. Once you've installed `ngrok`, simply run `ngrok http 8000` to start up an ngrok server that points to your machine on port 8000. 

You can verify that this is the case by looking at the "Forwarding" section of the output in your terminal window. You should see `http://localhost:8000` on the right side of the arrow, while the URL on the left of the arrow is the URL you're going to need for later. Copy that now and have it ready to use.

## Important Variables

First, you need to configure the AssemblyAI API key for both projects. 
Both projects are configured to look for the `AssemblyAI:ApiKey` key in the environment variables and user-secrets.

To set the API key in the user-secrets, run the commands below in the terminal, replacing `<YOUR_API_KEY>` with your AssemblyAI API key, which can be located on your [account dashboard](https://www.assemblyai.com/app/account).
```bash
dotnet user-secrets --project TranscriptionSubmitter set AssemblyAI:ApiKey "<YOUR_API_KEY>"
dotnet user-secrets --project WebhookReceiver set AssemblyAI:ApiKey "<YOUR_API_KEY>"
```

Next you'll want to replace the 2 variables `fileName` and `webhookUrl` under `TranscriptionSubmitter/Program.cs` with your audio file name/path and Ngrok URL from earlier, respectively. Make sure to retain the `/api/webhook` ending to your Ngrok URL, as otherwise you'll encounter a 404 error when Webhook responses are received.

For instance, your ngrok URL should be something like `https://09e2-2600-6c52-427f-a41f-1923-ce-c982-3f01.ngrok-free.app/api/webhook`. Now you're ready to run these two programs!

## Running the Webhook Server

In another terminal window (ensure that your ngrok program from before is still running), navigate into the `TranscriptionSubmitter` folder. Then run `dotnet run`. Now you'll see in the terminal output that your Webhook server is listening on port 8000, which is where your Webhook calls will be forwarded to.

## Running the TranscriptionSubmitter

In a third terminal window, navigate into the `WebhookReceiver` folder. Then run `dotnet run`. This will submit the file you specified under `fileName` to our API using your Ngrok URL as your Webhook URL. You'll get back a transcript ID to confirm that your file was successfully uploaded and provided to our API.

## Accepting the Webhook Response

After the file finishes processing, your Webhook server will be called with a payload containing the status of the transcript. If it's `completed`, we'll make a GET request to fetch the full transcript text and write it to a file named `transcripts/[id].txt`. If it's `error`, we'll log an error message to the terminal letting you know that the transcript failed.

Congrats! You've now transcribed a file with the AssemblyAI API using Webhooks in C#.

## Further Help

If anything happens to go wrong, or you have any questions on this project, please feel free to reach out to support@assemblyai.com. Happy coding!
