using CorpseLib;
using CorpseLib.Json;
using System.Runtime;

namespace DiscordNotifierPlugin
{
    public class Settings
    {
        public class JsonSerializer : AJsonSerializer<Settings>
        {
            protected override OperationResult<Settings> Deserialize(JsonObject reader)
            {
                Settings settings = new()
                {
                    m_Token = reader.GetOrDefault("token", string.Empty),
                    m_DelaySinceStart = reader.GetOrDefault("delay_since_start", 0),
                    m_DelaySinceMessage = reader.GetOrDefault("delay_since_message", 0)
                };
                settings.m_Messages.AddRange(reader.GetList<MessageSetting>("messages"));
                settings.m_Notifications.AddRange(reader.GetList<NotificationSetting>("notifications"));
                settings.m_TestNotifications.AddRange(reader.GetList<NotificationSetting>("tests"));
                return new(settings);
            }

            protected override void Serialize(Settings obj, JsonObject writer)
            {
                writer["token"] = obj.m_Token;
                writer["delay_since_start"] = obj.m_DelaySinceStart;
                writer["delay_since_message"] = obj.m_DelaySinceMessage;
                writer["messages"] = obj.m_Messages;
                writer["notifications"] = obj.m_Notifications;
                writer["tests"] = obj.m_TestNotifications;
            }
        }

        private readonly List<MessageSetting> m_Messages = [];
        private readonly List<NotificationSetting> m_TestNotifications = [];
        private readonly List<NotificationSetting> m_Notifications = [];
        private string m_Token = string.Empty;
        private int m_DelaySinceStart = 0;
        private int m_DelaySinceMessage = 0;

        public IEnumerable<MessageSetting> Messages => m_Messages;
        public IEnumerable<NotificationSetting> Notifications => m_Notifications;
        public IEnumerable<NotificationSetting> TestNotifications => m_TestNotifications;
        public string Token => m_Token;
        public int DelaySinceStart => m_DelaySinceStart;
        public int DelaySinceMessage => m_DelaySinceMessage;

        public void SetToken(string token) => m_Token = token;
        public void SetDelaySinceStart(int delaySinceStart) => m_DelaySinceStart = delaySinceStart;
        public void SetDelaySinceMessage(int delaySinceMessage) => m_DelaySinceMessage = delaySinceMessage;
    }
}
