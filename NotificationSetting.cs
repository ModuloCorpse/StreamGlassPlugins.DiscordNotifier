using CorpseLib;
using CorpseLib.DataNotation;

namespace DiscordNotifierPlugin
{
    public class NotificationSetting(string content, bool crosspost)
    {
        public class DataSerializer : ADataSerializer<NotificationSetting>
        {
            protected override OperationResult<NotificationSetting> Deserialize(DataObject reader)
            {
                if (reader.TryGet("content", out string? content) && content != null &&
                    reader.TryGet("crosspost", out bool? crosspost) && crosspost != null)
                {
                    NotificationSetting setting = new(content, crosspost == true);
                    setting.m_Channels.AddRange(reader.GetList<string>("channels"));
                    return new(setting);
                }
                return new("Deserialization error", "Cannot deserialize notification setting");
            }

            protected override void Serialize(NotificationSetting obj, DataObject writer)
            {
                writer["channels"] = obj.m_Channels;
                writer["content"] = obj.m_Content;
                writer["crosspost"] = obj.m_Crosspost;
            }
        }

        private readonly List<string> m_Channels = [];
        private string m_Content = content;
        private bool m_Crosspost = crosspost;

        public IEnumerable<string> Channels => m_Channels;
        public string Content => m_Content;
        public bool Crosspost => m_Crosspost;
    }
}
