using CorpseLib.DataNotation;
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
            DataHelper.RegisterSerializer(new MessagePartSettingDataSerializer());
            DataHelper.RegisterSerializer(new EmbedAuthorSetting.DataSerializer());
            DataHelper.RegisterSerializer(new EmbedFieldSetting.DataSerializer());
            DataHelper.RegisterSerializer(new EmbedSetting.DataSerializer());
            DataHelper.RegisterSerializer(new NotificationSetting.DataSerializer());
            DataHelper.RegisterSerializer(new MessageSetting.DataSerializer());
            DataHelper.RegisterSerializer(new Settings.DataSerializer());

            Settings? settings = JsonParser.LoadFromFile<Settings>(GetFilePath("settings.json"));
            if (settings != null)
                m_Core.SetSettings(settings);
        }

        protected override void OnInit() => m_Core.Connect();

        protected override void OnUnload() => m_Core.Disconnect();

        public void Test() => m_Core.Test();
    }
}
