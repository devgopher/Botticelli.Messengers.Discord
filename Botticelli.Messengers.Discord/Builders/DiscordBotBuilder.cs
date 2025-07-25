using Botticelli.Bot.Data;
using Botticelli.Bot.Data.Repositories;
using Botticelli.Bot.Data.Settings;
using Botticelli.Bot.Utils;
using Botticelli.Bot.Utils.TextUtils;
using Botticelli.Client.Analytics;
using Botticelli.Client.Analytics.Settings;
using Botticelli.Framework.Builders;
using Botticelli.Framework.Extensions;
using Botticelli.Framework.Options;
using Botticelli.Framework.Security;
using Botticelli.Framework.Services;
using Botticelli.Interfaces;
using Botticelli.Messengers.Discord.HostedService;
using Botticelli.Messengers.Discord.Options;
using Botticelli.Messengers.Discord.Utils;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Botticelli.Messengers.Discord.Builders;

public class DiscordBotBuilder : BotBuilder<DiscordBot, DiscordBotBuilder>
{
    private DiscordSocketClient? _client;

    private DiscordBotSettings? BotSettings { get; set; }

    protected override DiscordBot InnerBuild()
    {
        Services!.AddHttpClient<BotStatusService>()
            .AddServerCertificates(BotSettings);
        Services!.AddHostedService<BotStatusService>();
        Services!.AddHttpClient<BotKeepAliveService>()
            .AddServerCertificates(BotSettings);
        Services!.AddHttpClient<GetBroadCastMessagesService<DiscordBot>>()
            .AddServerCertificates(BotSettings);
        Services!.AddHostedService<BotKeepAliveService>();
        Services!.AddHostedService<GetBroadCastMessagesService<IBot<DiscordBot>>>();

        Services!.AddHostedService<DiscordBotHostedService>();
        var botId = BotDataUtils.GetBotId();

        if (botId == null) throw new InvalidDataException($"{nameof(botId)} shouldn't be null!");

        #region Metrics

        var metricsPublisher = new MetricsPublisher(AnalyticsClientSettingsBuilder.Build());
        var metricsProcessor = new MetricsProcessor(metricsPublisher);
        Services!.AddSingleton(metricsPublisher);
        Services!.AddSingleton(metricsProcessor);

        #endregion

        #region Data

        Services!.AddDbContext<BotInfoContext>(o =>
            o.UseSqlite($"Data source={BotDataAccessSettingsBuilder.Build().ConnectionString}"));
        Services!.AddScoped<IBotDataAccess, BotDataAccess>();

        #endregion

        #region TextTransformer

        Services!.AddTransient<ITextTransformer, DiscordTextTransformer>();

        #endregion


        Services.AddBotticelliFramework();

        var sp = Services.BuildServiceProvider();

        return new DiscordBot(_client,
            sp.GetRequiredService<IBotDataAccess>(),
            sp.GetRequiredService<MetricsProcessor>(),
            sp.GetRequiredService<ILogger<DiscordBot>>());
    }

    protected virtual DiscordBotBuilder AddBotSettings<TBotSettings>(BotSettingsBuilder<TBotSettings> settingsBuilder)
        where TBotSettings : BotSettings, new()
    {
        BotSettings = settingsBuilder.Build() as DiscordBotSettings ?? throw new InvalidOperationException();

        return this;
    }

    public DiscordBotBuilder AddClient(DiscordSocketClientBuilder clientBuilder)
    {
        _client = clientBuilder.Build();

        return this;
    }

    public static DiscordBotBuilder Instance(IServiceCollection services,
        ServerSettingsBuilder<ServerSettings> serverSettingsBuilder,
        BotSettingsBuilder<DiscordBotSettings> settingsBuilder,
        DataAccessSettingsBuilder<DataAccessSettings> dataAccessSettingsBuilder,
        AnalyticsClientSettingsBuilder<AnalyticsClientSettings> analyticsClientSettingsBuilder)
    {
        return (DiscordBotBuilder)new DiscordBotBuilder()
            .AddBotSettings(settingsBuilder)
            .AddServerSettings(serverSettingsBuilder)
            .AddServices(services)
            .AddAnalyticsSettings(analyticsClientSettingsBuilder)
            .AddBotDataAccessSettings(dataAccessSettingsBuilder);
    }
}