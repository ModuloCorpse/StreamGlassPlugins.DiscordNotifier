using CorpseLib.Ini;
using CorpseLib.Json;
using CorpseLib.Web.API;
using DiscordCorpse;
using StreamGlass.Core.Plugin;
using StreamGlass.Core.Settings;

namespace DiscordNotifierPlugin
{
    public class DiscordNotifierPlugin : APlugin
    {
        private readonly DiscordNotifierCore m_Core = new();

        public DiscordNotifierPlugin() : base("DiscordNotifier", "discord_notifier_settings.ini")
        {
            DiscordClientProtocol.StartLogging();
            DiscordAPI.StartLogging();
        }

        protected override PluginInfo GeneratePluginInfo() => new("1.0.0-beta", "ModuloCorpse<https://www.twitch.tv/chaporon_>");

        protected override void InitTranslation() { }

        protected override void InitSettings()
        {
            IniSection settings = m_Settings.GetOrAdd("settings");
            settings.Add("delay_since_message", "0");
            settings.Add("delay_since_start", "0");
            settings.Add("token", string.Empty);
            m_Core.SetSettings(settings, JsonParser.LoadFromFile(GetFilePath("message.json")));
        }

        protected override void InitPlugin() => m_Core.Connect();

        protected override void InitCommands() { }

        protected override void InitCanals() { }

        protected override AEndpoint[] GetEndpoints() => [];

        protected override void Unregister() => m_Core.Disconnect();

        protected override void Update(long deltaTime) { }

        protected override TabItemContent[] GetSettings() => [];

        protected override void TestPlugin() => m_Core.Test();
    }
}
