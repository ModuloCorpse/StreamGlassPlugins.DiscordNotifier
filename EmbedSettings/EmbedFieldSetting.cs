using CorpseLib;
using CorpseLib.DataNotation;
using CorpseLib.Placeholder;
using DiscordCorpse;

namespace DiscordNotifierPlugin.EmbedSettings
{
    public class EmbedFieldSetting(string name, string value, bool? inline)
    {
        public class DataSerializer : ADataSerializer<EmbedFieldSetting>
        {
            protected override OperationResult<EmbedFieldSetting> Deserialize(DataObject reader)
            {
                if (reader.TryGet("name", out string? name) && name != null &&
                    reader.TryGet("value", out string? value) && value != null)
                {
                    reader.TryGet("inline", out bool? inline);
                    return new(new(name, value, inline));
                }
                return new("Deserialization error", "Cannot deserialize embed field setting");
            }

            protected override void Serialize(EmbedFieldSetting obj, DataObject writer)
            {
                writer["name"] = obj.m_Name;
                writer["value"] = obj.m_Value;
                if (obj.m_Inline != null)
                    writer["inline"] = obj.m_Inline;
            }
        }

        private string m_Name = name;
        private string m_Value = value;
        private bool? m_Inline = inline;

        public void FillEmbed(DiscordEmbed embed, Cache cache, IContext[] contexts)
        {
            embed.AddField(new(Converter.Convert(m_Name, cache, contexts),
                Converter.Convert(m_Value, cache, contexts), m_Inline));
        }
    }
}
