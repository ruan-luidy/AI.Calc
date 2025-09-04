using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AI.Calc
{
  class Program
  {
    private static HttpClient httpClient = new HttpClient();
    private static IConfiguration configuration = null!;

    static async Task Main(string[] args)
    {
      configuration = new ConfigurationBuilder()
          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
          .Build();

      var apiKey = configuration["ClaudeApi:ApiKey"];
      var apiUrl = configuration["ClaudeApi:ApiUrl"];
      var model = configuration["ClaudeApi:Model"];
      var maxTokens = int.Parse(configuration["ClaudeApi:MaxTokens"] ?? "8000");

      if (string.IsNullOrEmpty(apiKey))
      {
        Console.WriteLine("Error: API Key not configured in appsettings.json");
        return;
      }

      httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);
      httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

      Console.WriteLine("╔══════════════════════════════════════════╗");
      Console.WriteLine("║       AI-POWERED SMART CALCULATOR       ║");
      Console.WriteLine("╚══════════════════════════════════════════╝");
      Console.WriteLine("\nType mathematical problems and I'll solve them!");
      Console.WriteLine("Examples: '2+2', 'square root of 20', 'what is 15% of 200'");
      Console.WriteLine("Type 'exit' to quit.\n");

      while (true)
      {
        Console.Write(">>> ");
        var input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
          continue;

        if (input.ToLower() == "exit" || input.ToLower() == "quit")
        {
          Console.WriteLine("\nGoodbye!");
          break;
        }

        try
        {
          Console.WriteLine("\nCalculating...");
          var result = await SolveWithClaude(input, apiUrl!, model!, maxTokens);
          Console.WriteLine($"\n📊 Result: {result}\n");
        }
        catch (Exception ex)
        {
          Console.WriteLine($"\n❌ Error: {ex.Message}\n");
        }
      }
    }

    static async Task<string> SolveWithClaude(string problem, string apiUrl, string model, int maxTokens)
    {
      var requestBody = new
      {
        model = model,
        max_tokens = maxTokens,
        messages = new[]
        {
          new
          {
            role = "user",
            content = $@"You are a precise mathematical calculator.
            Solve the following mathematical problem and return ONLY the numerical result or direct answer, without long explanations:

            {problem}
      
            If it's a simple calculation, return just the number.
            If it's something more complex, give a short and clear answer."
          }
        }
      };

      var json = JsonConvert.SerializeObject(requestBody);
      var content = new StringContent(json, Encoding.UTF8, "application/json");

      var response = await httpClient.PostAsync(apiUrl, content);

      if (!response.IsSuccessStatusCode)
      {
        var error = await response.Content.ReadAsStringAsync();
        throw new Exception($"API Error: {response.StatusCode} - {error}");
      }

      var responseContent = await response.Content.ReadAsStringAsync();
      dynamic? result = JsonConvert.DeserializeObject(responseContent);

      if (result?.content != null && result.content.Count > 0)
      {
        return result.content[0].text.ToString().Trim();
      }

      return "Could not calculate the result.";
    }
  }
}