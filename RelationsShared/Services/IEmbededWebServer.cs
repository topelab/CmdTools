namespace RelationsShared.Services
{
    public interface IEmbededWebServer
    {
        string BaseUrl { get; }

        void SetContent(string content);
        void Start(string localFiles);
        void Stop();
    }
}