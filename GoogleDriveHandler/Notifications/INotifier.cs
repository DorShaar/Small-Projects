
namespace GoogleDriveHandler.Notifications
{
    internal interface INotifier
    {
        Task Notify(string correlationId, string message, CancellationToken cancellationToken);

        Task NotifyFromHttpResponse(string correlationId,
            HttpResponseMessage message,
            CancellationToken cancellationToken);
    }
}
