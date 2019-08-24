namespace Jellyfin.Plugins.Scrobble.Api
{
    using System.Linq;
    using Jellyfin.Plugins.Scrobble.Configuration;
    using MediaBrowser.Model.Serialization;
    using MediaBrowser.Model.Services;
    using Microsoft.Extensions.Logging;

    [Route("/Scrobble/Users/{UserId}/Authorize", "POST")]
    public class DeviceAuthorization
    {
        [ApiMember(Name = "UserId", Description = "User Id", IsRequired = true, DataType = "string", ParameterType = "path", Verb = "POST")]
        public string UserId { get; set; }

        [ApiMember(Name = "Provider", Description = "Scrobble service provider", IsRequired = true, DataType = "string", ParameterType = "body", Verb = "POST")]
        public string Provider { get; set; }

        [ApiMember(Name = "Username", Description = "Username", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "POST")]
        public string Username { get; set; }

        [ApiMember(Name = "Password", Description = "Password", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "POST")]
        public string Password { get; set; }
    }

    public class ServerApiEndpoints : IService
    {
        private LastfmApi provider;

        public ServerApiEndpoints(
            IJsonSerializer jsonSerializer,
            ILogger logger)
        {
            this.provider = new LastfmApi(jsonSerializer, logger);
        }

        public object Post(DeviceAuthorization authRequest)
        {
            var user = GetUserConfiguration(authRequest.UserId);
            var accessToken = this.provider.CreateToken(authRequest.Username, authRequest.Password).Result;

            user.AccessToken = accessToken;
            Plugin.Instance.SaveConfiguration();

            return new { accessToken };
        }

        private static UserConfiguration GetUserConfiguration(string userId)
        {
            var user = Plugin.Instance.PluginConfiguration.UserConfigurations
            .FirstOrDefault<UserConfiguration>(u => u.UserId.Equals(userId));

            if (user == null)
            {
                user = new UserConfiguration()
                {
                    UserId = userId,
                };
            }

            return user;
        }
    }
}
