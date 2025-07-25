using Botticelli.Messengers.Discord.Options;
using Discord.WebSocket;

/// <summary>
///     A builder for configuring and creating instances of <see cref="DiscordSocketClient"/>.
/// </summary>
public class DiscordSocketClientBuilder
{
    private readonly DiscordSocketConfig _config = new();

    /// <summary>
    ///     Sets the total number of shards for the client.
    /// </summary>
    /// <param name="totalShards">The total number of shards.</param>
    /// <returns>The current instance of the builder.</returns>
    public DiscordSocketClientBuilder WithTotalShards(int totalShards)
    {
        _config.TotalShards = totalShards;
        return this;
    }

    /// <summary>
    ///     Sets the size of the message cache.
    /// </summary>
    /// <param name="size">The size of the message cache.</param>
    /// <returns>The current instance of the builder.</returns>
    public DiscordSocketClientBuilder WithMessageCacheSize(int size)
    {
        _config.MessageCacheSize = size;
        return this;
    }

    /// <summary>
    ///     Sets the large threshold for guilds.
    /// </summary>
    /// <param name="threshold">The large threshold value.</param>
    /// <returns>The current instance of the builder.</returns>
    public DiscordSocketClientBuilder WithLargeThreshold(int threshold)
    {
        _config.LargeThreshold = threshold;
        return this;
    }

    /// <summary>
    ///     Specifies whether to always download user data.
    /// </summary>
    /// <param name="value">True to always download users; otherwise, false.</param>
    /// <returns>The current instance of the builder.</returns>
    public DiscordSocketClientBuilder WithAlwaysDownloadUsers(bool value)
    {
        _config.AlwaysDownloadUsers = value;
        return this;
    }

    /// <summary>
    ///     Sets the handler timeout in milliseconds.
    /// </summary>
    /// <param name="timeout">The timeout value in milliseconds, or null for no timeout.</param>
    /// <returns>The current instance of the builder.</returns>
    public DiscordSocketClientBuilder WithHandlerTimeout(int? timeout)
    {
        _config.HandlerTimeout = timeout;
        return this;
    }

    /// <summary>
    ///     Specifies whether to log gateway intent warnings.
    /// </summary>
    /// <param name="value">True to log warnings; otherwise, false.</param>
    /// <returns>The current instance of the builder.</returns>
    public DiscordSocketClientBuilder WithLogGatewayIntentWarnings(bool value)
    {
        _config.LogGatewayIntentWarnings = value;
        return this;
    }

    /// <summary>
    ///     Specifies whether to suppress unknown dispatch warnings.
    /// </summary>
    /// <param name="value">True to suppress warnings; otherwise, false.</param>
    /// <returns>The current instance of the builder.</returns>
    public DiscordSocketClientBuilder WithSuppressUnknownDispatchWarnings(bool value)
    {
        _config.SuppressUnknownDispatchWarnings = value;
        return this;
    }
    
    /// <summary>
    ///     Specifies whether to suppress unknown dispatch warnings.
    /// </summary>
    /// <param name="value">True to suppress warnings; otherwise, false.</param>
    /// <returns>The current instance of the builder.</returns>
    public DiscordSocketClientBuilder With(Action<DiscordSocketConfig> action)
    {
        action(_config);
        
        return this;
    }


    /// <summary>
    ///     Builds and returns a new instance of <see cref="DiscordSocketClient"/> with the configured settings.
    /// </summary>
    /// <returns>A new instance of <see cref="DiscordSocketClient"/>.</returns>
    public DiscordSocketClient? Build()
    {
        return new DiscordSocketClient(_config);
    }
}
