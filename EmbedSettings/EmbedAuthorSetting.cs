using CorpseLib.Json;
using CorpseLib;
using CorpseLib.Network;
using CorpseLib.Placeholder;
using DiscordCorpse;

namespace DiscordNotifierPlugin.EmbedSettings
{
    public class EmbedAuthorSetting(string name, string? url, string? icon)
    {
        public class JsonSerializer : AJsonSerializer<EmbedAuthorSetting>
        {
            protected override OperationResult<EmbedAuthorSetting> Deserialize(JsonObject reader)
            {
                if (reader.TryGet("name", out string? name) && name != null)
                {
                    reader.TryGet("url", out string? url);
                    reader.TryGet("icon", out string? icon);
                    return new(new(name, url, icon));
                }
                return new("Deserialization error", "Cannot deserialize embed author setting");
            }

            protected override void Serialize(EmbedAuthorSetting obj, JsonObject writer)
            {
                writer["name"] = obj.m_Name;
                if (obj.m_URL != null)
                    writer["url"] = obj.m_URL;
                if (obj.m_Icon != null)
                    writer["icon"] = obj.m_Icon;
            }
        }

        private string m_Name = name;
        private string? m_URL = url;
        private string? m_Icon = icon;

        public void FillEmbed(DiscordEmbed embed, Cache cache, IContext[] contexts)
        {
            string name = Converter.Convert(m_Name, cache, contexts);
            URI? url = null;
            if (m_URL != null)
                url = URI.TryParse(Converter.Convert(m_URL, cache, contexts));
            URI? icon = null;
            if (m_Icon != null)
                icon = URI.TryParse(Converter.Convert(m_Icon, cache, contexts));
            embed.SetAuthor(new(name, url, icon, null));
        }
    }
}
