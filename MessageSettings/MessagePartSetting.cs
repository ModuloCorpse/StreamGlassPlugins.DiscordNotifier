using CorpseLib;
using CorpseLib.DataNotation;
using CorpseLib.Json;
using CorpseLib.Placeholder;
using DiscordCorpse.MessagePart;

namespace DiscordNotifierPlugin.MessageSettings
{
    public abstract class MessagePartSetting(string type)
    {
        private readonly string m_Type = type;

        public string Type => m_Type;

        internal abstract bool ReadObject(DataObject obj);
        internal abstract void FillObject(DataObject obj);
        internal abstract bool FillText(DiscordText text, Cache cache, IContext[] contexts);
    }

    public class MessagePartSettingDataSerializer : ADataSerializer<MessagePartSetting>
    {
        protected override OperationResult<MessagePartSetting> Deserialize(DataObject reader)
        {
            if (reader.TryGet("type", out string? type) && type != null)
            {
                MessagePartSetting? messagePartSetting = type switch
                {
                    "link" => new LinkMessagePartSetting(),
                    "text" => new TextMessagePartSetting(),
                    _ => null
                };
                if (messagePartSetting == null)
                    return new("Deserialization error", "Invalid message part setting type");
                if (!messagePartSetting.ReadObject(reader))
                    return new("Deserialization error", "Invalid message part setting data");
                return new(messagePartSetting);
            }
            return new("Deserialization error", "Cannot deserialize message part setting");
        }

        protected override void Serialize(MessagePartSetting obj, DataObject writer)
        {
            writer["type"] = obj.Type;
            obj.FillObject(writer);
        }
    }
}
