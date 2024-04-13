using CorpseLib.Network;
using DiscordCorpse;
using StreamGlass.Core;

namespace DiscordNotifierPlugin
{
    public class Core : IDiscordHandler
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        private DiscordClient m_DiscordClient = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        private Settings m_Settings = new();
        private DateTime m_LastStreamStart = DateTime.MinValue;
        private DateTime m_LastMessagePosted = DateTime.MinValue;
        private bool m_IsConnected = false;

        internal void SetSettings(Settings settings) => m_Settings = settings;

        public void Connect()
        {
            if (!m_IsConnected)
            {
                string discordToken = m_Settings.Token;
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
            MessageBuilder builder = new(new StreamGlassContext());
            foreach (MessageSetting message in m_Settings.Messages)
                messagesToPost[message.ID] = builder.Build(message);

            IEnumerable<NotificationSetting> notifications = (isTest) ? m_Settings.TestNotifications : m_Settings.Notifications;
            foreach (NotificationSetting notification in notifications)
            {
                if (notification.Channels.Count() > 0)
                {
                    if (messagesToPost.TryGetValue(notification.Content, out DiscordMessage? discordMessage))
                    {
                        foreach (string channel in notification.Channels)
                        {
                            if (notification.Crosspost)
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
            int delaySinceStart = m_Settings.DelaySinceStart;
            int delaySinceMessage = m_Settings.DelaySinceMessage;
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
