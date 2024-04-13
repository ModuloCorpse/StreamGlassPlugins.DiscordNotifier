using CorpseLib.Json;
using CorpseLib.Network;
using CorpseLib.Placeholder;
using DiscordCorpse.MessagePart;
using DiscordCorpse.MessagePart.Text;

namespace DiscordNotifierPlugin.MessageSettings
{
    public class LinkMessagePartSetting() : MessagePartSetting("link")
    {
        private string m_Link = string.Empty;
        private string? m_HideText = null;
        private bool m_DisableEmbed = false;

        internal override bool ReadObject(JsonObject obj)
        {
            if (obj.TryGet("text", out string? link) && link != null)
            {
                m_Link = link;
                if (obj.TryGet("hide", out string? hide) && hide != null)
                    m_HideText = hide;
                if (obj.TryGet("embed", out bool? embed))
                    m_DisableEmbed = (embed == false);
                return true;
            }
            return false;
        }

        internal override void FillObject(JsonObject obj)
        {
            obj["text"] = m_Link;
            if (m_HideText != null)
                obj["hide"] = m_HideText;
            if (m_DisableEmbed)
                obj["embed"] = false;
        }

        internal override bool FillText(DiscordText text, Cache cache, IContext[] contexts)
        {
            string link = Converter.Convert(m_Link, cache, contexts);
            URI? linkURI = URI.TryParse(link);
            if (linkURI != null)
            {
                DiscordLink formatedText = new(linkURI);
                if (m_HideText != null)
                    formatedText.Hide(m_HideText);
                if (m_DisableEmbed)
                    formatedText.NoEmbed();
                text.Add(formatedText);
                return true;
            }
            return false;
        }
    }
}
