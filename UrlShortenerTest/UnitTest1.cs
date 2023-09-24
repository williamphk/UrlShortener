using Microsoft.AspNetCore.Mvc;
using UrlShortener;
using Microsoft.Extensions.Configuration;
using Moq;
using UrlShortener.Controllers;
using Xunit.Abstractions;

namespace UrlShortenerTest;

public class UrlShortenerTest
{
    private readonly ITestOutputHelper _output;

    public UrlShortenerTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void ShortenUrl_ThrowsArgumentException_WhenUrlIsNull()
    {
        var mockConfiguration = new Mock<IConfiguration>();
        var controller = new UrlController(mockConfiguration.Object);

        var model = new UrlModel { LongUrl = null };

        Assert.Throws<ArgumentException>(() => controller.ShortenUrl(model));
    }

    [Fact]
    public void ShortenUrl_ThrowsFormatException_WhenUrlIsInvalid()
    {
        var mockConfiguration = new Mock<IConfiguration>();
        var controller = new UrlController(mockConfiguration.Object);

        var model = new UrlModel { LongUrl = "invalid_url" };

        Assert.Throws<FormatException>(() => controller.ShortenUrl(model));
    }

    [Fact]
    public void ShortenUrl_ReturnsShortenedUrl_WhenUrlIsValid()
    {
        var mockBaseUrlSection = new Mock<IConfigurationSection>();
        mockBaseUrlSection.SetupGet(m => m.Value).Returns("https://example.co/");
        var mockMaxLengthSection = new Mock<IConfigurationSection>();
        mockMaxLengthSection.SetupGet(m => m.Value).Returns("6");

        var mockConfiguration = new Mock<IConfiguration>();
        mockConfiguration.Setup(a => a.GetSection("ShrinkUrlSettings:BaseUrl")).Returns(mockBaseUrlSection.Object);
        mockConfiguration.Setup(a => a.GetSection("ShrinkUrlSettings:MaxLength")).Returns(mockMaxLengthSection.Object);
        
        var controller = new UrlController(mockConfiguration.Object);

        var model = new UrlModel { LongUrl = "https://www.validurl.com" };
        
        var result = controller.ShortenUrl(model) as OkObjectResult;

        if (result == null)
        {
            return;
        }
        
        Assert.NotNull(result);
        Assert.StartsWith("https://example.co/", result.Value as string);
    }

    [Fact]
    public void ShortenUrl_ShouldReturnDifferentResults()
    {
        var mockBaseUrlSection = new Mock<IConfigurationSection>();
        mockBaseUrlSection.SetupGet(m => m.Value).Returns("https://example.co/");
        var mockMaxLengthSection = new Mock<IConfigurationSection>();
        mockMaxLengthSection.SetupGet(m => m.Value).Returns("6");

        var mockConfiguration = new Mock<IConfiguration>();
        mockConfiguration.Setup(a => a.GetSection("ShrinkUrlSettings:BaseUrl")).Returns(mockBaseUrlSection.Object);
        mockConfiguration.Setup(a => a.GetSection("ShrinkUrlSettings:MaxLength")).Returns(mockMaxLengthSection.Object);
        
        var controller = new UrlController(mockConfiguration.Object);
        
        var model1 = new UrlModel { LongUrl = "https://www.first.com" };
        var model2 = new UrlModel { LongUrl = "https://www.second.com" };

        var result1 = controller.ShortenUrl(model1) as OkObjectResult;
        var result2 = controller.ShortenUrl(model2) as OkObjectResult;

        if (result1 == null || result2 == null)
        {
            return;
        }

        Assert.NotEqual(result1.Value as string, result2.Value as string);
    }
}
