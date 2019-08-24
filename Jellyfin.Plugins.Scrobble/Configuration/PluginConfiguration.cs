using System.Collections.Generic;
using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugins.Scrobble.Configuration
{
    public class ScrobbleProvider
    {
        public string Name { get; set; }
        public string AccessToken { get; set; }
        public bool Enabled { get; set; }
    }

    public class UserConfiguration
    {
        public UserConfiguration()
        {
            this.Providers = new[] { new ScrobbleProvider { Name = "Last.fm" } };
        }

        public string UserId { get; set; }
        public ScrobbleProvider[] Providers { get; set; }
        public string AccessToken { get; set; }
        public bool Scrobble { get; set; }
    }

    public class PluginConfiguration : BasePluginConfiguration
    {
        public PluginConfiguration()
        {
            this.UserConfigurations = new UserConfiguration[] {};
        }

        public UserConfiguration[] UserConfigurations { get; set; }
    }
}
