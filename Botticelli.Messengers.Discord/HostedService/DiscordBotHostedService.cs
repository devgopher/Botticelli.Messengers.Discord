using Botticelli.Interfaces;
using Botticelli.Shared.API.Admin.Requests;
using Microsoft.Extensions.Hosting;

namespace Botticelli.Messengers.Discord.HostedService;

public class DiscordBotHostedService : IHostedService
{
    private readonly IBot<DiscordBot> _bot;

    public DiscordBotHostedService(IBot<DiscordBot> bot)
    {
        _bot = bot;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _bot.StartBotAsync(StartBotRequest.GetInstance(), CancellationToken.None);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _bot.StopBotAsync(StopBotRequest.GetInstance(), CancellationToken.None);
    }
}