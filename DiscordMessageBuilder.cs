using CorpseLib.Json;
using CorpseLib.Network;
using CorpseLib.Placeholder;
using DiscordCorpse;
using DiscordCorpse.MessagePart;
using DiscordCorpse.MessagePart.Text;
using System.Diagnostics.CodeAnalysis;

namespace DiscordNotifierPlugin
{
    public class DiscordMessageBuilder(params IContext[] context)
    {
        private readonly Cache m_Cache = new();
        private readonly IContext[] m_Context = context;
        private readonly DateTime m_NowTimestamp = DateTime.UtcNow;

        private string? GetStr(string key, JsonObject obj)
        {
            if (obj.TryGet(key, out string? str) && str != null)
                return Converter.Convert(str, m_Cache, m_Context);
            return null;
        }

        private bool TryGetStr(string key, JsonObject obj, [NotNullWhen(true)] out string? str)
        {
            str = GetStr(key, obj);
            return str != null;
        }

        private void FillParts(JsonObject obj, DiscordMessage message)
        {
            List<JsonObject> parts = obj.GetList<JsonObject>("parts");
            DiscordText discordText = [];
            bool haveText = false;
            foreach (JsonObject part in parts)
            {
                if (part.TryGet("type", out string? type) && type != null)
                {
                    string? text = GetStr("text", part);
                    switch (type)
                    {
                        case "text":
                        {
                            if (text != null)
                            {
                                DiscordFormatedText formatedText = new(text);
                                if (part.TryGet("codeblock", out bool? codeblock) && codeblock == true)
                                    formatedText.CodeBlock();
                                if (part.TryGet("strikethrough", out bool? strikethrough) && strikethrough == true)
                                    formatedText.Strikethrough();
                                if (part.TryGet("italic", out bool? italic) && italic == true)
                                    formatedText.Italic();
                                if (part.TryGet("bold", out bool? bold) && bold == true)
                                    formatedText.Bold();
                                if (part.TryGet("underline", out bool? underline) && underline == true)
                                    formatedText.Underline();
                                if (part.TryGet("spoiler", out bool? spoiler) && spoiler == true)
                                    formatedText.Spoiler();
                                discordText.Add(formatedText);
                                haveText = true;
                            }
                            break;
                        }
                        case "link":
                        {
                            if (text != null)
                            {
                                DiscordLink formatedText = new(URI.Parse(text));
                                if (part.TryGet("hide", out string? hide) && hide != null)
                                    formatedText.Hide(hide);
                                if (part.TryGet("embed", out bool? embed) && embed == false)
                                    formatedText.NoEmbed();
                                discordText.Add(formatedText);
                                haveText = true;
                            }
                            break;
                        }
                    }
                }
            }
            if (haveText)
                message.Add(discordText);
        }

        private void FillEmbed(JsonObject obj, DiscordMessage message)
        {
            List<JsonObject> embeds = obj.GetList<JsonObject>("embeds");
            foreach (JsonObject embed in embeds)
            {
                DiscordEmbed discordEmbed = new();
                if (embed.TryGet("color", out int? color) && color != null)
                    discordEmbed.SetColor((int)color);
                if (TryGetStr("title", embed, out string? title))
                    discordEmbed.SetTitle(title);
                if (TryGetStr("url", embed, out string? url))
                    discordEmbed.SetURL(URI.Parse(url));
                if (TryGetStr("thumbnail", embed, out string? thumbnailURL))
                    discordEmbed.SetThumbnail(new(URI.Parse(thumbnailURL), null, null, null));
                if (TryGetStr("timestamp", embed, out string? timestamp))
                {
                    if (timestamp == "now")
                        discordEmbed.SetTimestamp(m_NowTimestamp);
                    else
                        discordEmbed.SetTimestamp(DateTime.Parse(timestamp));
                }
                if (embed.TryGet("author", out JsonObject? author) && author != null &&
                    TryGetStr("name", author, out string? authorName))
                {
                    URI? authorURL = URI.NullParse(GetStr("url", author));
                    URI? authorIcon = URI.NullParse(GetStr("icon", author));
                    discordEmbed.SetAuthor(new(authorName, authorURL, authorIcon, null));
                }
                List<JsonObject> fields = embed.GetList<JsonObject>("fields");
                if (fields.Count < 25)
                {
                    foreach (JsonObject field in fields)
                    {
                        if (TryGetStr("name", field, out string? fieldName) &&
                            TryGetStr("value", field, out string? fieldValue))
                        {
                            bool? fieldInline = field.GetOrDefault<bool?>("inline", null);
                            discordEmbed.AddField(new(fieldName, fieldValue, fieldInline));
                        }
                    }
                }
                message.Add(discordEmbed);
            }
        }

        public DiscordMessage Build(JsonObject obj)
        {
            DiscordMessage message = [];
            FillParts(obj, message);
            FillEmbed(obj, message);
            return message;
        }
    }
}
