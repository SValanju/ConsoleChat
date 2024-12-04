using Spectre.Console;
using System.Text.Json;
using System.Text;
using ConsoleChat;
using System.IO;

const string apiKey = "API_KEY";
const string apiUrl = "https://api.cohere.com/v2/chat";
const string aiModel = "command-r-plus-08-2024";

AnsiConsole.Write(new FigletText("ConsoleChat").Centered().Color(Color.Yellow));
AnsiConsole.Write(new Text("Type '/q' to quit the program.\n", new Style(Color.Grey)).Centered());

while (true)
{
    AnsiConsole.Write(new Rule { Style = "grey" });

    var input = AnsiConsole.Ask<string>("[blue]You:[/]");

    AnsiConsole.Write(new Rule { Style = "grey dim" });

    if (input?.ToLower() == "/q") break;

    var responseText = await AnsiConsole.Status()
        .StartAsync("ConsoleChat is thinking...", async ctx =>
            await SendCohereChatRequest(input));

    AnsiConsole.Markup("[green]Assistant:[/]\n");

    if (responseText.Contains("error", StringComparison.InvariantCultureIgnoreCase))
        AnsiConsole.WriteException(new Exception(responseText));
    else
        AnsiConsole.WriteLine(responseText);
}

static async Task<string> SendCohereChatRequest(string userInput)
{
    using var httpClient = new HttpClient();

    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

    var payload = new
    {
        model = aiModel,
        stream = false,
        messages = new[]
        {
            new
            {
                role = "user",
                content = userInput
            }
        }
    };

    var jsonPayload = JsonSerializer.Serialize(payload);
    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

    try
    {
        var response = await httpClient.PostAsync(apiUrl, content);

        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();

            var parsedResponse = JsonSerializer.Deserialize<CohereChatResponse>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var assistantResponse = parsedResponse.Message.Content[0].Text;

            var usageInfo = $"Input Tokens: {parsedResponse.Usage.BilledUnits.InputTokens}, Output Tokens: {parsedResponse.Usage.BilledUnits.OutputTokens}";

            return $"{assistantResponse}\nUsage: {usageInfo}";
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            return $"Error: {response.StatusCode}, {error}";
        }
    }
    catch (Exception ex)
    {
        return $"An error occurred: {ex.Message}";
    }
}