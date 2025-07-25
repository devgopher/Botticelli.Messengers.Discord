using Botticelli.Framework.Options;

namespace Botticelli.Messengers.Discord.Options;

/// <inheritdoc />
public class DiscordBotSettings : BotSettings
{
    public static string Section => "DiscordBot";
    
    /// <summary>
    ///     Gets or sets the WebSocket host to connect to. If <see langword="null" />, the client will use the
    ///     /gateway endpoint.
    /// </summary>
    public string GatewayHost { get; set; } = null;

    /// <summary>
    ///     Gets or sets the time, in milliseconds, to wait for a connection to complete before aborting.
    /// </summary>
    public int ConnectionTimeout { get; set; } = 30000;

    /// <summary>
    ///     Gets or sets the ID for this shard. Must be less than <see cref="TotalShards" />.
    /// </summary>
    public int? ShardId { get; set; } = null;

    /// <summary>
    ///     Gets or sets the total number of shards for this application.
    /// </summary>
    public int? TotalShards { get; set; } = null;

    /// <summary>
    ///     Gets or sets whether the client should download the default stickers on startup.
    /// </summary>
    public bool AlwaysDownloadDefaultStickers { get; set; } = false;

    /// <summary>
    ///     Gets or sets whether or not the client should automatically resolve the stickers sent on a message.
    /// </summary>
    public bool AlwaysResolveStickers { get; set; } = false;

    /// <summary>
    ///     Gets or sets the number of messages per channel that should be kept in cache. Setting this to zero
    ///     disables the message cache entirely.
    /// </summary>
    public int MessageCacheSize { get; set; } = 0;

    /// <summary>
    ///     Gets or sets the number of audit logs per guild that should be kept in cache. Setting this to zero
    ///     disables the audit log cache entirely.
    /// </summary>
    public int AuditLogCacheSize { get; set; } = 0;

    /// <summary>
    ///     Gets or sets the max number of users a guild may have for offline users to be included in the READY
    ///     packet. The maximum value allowed is 250.
    /// </summary>
    public int LargeThreshold { get; set; } = 250;


    /// <summary>
    ///     Gets or sets whether all users should be downloaded as guilds come available.
    /// </summary>
    /// </remarks>
    public bool AlwaysDownloadUsers { get; set; } = false;

    /// <summary>
    ///     Gets or sets the timeout for event handlers, in milliseconds, after which a warning will be logged.
    ///     Setting this property to <see langword="null" />disables this check.
    /// </summary>
    public int? HandlerTimeout { get; set; } = 3000;

    /// <summary>
    ///     Gets or sets the maximum identify concurrency.
    /// </summary>
    public int IdentifyMaxConcurrency { get; set; } = 1;

    /// <summary>
    ///     Gets or sets whether to include the raw payload on gateway errors.
    /// </summary>
    public bool IncludeRawPayloadOnGatewayErrors { get; set; } = false;

    /// <summary>
    ///     Gets or sets whether to log warnings related to guild intents and events.
    /// </summary>
    public bool LogGatewayIntentWarnings { get; set; } = true;

    /// <summary>
    ///     Gets or sets whether Unknown Dispatch event messages should be logged.
    /// </summary>
    public bool SuppressUnknownDispatchWarnings { get; set; } = true;
}