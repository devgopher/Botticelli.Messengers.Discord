using Botticelli.Bot.Data.Repositories;
using Botticelli.Client.Analytics;
using Botticelli.Framework;
using Botticelli.Framework.Global;
using Botticelli.Interfaces;
using Botticelli.Messengers.Discord.Handlers;
using Botticelli.Shared.API;
using Botticelli.Shared.API.Admin.Requests;
using Botticelli.Shared.API.Admin.Responses;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.API.Client.Responses;
using Botticelli.Shared.Constants;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace Botticelli.Messengers.Discord;

public class DiscordBot : BaseBot<DiscordBot>
{
    private readonly DiscordSocketClient? _client;
    private readonly IBotDataAccess _data;
    private bool _eventsAttached;

    public DiscordBot(DiscordSocketClient? client,
        IBotDataAccess data,
        MetricsProcessor metrics,
        ILogger<DiscordBot> logger) : base(logger, metrics)
    {
        _data = data;
        _client = client;
    }

    public override BotType Type => BotType.Vk;


    protected override async Task<StopBotResponse> InnerStopBotAsync(StopBotRequest request, CancellationToken token)
    {
        try
        {
            await _client.StopAsync();
            // var context = new SocketCommandContext(_client, msg);
            //
            // context.Channel.SendMessageAsync(new )
            return StopBotResponse.GetInstance(request.Uid, string.Empty, AdminCommandStatus.Ok);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }

        return StopBotResponse.GetInstance(request.Uid, "Error stopping a bot", AdminCommandStatus.Fail);
    }

    protected override async Task<SendMessageResponse> InnerSendMessageAsync<TSendOptions>(SendMessageRequest request,
        ISendOptionsBuilder<TSendOptions>? optionsBuilder, bool isUpdate,
        CancellationToken token)
    {
        throw new NotImplementedException();
    }

    protected override async Task<RemoveMessageResponse> InnerDeleteMessageAsync(DeleteMessageRequest request,
        CancellationToken token)
    {
        throw new NotImplementedException();
    }

    protected override async Task<StartBotResponse> InnerStartBotAsync(StartBotRequest request, CancellationToken token)
    {
        try
        {
            Logger.LogInformation($"{nameof(StartBotAsync)}...");
            var response = StartBotResponse.GetInstance(AdminCommandStatus.Ok, "");

            if (BotStatusKeeper.IsStarted)
            {
                Logger.LogInformation($"{nameof(StartBotAsync)}: already started");

                return response;
            }

            if (response.Status != AdminCommandStatus.Ok || BotStatusKeeper.IsStarted) return response;

            if (!_eventsAttached)
            {
                _eventsAttached = true;
            }

            await _client.StartAsync();

            BotStatusKeeper.IsStarted = true;
            Logger.LogInformation($"{nameof(StartBotAsync)}: started");

            return response;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }

        return StartBotResponse.GetInstance(AdminCommandStatus.Fail, "error");
    }

    public override Task SetBotContext(BotData.Entities.Bot.BotData? context, CancellationToken token)
    {
        return Task.CompletedTask;
    }


    public event MsgSentEventHandler MessageSent;
    public event MsgReceivedEventHandler MessageReceived;
    public event MsgRemovedEventHandler MessageRemoved;
    public event MessengerSpecificEventHandler MessengerSpecificEvent;
}