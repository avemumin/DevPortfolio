namespace Client.Static
{
    internal static class APIEndpoints
    {
#if DEBUG
        internal const string ServerBaseUrl = "https://localhost:5003";
#else
        internal const string ServerBaseUrl = "https://appname.azurewebsies.net";
#endif

        internal readonly static string s_categories = $"{ServerBaseUrl}/api/categories";
        internal readonly static string s_imageUpload = $"{ServerBaseUrl}/api/imageUpload";
    }
}
