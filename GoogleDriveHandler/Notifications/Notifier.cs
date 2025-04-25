
namespace GoogleDriveHandler.Notifications
{
    internal class Notifier : INotifier
    {
        public Task Notify(string correlationId, string message, CancellationToken cancellationToken)
        {
            Console.WriteLine(buildMessage(correlationId, message));
            return Task.CompletedTask;
        }

        public async Task NotifyFromHttpResponse(string correlationId,
            HttpResponseMessage responseMessage,
            CancellationToken cancellationToken)
        {
            if (responseMessage.IsSuccessStatusCode)
            {
                await Notify(correlationId,
                    $"Http response message: {responseMessage.StatusCode}",
                    cancellationToken);
                return;
            }

            string errorMessage = await responseMessage.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            await Notify(correlationId, errorMessage, cancellationToken);
        }

        private static string buildMessage(string correlationId, string message)
        {
            return $"{correlationId}: {message}";
        }
    }
}
