using CorpseLib.Json;
using DiscordCorpse;
using DiscordNotifierPlugin.EmbedSettings;
using DiscordNotifierPlugin.MessageSettings;
using StreamGlass.Core.Plugin;

namespace DiscordNotifierPlugin
{
    public class DiscordNotifierPlugin : APlugin, ITestablePlugin
    {
        private readonly Core m_Core = new();

        public DiscordNotifierPlugin() : base("DiscordNotifier")
        {
            DiscordClientProtocol.StartLogging();
            DiscordAPI.StartLogging();
        }

        protected override PluginInfo GeneratePluginInfo() => new("1.0.0-beta", "ModuloCorpse<https://www.twitch.tv/chaporon_>");

        protected override void OnLoad()
        {
            JsonHelper.RegisterSerializer(new MessagePartSettingJsonSerializer());
            JsonHelper.RegisterSerializer(new EmbedAuthorSetting.JsonSerializer());
            JsonHelper.RegisterSerializer(new EmbedFieldSetting.JsonSerializer());
            JsonHelper.RegisterSerializer(new EmbedSetting.JsonSerializer());
            JsonHelper.RegisterSerializer(new NotificationSetting.JsonSerializer());
            JsonHelper.RegisterSerializer(new MessageSetting.JsonSerializer());
            JsonHelper.RegisterSerializer(new Settings.JsonSerializer());

            Settings? settings = JsonParser.LoadFromFile<Settings>(GetFilePath("settings.json"));
            if (settings != null)
                m_Core.SetSettings(settings);
        }

        protected override void OnInit() => m_Core.Connect();

        protected override void OnUnload() => m_Core.Disconnect();

        public void Test() => m_Core.Test();
    }
}
