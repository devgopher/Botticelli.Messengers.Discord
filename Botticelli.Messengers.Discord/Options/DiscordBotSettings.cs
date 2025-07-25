using Botticelli.Framework.Options;

namespace Botticelli.Messengers.Discord.Options;

/// <inheritdoc />
public class DiscordBotSettings : BotSettings
{
    public int PollIntervalMs { get; set; } = 500;
    public static string Section => "DiscordBot";
}