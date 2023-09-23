using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UrlShortener.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrlController : ControllerBase
    {
        private readonly IConfiguration _configuration;
    
        public UrlController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        /// <summary>
        /// Generates a shortened URL based on the provided model containing the original URL.
        /// Throws exceptions for null, empty, or poorly-formatted original URLs.
        /// </summary>
        /// <param name="model">An object containing the original long URL.</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: a shortened url.
        /// Throws ArgumentException if the original URL string is empty or null.
        /// Throws FormatException if the original URL string is invalid.
        /// </returns>
        /// <example>
        /// POST: api/Url with a JSON body containing { "LongUrl": "https://www.example.com" }
        /// Returns: 200 OK and "www.example.co/123abc"
        /// </example>
        [HttpPost]
        public IActionResult ShortenUrl(UrlModel model)
        {
            if (string.IsNullOrEmpty(model.LongUrl))
            {
                throw new ArgumentException("The original URL string is empty or null.");
            }
            
            if (!Uri.IsWellFormedUriString(model.LongUrl, UriKind.Absolute))
            {
                throw new FormatException("The original URL format is invalid.");
            }
            
            var baseUrl = _configuration.GetValue<string>("ShrinkUrlSettings:BaseUrl");
            var maxLength = _configuration.GetValue<int>("ShrinkUrlSettings:MaxLength");
            
            var shortUrl = GenerateShortUrl(model.LongUrl, baseUrl, maxLength);
            return Ok(shortUrl);
        }

        /// <summary>
        /// Generates a shortened URL based on the given long URL, base URL, and maximum length.
        /// </summary>
        /// <param name="longUrl">The original long URL to be shortened.</param>
        /// <param name="baseUrl">The base URL to be used for generating the shortened URL.</param>
        /// <param name="maxLength">The maximum length for the encrypted portion of the shortened URL.</param>
        /// <returns>
        /// Returns a shortened URL combining the base URL and a truncated, encrypted form of the original URL.
        /// </returns>
        /// <example>
        /// var shortUrl = GenerateShortUrl("https://www.google.com", "https://example.co", 6);
        /// // Result could be "https://example.co/abcd12"
        /// </example>
        private static string GenerateShortUrl(string longUrl, string baseUrl, int maxLength)
        {
            var encryptedUrl = Cryptography.EncryptUrl(longUrl, maxLength);
            return $"{baseUrl}{encryptedUrl}";
        }
    }
}
