using Xunit;
using FluentAssertions;
using Moq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using Moq.Protected;
using Newtonsoft.Json;
using System.Text;

namespace AI.Calc.Tests
{
  public class MemeCalculatorTests
  {
    [Fact]
    public async Task QuickMafs_TwoPlusTwoIsMinusOneThatsFree()
    {
      // Arrange - Man's not hot
      var mockResponse = CreateMockResponse("3");
      var httpClient = CreateMockHttpClient(mockResponse);

      // Act - QUICK MAFFS
      var result = await CallClaudeWithMock(httpClient, "2+2-1");

      // Assert - That's free, quick maffs
      result.Should().Be("3");
    }

    [Fact]
    public async Task NiceNumber_Returns420BlazeIt()
    {
      // Arrange - Smoke weed everyday
      var mockResponse = CreateMockResponse("420");
      var httpClient = CreateMockHttpClient(mockResponse);

      // Act 
      var result = await CallClaudeWithMock(httpClient, "421 - 1");

      // Assert - Nice
      result.Should().Be("420");
    }

    [Fact]
    public async Task ElonMusk_Returns69Nice()
    {
      // Arrange - hehe funny number
      var mockResponse = CreateMockResponse("69");
      var httpClient = CreateMockHttpClient(mockResponse);

      // Act
      var result = await CallClaudeWithMock(httpClient, "nice number");

      // Assert - Nice
      result.Should().Be("69");
    }

    [Fact]
    public async Task OverNineThousand_ItsActually9001()
    {
      // Arrange - WHAT?! NINE THOUSAND?!
      var mockResponse = CreateMockResponse("9001");
      var httpClient = CreateMockHttpClient(mockResponse);

      // Act - IT'S OVER 9000!!!
      var result = await CallClaudeWithMock(httpClient, "9000 + 1");

      // Assert - Vegeta intensifies
      result.Should().Be("9001");
    }

    [Fact]
    public async Task TreeFiddy_LochNessMonsterNeeds350()
    {
      // Arrange - God damn Loch Ness Monster!
      var mockResponse = CreateMockResponse("3.50");
      var httpClient = CreateMockHttpClient(mockResponse);

      // Act - I ain't giving you no tree fiddy!
      var result = await CallClaudeWithMock(httpClient, "How much does the Loch Ness Monster need?");

      // Assert - It was about that time I realized...
      result.Should().Be("3.50");
    }

    [Fact]
    public async Task Stonks_OnlyGoUp()
    {
      // Arrange - 📈 STONKS
      var mockResponse = CreateMockResponse("TO THE MOON 🚀");
      var httpClient = CreateMockHttpClient(mockResponse);

      // Act - Diamond hands 💎🙌
      var result = await CallClaudeWithMock(httpClient, "GME price prediction?");

      // Assert - Apes together strong
      result.Should().Contain("MOON");
    }

    [Fact]
    public async Task DivideByZero_ReturnsInfinity()
    {
      // Arrange - Chuck Norris can divide by zero
      var mockResponse = CreateMockResponse("∞");
      var httpClient = CreateMockHttpClient(mockResponse);

      // Act - Breaking mathematics
      var result = await CallClaudeWithMock(httpClient, "1/0");

      // Assert - Mind blown
      result.Should().Be("∞");
    }

    [Fact]
    public async Task Answer_ToLifeUniverseEverything_Is42()
    {
      // Arrange - Deep Thought calculated for 7.5 million years
      var mockResponse = CreateMockResponse("42");
      var httpClient = CreateMockHttpClient(mockResponse);

      // Act - Don't Panic
      var result = await CallClaudeWithMock(httpClient, "What is the answer to life, universe and everything?");

      // Assert - So long and thanks for all the fish
      result.Should().Be("42");
    }

    [Fact]
    public async Task PiNumber_Returns314Approximately()
    {
      // Arrange - π is exactly 3!
      var mockResponse = CreateMockResponse("3.14159");
      var httpClient = CreateMockHttpClient(mockResponse);

      // Act - Engineers: π = 3, take it or leave it
      var result = await CallClaudeWithMock(httpClient, "value of pi");

      // Assert 
      result.Should().StartWith("3.14");
    }

