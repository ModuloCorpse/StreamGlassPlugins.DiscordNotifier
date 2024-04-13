using CorpseLib;
using CorpseLib.Json;
using DiscordNotifierPlugin.EmbedSettings;
using DiscordNotifierPlugin.MessageSettings;

namespace DiscordNotifierPlugin
{
    public class MessageSetting(string id)
    {
        public class JsonSerializer : AJsonSerializer<MessageSetting>
        {
            protected override OperationResult<MessageSetting> Deserialize(JsonObject reader)
            {
                if (reader.TryGet("id", out string? id) && id != null &&
                    reader.TryGet("content", out JsonObject? content) && content != null)
                {
                    MessageSetting setting = new(id);
                    setting.m_Parts.AddRange(content.GetList<MessagePartSetting>("parts"));
                    setting.m_Embeds.AddRange(content.GetList<EmbedSetting>("embeds"));
                    return new(setting);
                }
                return new("Deserialization error", "Cannot deserialize message setting");
            }

            protected override void Serialize(MessageSetting obj, JsonObject writer)
            {
                writer["id"] = obj.m_ID;
                writer["content"] = new JsonObject()
                {
                    { "parts", obj.m_Parts },
                    { "embeds", obj.m_Embeds }
                };
            }
        }

        private readonly List<MessagePartSetting> m_Parts = [];
        private readonly List<EmbedSetting> m_Embeds = [];
        private string m_ID = id;

        public IEnumerable<MessagePartSetting> Parts => m_Parts;
        public IEnumerable<EmbedSetting> Embeds => m_Embeds;
        public string ID => m_ID;
    }
}
