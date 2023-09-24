using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
            // Validate input URL
            if (string.IsNullOrEmpty(model.LongUrl))
            {
                throw new ArgumentException("The original URL string is empty or null.");
            }
            
            if (!Uri.IsWellFormedUriString(model.LongUrl, UriKind.Absolute))
            {
                throw new FormatException("The original URL format is invalid.");
            }
            
            // Retrieve configuration
            var baseUrl = _configuration.GetValue<string>("ShrinkUrlSettings:BaseUrl");
            var maxLength = _configuration.GetValue<int>("ShrinkUrlSettings:MaxLength");

            // Generate a random string for uniqueness
            var randomString = GenerateRandomString(maxLength);
            
            // Encrypt the random string for additional security
            var encryptedString = Cryptography.EncryptUrl(randomString, maxLength);

            // Construct the final short URL
            var shortUrl = $"{baseUrl}{encryptedString}";
            return Ok(shortUrl);
        }
        
        /// <summary>
        /// Generates a random alphanumeric string of the specified length.
        /// </summary>
        /// <param name="stringLength">The length of the generated random string.</param>
        /// <returns>A random alphanumeric string of the specified length.</returns>
        /// <example>
        /// GenerateRandomString(6) could return "Abc123".
        /// </example>
        private static string GenerateRandomString(int stringLength)
        {
            // Allowed characters
            string allowed = "ABCDEFGHIJKLMONOPQRSTUVWXYZabcdefghijklmonopqrstuvwxyz0123456789";

            char[] randomChars = new char[stringLength];

            for (int i = 0; i < stringLength; i++)
            {
                randomChars[i] = allowed[RandomNumberGenerator.GetInt32(0, allowed.Length)];
            }

            return new string(randomChars);
        }
    }
}
