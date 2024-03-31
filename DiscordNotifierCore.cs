using CorpseLib.Ini;
using CorpseLib.Json;
using CorpseLib.Network;
using CorpseLib.Placeholder;
using DiscordCorpse;
using StreamGlass.Core;

namespace DiscordNotifierPlugin
{
    public class DiscordNotifierCore : IDiscordHandler
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        private DiscordClient m_DiscordClient = null;
        private IniSection m_Settings = null;
        private JsonObject m_MessageJson = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        private DateTime m_LastStreamStart = DateTime.MinValue;
        private DateTime m_LastMessagePosted = DateTime.MinValue;
        private bool m_IsConnected = false;

        internal void SetSettings(IniSection settings, JsonObject messageSerializer)
        {
            m_Settings = settings;
            m_MessageJson = messageSerializer;
        }

        public void Connect()
        {
            if (!m_IsConnected)
            {
                string discordToken = m_Settings.Get("token");
                if (!string.IsNullOrEmpty(discordToken))
                {
                    m_DiscordClient = DiscordClient.NewConnection(discordToken, this, new DebugLogMonitor(DiscordClientProtocol.DISCORD_GATEWAY));
                    m_IsConnected = true;
                }
            }
        }

        public void Disconnect()
        {
            if (m_IsConnected)
            {
                m_DiscordClient.Disconnect();
                m_IsConnected = false;
            }
        }

        public void OnReady()
        {
            StreamGlassCanals.Register("twitch_stream_start", OnStreamStart);
        }

        private void PostMessages(bool isTest)
        {
            Dictionary<string, DiscordMessage> messagesToPost = [];
            DiscordMessageBuilder builder = new(new StreamGlassContext());
            List<JsonObject> messages = m_MessageJson.GetList<JsonObject>("messages");
            foreach (JsonObject message in messages)
            {
                if (message.TryGet("id", out string? id) && id != null &&
                    message.TryGet("content", out JsonObject? content) && content != null)
                    messagesToPost[id] = builder.Build(content);
            }

            List<JsonObject> notifications = m_MessageJson.GetList<JsonObject>((isTest) ? "tests" : "notifications");
            foreach (JsonObject notification in notifications)
            {
                List<string> channels = notification.GetList<string>("channels");
                if (channels.Count > 0 && notification.TryGet("content", out string? content) && content != null)
                {
                    if (messagesToPost.TryGetValue(content, out DiscordMessage? discordMessage))
                    {
                        bool crossPost = false;
                        if (notification.TryGet("crosspost", out bool? crosspost) && crosspost == true)
                            crossPost = true;
                        foreach (string channel in channels)
                        {
                            if (crossPost)
                                m_DiscordClient.CrossPostMessage(channel, discordMessage);
                            else
                                m_DiscordClient.SendMessage(channel, discordMessage);
                        }
                    }
                }
            }
        }

        private void OnStreamStart()
        {
            if (!int.TryParse(m_Settings.Get("delay_since_start"), out int delaySinceStart))
                delaySinceStart = 0;
            if (!int.TryParse(m_Settings.Get("delay_since_message"), out int delaySinceMessage))
                delaySinceMessage = 0;
            DateTime now = DateTime.Now;
            if ((now - m_LastStreamStart).TotalSeconds >= delaySinceStart &&
                (now - m_LastMessagePosted).TotalSeconds >= delaySinceMessage)
            {
                PostMessages(false);
                m_LastMessagePosted = now;
            }
            m_LastStreamStart = now;
        }

        public void Test() => PostMessages(true);

        public void OnMessageCreate(DiscordReceivedMessage message) { }
    }
}
