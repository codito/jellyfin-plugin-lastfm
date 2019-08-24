using System.Runtime.Serialization;

namespace Jellyfin.Plugins.Scrobble.Api.Models
{
    public class Session
    {
        public int subscriber { get; set; }

        public string name { get; set; }

        public string key { get; set; }
    }

    public class AuthTokenResponse
    {
        public Session session { get; set; }
    }

    public class ScrobbleResponse
    {

    }
}
