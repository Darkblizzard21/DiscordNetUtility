using Discord;

namespace DNU
{
    public static class GuildChannelExtensions
    {
        public static string GetMentionOrName(this IGuildChannel channel)
        {
            if (channel is ITextChannel textChannel)
            {
                return textChannel.Mention;
            }
            return channel.Name;
        }
    }
}