    [Fact]
    public async Task ServerError_ReturnsNoStonks()
    {
      // Arrange - 📉 Not stonks
      var httpClient = CreateMockHttpClient(null, HttpStatusCode.InternalServerError);

      // Act & Assert - F in the chat
      var act = async () => await CallClaudeWithMock(httpClient, "calculate my losses");
      await act.Should().ThrowAsync<Exception>()
          .WithMessage("*API Error*");
    }

    [Fact]
    public async Task InvalidApiKey_Returns401Unauthorized()
    {
      // Arrange - You shall not pass!
      var httpClient = CreateMockHttpClient(null, HttpStatusCode.Unauthorized);

      // Act & Assert - Gandalf says no
      var act = async () => await CallClaudeWithMock(httpClient, "1+1");
      await act.Should().ThrowAsync<Exception>()
          .WithMessage("*Unauthorized*");
    }

    [Fact]
    public async Task RickRoll_NeverGonnaGiveYouUp()
    {
      // Arrange - We're no strangers to love
      var mockResponse = CreateMockResponse("Never gonna give you up, never gonna let you down");
      var httpClient = CreateMockHttpClient(mockResponse);

      // Act - You know the rules and so do I
      var result = await CallClaudeWithMock(httpClient, "Rick Astley's famous equation");

      // Assert - A full commitment's what I'm thinking of
      result.Should().Contain("Never gonna");
    }

    [Fact]
    public async Task ThisIsSparta_Returns300()
    {
      // Arrange - SPARTANS!
      var mockResponse = CreateMockResponse("300");
      var httpClient = CreateMockHttpClient(mockResponse);

      // Act - THIS IS SPARTA! *kicks*
      var result = await CallClaudeWithMock(httpClient, "How many Spartans?");

      // Assert - Tonight we dine in hell!
      result.Should().Be("300");
    }

    [Fact]
    public async Task OrderSixtySix_ExecuteIt()
    {
      // Arrange - The time has come
      var mockResponse = CreateMockResponse("66");
      var httpClient = CreateMockHttpClient(mockResponse);

      // Act - DEW IT!
      var result = await CallClaudeWithMock(httpClient, "Execute order?");

      // Assert - It will be done, my lord
      result.Should().Be("66");
    }

    [Fact]
    public async Task MatrixChoice_RedPillOrBluePill()
    {
      // Arrange - Wake up Neo...
      var mockResponse = CreateMockResponse("There is no spoon");
      var httpClient = CreateMockHttpClient(mockResponse);

      // Act - Follow the white rabbit
      var result = await CallClaudeWithMock(httpClient, "What is the Matrix?");

      // Assert - Welcome to the real world
      result.Should().Contain("spoon");
    }

    // Helper Methods - The backend of memes
    private HttpClient CreateMockHttpClient(string responseContent, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
      var mockHandler = new Mock<HttpMessageHandler>();

      var response = new HttpResponseMessage(statusCode);
      if (responseContent != null)
      {
        var apiResponse = new
        {
          content = new[]
            {
                        new { text = responseContent }
                    }
        };
        response.Content = new StringContent(JsonConvert.SerializeObject(apiResponse), Encoding.UTF8, "application/json");
      }

      mockHandler.Protected()
          .Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.IsAny<HttpRequestMessage>(),
              ItExpr.IsAny<CancellationToken>())
          .ReturnsAsync(response);

      return new HttpClient(mockHandler.Object);
    }

    private string CreateMockResponse(string text)
    {
      return text;
    }

    private async Task<string> CallClaudeWithMock(HttpClient httpClient, string problem)
    {
      var requestBody = new
      {
        model = "claude-3-sonnet",
        max_tokens = 100,
        messages = new[]
          {
                    new
                    {
                        role = "user",
                        content = problem
                    }
                }
      };

      var json = JsonConvert.SerializeObject(requestBody);
      var content = new StringContent(json, Encoding.UTF8, "application/json");

      var response = await httpClient.PostAsync("https://api.anthropic.com/v1/messages", content);

      if (!response.IsSuccessStatusCode)
      {
        throw new Exception($"API Error: {response.StatusCode}");
      }

      var responseContent = await response.Content.ReadAsStringAsync();
      dynamic result = JsonConvert.DeserializeObject(responseContent);

      if (result?.content != null && result.content.Count > 0)
      {
        return result.content[0].text.ToString().Trim();
      }

      return "Could not calculate the result.";
    }
  }
}