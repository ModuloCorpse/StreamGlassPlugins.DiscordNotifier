using CorpseLib.Json;
using CorpseLib;
using CorpseLib.Network;
using CorpseLib.Placeholder;
using DiscordCorpse;

namespace DiscordNotifierPlugin.EmbedSettings
{
    public class EmbedSetting
    {
        public class JsonSerializer : AJsonSerializer<EmbedSetting>
        {
            protected override OperationResult<EmbedSetting> Deserialize(JsonObject reader)
            {
                reader.TryGet("color", out int? color);
                reader.TryGet("title", out string? title);
                reader.TryGet("url", out string? url);
                reader.TryGet("thumbnail", out string? thumbnail);
                reader.TryGet("timestamp", out string? timestamp);
                reader.TryGet("author", out EmbedAuthorSetting? author);
                List<EmbedFieldSetting> fields = reader.GetList<EmbedFieldSetting>("fields");
                EmbedSetting setting = new()
                {
                    m_Fields = fields,
                    m_Author = author,
                    m_Title = title,
                    m_URL = url,
                    m_Thumbnail = thumbnail,
                    m_Timestamp = timestamp,
                    m_Color = color
                };
                return new(setting);
            }

            protected override void Serialize(EmbedSetting obj, JsonObject writer)
            {
                if (obj.m_Color != null)
                    writer["color"] = obj.m_Color;
                if (obj.m_Title != null)
                    writer["title"] = obj.m_Title;
                if (obj.m_URL != null)
                    writer["url"] = obj.m_URL;
                if (obj.m_Thumbnail != null)
                    writer["thumbnail"] = obj.m_Thumbnail;
                if (obj.m_Timestamp != null)
                    writer["timestamp"] = obj.m_Timestamp;
                if (obj.m_Author != null)
                    writer["author"] = obj.m_Author;
                if (obj.m_Fields.Count > 0)
                    writer["fields"] = obj.m_Fields;
            }
        }

        private List<EmbedFieldSetting> m_Fields = [];
        private EmbedAuthorSetting? m_Author = null;
        private string? m_Title = null;
        private string? m_URL = null;
        private string? m_Thumbnail = null;
        private string? m_Timestamp = null;
        private int? m_Color = null;

        public EmbedFieldSetting[] Fields => [..m_Fields];
        public EmbedAuthorSetting? Author => m_Author;
        public string? Title => m_Title;
        public string? URL => m_URL;
        public string? Thumbnail => m_Thumbnail;
        public string? Timestamp => m_Timestamp;
        public int? Color => m_Color;


        public void AddEmbed(DiscordMessage message, DateTime now, Cache cache, IContext[] contexts)
        {
            DiscordEmbed discordEmbed = new();
            if (m_Color != null)
                discordEmbed.SetColor((int)m_Color);
            if (m_Title != null)
                discordEmbed.SetTitle(Converter.Convert(m_Title, cache, contexts));
            if (m_URL != null)
            {
                URI? url = URI.TryParse(Converter.Convert(m_URL, cache, contexts));
                if (url != null)
                    discordEmbed.SetURL(url);
            }
            if (m_Thumbnail != null)
            {
                URI? thumbnail = URI.TryParse(Converter.Convert(m_Thumbnail, cache, contexts));
                if (thumbnail != null)
                    discordEmbed.SetThumbnail(new(thumbnail, null, null, null));
            }
            if (m_Timestamp != null)
            {
                if (m_Timestamp == "now")
                    discordEmbed.SetTimestamp(now);
                else
                    discordEmbed.SetTimestamp(DateTime.Parse(m_Timestamp));
            }
            m_Author?.FillEmbed(discordEmbed, cache, contexts);
            foreach (EmbedFieldSetting field in m_Fields)
                field.FillEmbed(discordEmbed, cache, contexts);
            message.Add(discordEmbed);
        }
    }
}
