using CorpseLib.Json;
using CorpseLib.Placeholder;
using DiscordCorpse.MessagePart;
using DiscordCorpse.MessagePart.Text;

namespace DiscordNotifierPlugin.MessageSettings
{
    public class TextMessagePartSetting() : MessagePartSetting("text")
    {
        private string m_Text = string.Empty;
        private bool m_Codeblock = false;
        private bool m_Strikethrough = false;
        private bool m_Italic = false;
        private bool m_Bold = false;
        private bool m_Underline = false;
        private bool m_Spoiler = false;

        internal override bool ReadObject(JsonObject obj)
        {
            if (obj.TryGet("text", out string? text) && text != null)
            {
                m_Text = text;
                if (obj.TryGet("codeblock", out bool? codeblock) && codeblock == true)
                    m_Codeblock = (codeblock == true);
                if (obj.TryGet("strikethrough", out bool? strikethrough) && strikethrough == true)
                    m_Strikethrough = (strikethrough == true);
                if (obj.TryGet("italic", out bool? italic) && italic == true)
                    m_Italic = (italic == true);
                if (obj.TryGet("bold", out bool? bold) && bold == true)
                    m_Bold = (bold == true);
                if (obj.TryGet("underline", out bool? underline) && underline == true)
                    m_Underline = (underline == true);
                if (obj.TryGet("spoiler", out bool? spoiler) && spoiler == true)
                    m_Spoiler = (spoiler == true);
                return true;
            }
            return false;
        }

        internal override void FillObject(JsonObject obj)
        {
            obj["text"] = m_Text;
            obj["codeblock"] = m_Codeblock;
            obj["strikethrough"] = m_Strikethrough;
            obj["italic"] = m_Italic;
            obj["bold"] = m_Bold;
            obj["underline"] = m_Underline;
            obj["spoiler"] = m_Spoiler;
        }

        internal override bool FillText(DiscordText text, Cache cache, IContext[] contexts)
        {
            string textPart = Converter.Convert(m_Text, cache, contexts);
            DiscordFormatedText formatedText = new(textPart);
            if (m_Codeblock)
                formatedText.CodeBlock();
            if (m_Strikethrough)
                formatedText.Strikethrough();
            if (m_Italic)
                formatedText.Italic();
            if (m_Bold)
                formatedText.Bold();
            if (m_Underline)
                formatedText.Underline();
            if (m_Spoiler)
                formatedText.Spoiler();
            text.Add(formatedText);
            return true;
        }
    }
}
