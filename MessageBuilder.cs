using CorpseLib.Placeholder;
using DiscordCorpse;
using DiscordCorpse.MessagePart;
using DiscordNotifierPlugin.EmbedSettings;
using DiscordNotifierPlugin.MessageSettings;

namespace DiscordNotifierPlugin
{
    public class MessageBuilder(params IContext[] context)
    {
        private readonly Cache m_Cache = new();
        private readonly IContext[] m_Context = context;
        private readonly DateTime m_NowTimestamp = DateTime.UtcNow;

        public DiscordMessage Build(MessageSetting setting)
        {
            DiscordMessage message = [];
            DiscordText discordText = [];
            bool haveText = false;
            foreach (MessagePartSetting part in setting.Parts)
            {
                if (part.FillText(discordText, m_Cache, m_Context))
                    haveText = true;
            }
            if (haveText)
                message.Add(discordText);
            foreach (EmbedSetting embed in setting.Embeds)
                embed.AddEmbed(message, m_NowTimestamp, m_Cache, m_Context);
            return message;
        }
    }
}
