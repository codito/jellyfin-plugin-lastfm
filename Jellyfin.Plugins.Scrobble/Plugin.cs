namespace Jellyfin.Plugins.Scrobble
{
    using System;
    using System.Collections.Generic;
    using Jellyfin.Plugins.Scrobble.Configuration;
    using MediaBrowser.Common.Configuration;
    using MediaBrowser.Common.Plugins;
    using MediaBrowser.Model.Plugins;
    using MediaBrowser.Model.Serialization;

    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer) : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
        }

        public static Plugin Instance { get; private set; }

        public override string Name => "Scrobble";

        public override Guid Id => Guid.Parse("f41f8b8a-8f9b-410f-a45f-e564f0bd0ece");

        public PluginConfiguration PluginConfiguration => this.Configuration;

        public IEnumerable<PluginPageInfo> GetPages()
        {
            return new[]
            {
                new PluginPageInfo
                {
                    Name = this.Name,
                    EmbeddedResourcePath = string.Format("{0}.Configuration.configPage.html", this.GetType().Namespace),
                }
            };
        }
    }
}
