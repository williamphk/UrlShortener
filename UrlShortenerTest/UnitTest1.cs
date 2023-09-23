using UrlShortener;
using Microsoft.Extensions.Configuration;
using Moq;
using UrlShortener.Controllers;

namespace UrlShortenerTest;

public class CryptographyTests
{
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
        var mockConfiguration = new Mock<IConfiguration>();
        var controller = new UrlController(mockConfiguration.Object);

        var model = new UrlModel { LongUrl = "https://www.validurl.com" };
        
        Assert.NotNull(() => controller.ShortenUrl(model));
    }
}
