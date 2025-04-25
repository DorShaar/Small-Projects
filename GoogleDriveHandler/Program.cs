// See https://aka.ms/new-console-template for more information
using GoogleDriveHandler;
using GoogleDriveHandler.Notifications;

Console.WriteLine("Hello, Google Drive World!");

// tODO DOR change to google drive handler

GoogleDriveCredentials googleDriveCredentials = new()
{
    ClientId = "",
    ClientSecret = "",
    RefreshToken = "",
    AccessToken = ""
};

Notifier notifier = new();
GoogleDriveCredentialsHandler googleDriveCredentialsHandler = new(googleDriveCredentials, notifier);

Reader googleDriveReader = new(googleDriveCredentialsHandler, notifier);

Uploader googleDriveUploader = new(googleDriveCredentialsHandler, googleDriveReader, notifier);

await googleDriveUploader.Upload(@"some_path",
                      "records",
    CancellationToken.None);