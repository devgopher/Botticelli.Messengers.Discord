using Botticelli.Bot.Data.Repositories;
using Botticelli.Client.Analytics;
using Botticelli.Framework;
using Botticelli.Framework.Events;
using Botticelli.Framework.Exceptions;
using Botticelli.Framework.Global;
using Botticelli.Interfaces;
using Botticelli.Messengers.Discord.Handlers;
using Botticelli.Shared.API;
using Botticelli.Shared.API.Admin.Requests;
using Botticelli.Shared.API.Admin.Responses;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.API.Client.Responses;
using Botticelli.Shared.Constants;
using Microsoft.Extensions.Logging;

namespace Botticelli.Messengers.Discord;

public class DiscordBot : BaseBot<DiscordBot>
{
    private readonly IBotDataAccess _data;
    private readonly IBotUpdateHandler _handler;
    private readonly MessagePublisher? _messagePublisher;
    private bool _eventsAttached;

    public DiscordBot(MessagePublisher? messagePublisher,
                 IBotDataAccess data,
                 IBotUpdateHandler handler,
                 MetricsProcessor metrics,
                 ILogger<DiscordBot> logger) : base(logger, metrics)
    {
        _messagePublisher = messagePublisher;
        _data = data;
        _handler = handler;
        BotUserId = null; // TODO get it from VK
    }

    public override BotType Type => BotType.Vk;


    protected override async Task<StopBotResponse> InnerStopBotAsync(StopBotRequest request, CancellationToken token)
    {
        try
        {
            await _messagesProvider.Stop();

            return StopBotResponse.GetInstance(request.Uid, string.Empty, AdminCommandStatus.Ok);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }

        return StopBotResponse.GetInstance(request.Uid, "Error stopping a bot", AdminCommandStatus.Fail);
    }

    protected override async Task<SendMessageResponse> InnerSendMessageAsync<TSendOptions>(SendMessageRequest request, ISendOptionsBuilder<TSendOptions>? optionsBuilder, bool isUpdate,
        CancellationToken token) =>
        throw new NotImplementedException();

    protected override async Task<RemoveMessageResponse> InnerDeleteMessageAsync(DeleteMessageRequest request, CancellationToken token) => throw new NotImplementedException();

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
                _messagesProvider.OnUpdates += (args, ct) =>
                {
                    var updates = args?.Response?.Updates;

                    if (updates == null || !updates.Any()) return;

                    _handler.HandleUpdateAsync(updates, ct);
                };

                _eventsAttached = true;
            }

            await _messagesProvider.Start(token);

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


    public virtual event MsgSentEventHandler MessageSent;
    public virtual event MsgReceivedEventHandler MessageReceived;
    public virtual event MsgRemovedEventHandler MessageRemoved;
    public virtual event MessengerSpecificEventHandler MessengerSpecificEvent;
}