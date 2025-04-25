namespace GoogleDriveHandler
{
    internal class GoogleDriveCredentials
    {
        public static string TokenUri => "https://oauth2.googleapis.com/token";

        public static string ScopeUri => "https://www.googleapis.com/auth/drive";

        public static string RedirectUri => "http://localhost:5000/";

        public string AuthUrl => buildAuthUri();

        public required string ClientId { get; init; }

        public required string ClientSecret { get; init; }

        public string? RefreshToken { get; set; }

        public string? AccessToken { get; set; }

        private string buildAuthUri()
        {
            return $"https://accounts.google.com/o/oauth2/v2/auth?response_type=code&scope={Uri.EscapeDataString(ScopeUri)}&redirect_uri={Uri.EscapeDataString(RedirectUri)}&client_id={Uri.EscapeDataString(ClientId)}";
        }
    }
}