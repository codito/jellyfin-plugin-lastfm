using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;

using Jellyfin.Plugins.Scrobble.Api.Models;
using System.Runtime.Serialization.Json;
using MediaBrowser.Model.Serialization;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugins.Scrobble.Api
{
    public class LastfmApi
    {
        private const string API_KEY = "9e35f765b0421798463cc5dfd7be7808";
        private const string API_SECRET = "5a00bfc5d99d5abd90294d085da4900c";
        private IJsonSerializer serializer;
        private ILogger logger;

        public LastfmApi(IJsonSerializer serializer, ILogger logger)
        {
            this.serializer = serializer;
            this.logger = logger;
        }

        public async Task<string> CreateToken(string username, string password)
        {
            try
            {
                var parameters = new Dictionary<string, string> { { "username", username }, { "password", password } };
                var client = new HttpClient();
                var content = this.CreatePostBody("auth.getMobileSession", parameters);
                var requestUri = "https://ws.audioscrobbler.com/2.0/";
                var response = await client.PostAsync(requestUri, content);

                System.Console.WriteLine(response.ToString());
                System.Console.WriteLine(response.Content.ReadAsStringAsync().Result);

                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    var authResponse = this.serializer.DeserializeFromStream<AuthTokenResponse>(stream);

                    return authResponse.session.key;
                }
            }
            catch (HttpRequestException)
            {
                throw;
            }
        }

        public HttpContent CreatePostBody(string method, Dictionary<string, string> parameters)
        {
            var content = new Dictionary<string, string>(parameters);
            content["api_key"] = API_KEY;
            content["method"] = method;
            content["api_sig"] = this.CreateSignature(content);

            // API signature must not include format or callback
            content["format"] = "json";

            return new FormUrlEncodedContent(content);
        }

        /// <summary>
        /// Create an API signature for the request.
        /// </summary>
        /// <remarks>See section 8 at https://www.last.fm/api/authspec</summary>
        private string CreateSignature(Dictionary<string, string> parameters)
        {
            // Order the parameters alphabetically and concatenate them in `<name><value>` scheme
            var builder = new StringBuilder();
            foreach (var item in parameters.OrderBy(kv => kv.Key))
            {
                builder.AppendFormat("{0}{1}", item.Key, item.Value);
            }

            System.Console.WriteLine(builder.ToString());
            // Concatenate the secret and build a md5 hash
            builder.Append(API_SECRET);
            using (MD5 md5Hash = MD5.Create())
            {
                return GetMd5Hash(md5Hash, builder.ToString());
            }
        }

        private static string GetMd5Hash(MD5 md5Hash, string input)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var builder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                builder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return builder.ToString();
        }
    }
}
