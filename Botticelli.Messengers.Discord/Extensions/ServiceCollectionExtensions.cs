using System.Configuration;
using Botticelli.Bot.Data.Settings;
using Botticelli.Client.Analytics.Settings;
using Botticelli.Controls.Parsers;
using Botticelli.Framework.Options;
using Botticelli.Interfaces;
using Botticelli.Messengers.Discord.Builders;
using Botticelli.Messengers.Discord.Layout;
using Botticelli.Messengers.Discord.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Botticelli.Messengers.Discord.Extensions;

public static class ServiceCollectionExtensions
{
    private static readonly BotSettingsBuilder<DiscordBotSettings> SettingsBuilder = new();
    private static readonly ServerSettingsBuilder<ServerSettings> ServerSettingsBuilder = new();

    private static readonly AnalyticsClientSettingsBuilder<AnalyticsClientSettings> AnalyticsClientOptionsBuilder =
            new();

    private static readonly DataAccessSettingsBuilder<DataAccessSettings> DataAccessSettingsBuilder = new();

    public static IServiceCollection AddVkBot(this IServiceCollection services, IConfiguration configuration)
    {
        var vkBotSettings = configuration
                            .GetSection(DiscordBotSettings.Section)
                            .Get<DiscordBotSettings>() ??
                            throw new ConfigurationErrorsException($"Can't load configuration for {nameof(DiscordBotSettings)}!");

        var analyticsClientSettings = configuration
                                      .GetSection(AnalyticsClientSettings.Section)
                                      .Get<AnalyticsClientSettings>() ??
                                      throw new ConfigurationErrorsException($"Can't load configuration for {nameof(AnalyticsClientSettings)}!");

        var serverSettings = configuration
                             .GetSection(ServerSettings.Section)
                             .Get<ServerSettings>() ??
                             throw new ConfigurationErrorsException($"Can't load configuration for {nameof(ServerSettings)}!");

        var dataAccessSettings = configuration
                                 .GetSection(DataAccessSettings.Section)
                                 .Get<DataAccessSettings>() ??
                                 throw new ConfigurationErrorsException($"Can't load configuration for {nameof(DataAccessSettings)}!");
        ;

        return services.AddVkBot(vkBotSettings,
                                 analyticsClientSettings,
                                 serverSettings,
                                 dataAccessSettings);
    }

    public static IServiceCollection AddVkBot(this IServiceCollection services,
                                              DiscordBotSettings botSettings,
                                              AnalyticsClientSettings analyticsClientSettings,
                                              ServerSettings serverSettings,
                                              DataAccessSettings dataAccessSettings)
    {
        return services.AddVkBot(o => o.Set(botSettings),
                                 o => o.Set(analyticsClientSettings),
                                 o => o.Set(serverSettings),
                                 o => o.Set(dataAccessSettings));
    }

    /// <summary>
    ///     Adds a Vk bot
    /// </summary>
    /// <param name="services"></param>
    /// <param name="optionsBuilderFunc"></param>
    /// <param name="analyticsOptionsBuilderFunc"></param>
    /// <param name="serverSettingsBuilderFunc"></param>
    /// <param name="dataAccessSettingsBuilderFunc"></param>
    /// <returns></returns>
    public static IServiceCollection AddVkBot(this IServiceCollection services,
                                              Action<BotSettingsBuilder<DiscordBotSettings>> optionsBuilderFunc,
                                              Action<AnalyticsClientSettingsBuilder<AnalyticsClientSettings>> analyticsOptionsBuilderFunc,
                                              Action<ServerSettingsBuilder<ServerSettings>> serverSettingsBuilderFunc,
                                              Action<DataAccessSettingsBuilder<DataAccessSettings>> dataAccessSettingsBuilderFunc)
    {
        optionsBuilderFunc(SettingsBuilder);
        serverSettingsBuilderFunc(ServerSettingsBuilder);
        analyticsOptionsBuilderFunc(AnalyticsClientOptionsBuilder);
        dataAccessSettingsBuilderFunc(DataAccessSettingsBuilder);

        var clientBuilder = LongPollMessagesProviderBuilder.Instance(SettingsBuilder);

        var botBuilder = DiscordBotBuilder.Instance(services,
                                               ServerSettingsBuilder,
                                               SettingsBuilder,
                                               DataAccessSettingsBuilder,
                                               AnalyticsClientOptionsBuilder)
                                     .AddClient(clientBuilder);
        var bot = botBuilder.Build();

        return services.AddSingleton<IBot<DiscordBot>>(bot)
                       .AddSingleton<IBot>(bot);
    }

    // public static IServiceCollection AddDiscordLayoutsSupport(this IServiceCollection services)
    // {
    //     return services.AddSingleton<ILayoutParser, JsonLayoutParser>()
    //                    .AddSingleton<ILayoutSupplier<DiscordKeyboardMarkup>, DiscordLayoutSupplier>()
    //                    .AddSingleton<ILayoutLoader<DicsordKeyboardMarkup>,
    //                            LayoutLoader<ILayoutParser, ILayoutSupplier<DiscordKeyboardMarkup>, DiscordKeyboardMarkup>>();
    // }
}