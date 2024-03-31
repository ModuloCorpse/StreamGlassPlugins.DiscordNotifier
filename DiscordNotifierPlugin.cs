using CorpseLib.Ini;
using CorpseLib.Json;
using DiscordCorpse;
using StreamGlass.Core.Plugin;

namespace DiscordNotifierPlugin
{
    public class DiscordNotifierPlugin : APlugin, ITestablePlugin
    {
        private readonly DiscordNotifierCore m_Core = new();

        public DiscordNotifierPlugin() : base("DiscordNotifier", "discord_notifier_settings.ini")
        {
            DiscordClientProtocol.StartLogging();
            DiscordAPI.StartLogging();
        }

        protected override PluginInfo GeneratePluginInfo() => new("1.0.0-beta", "ModuloCorpse<https://www.twitch.tv/chaporon_>");

        protected override void OnLoad()
        {
            IniSection settings = m_Settings.GetOrAdd("settings");
            settings.Add("delay_since_message", "0");
            settings.Add("delay_since_start", "0");
            settings.Add("token", string.Empty);
            m_Core.SetSettings(settings, JsonParser.LoadFromFile(GetFilePath("message.json")));
        }

        protected override void OnInit() => m_Core.Connect();

        protected override void OnUnload() => m_Core.Disconnect();

        public void Test() => m_Core.Test();
    }
}
