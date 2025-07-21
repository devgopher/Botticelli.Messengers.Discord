using Botticelli.Framework.Events;

namespace Botticelli.Messengers.Discord.Handlers;

public interface IBotUpdateHandler
{
    public delegate void MsgReceivedEventHandler(object sender, MessageReceivedBotEventArgs e);

    public Task HandleUpdateAsync(List<UpdateEvent> update, CancellationToken cancellationToken);

    public event MsgReceivedEventHandler MessageReceived;
}