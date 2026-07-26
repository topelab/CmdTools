using System.Net;
using System.Net.Sockets;

namespace RelationsShared.Services
{
    internal class EmbededWebServer : IEmbededWebServer
    {
        private HttpListener listener;
        private string localFiles;
        private string content = "<html><body><p>Hello!!</p></body></html>";

        public string BaseUrl { get; private set; }

        public void Start(string localFiles)
        {
            this.localFiles = localFiles;

            int freePort = GetFreePort();
            BaseUrl = $"http://localhost:{freePort}/";
            listener = new HttpListener();
            listener.Prefixes.Add(BaseUrl);
            listener.Start();

            Task.Run(ListenRequests);
        }

        public void SetContent(string content)
        {
            this.content = content;
        }

        private int GetFreePort()
        {
            TcpListener temporal = new TcpListener(IPAddress.Loopback, 0);
            temporal.Start();
            int port = ((IPEndPoint)temporal.LocalEndpoint).Port;
            temporal.Stop();
            return port;
        }

        private async Task ListenRequests()
        {
            while (listener.IsListening)
            {
                try
                {
                    var context = await listener.GetContextAsync();
                    var request = context.Request;
                    var response = context.Response;

                    string urlPath = request.Url.AbsolutePath.TrimStart('/');
                    if (string.IsNullOrEmpty(urlPath))
                    {
                        urlPath = "index.html";
                    }

                    if (urlPath.StartsWith("index", StringComparison.OrdinalIgnoreCase) && urlPath.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
                    {
                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(content);
                        response.ContentLength64 = buffer.Length;
                        response.ContentType = "text/html";
                        await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                    }
                    else
                    {
                        string filePath = Path.Combine(localFiles, urlPath);

                        if (File.Exists(filePath))
                        {
                            byte[] buffer = await File.ReadAllBytesAsync(filePath);
                            response.ContentLength64 = buffer.Length;
                            var extension = Path.GetExtension(filePath).ToLowerInvariant();

                            switch (extension)
                            {
                                case ".js":
                                case ".mjs":
                                    response.ContentType = "text/javascript";
                                    break;
                                case ".html":
                                    response.ContentType = "text/html";
                                    break;
                                case ".css":
                                    response.ContentType = "text/css";
                                    break;
                                default:
                                    response.ContentType = "text/plain";
                                    break;
                            }

                            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                        }
                        else
                        {
                            response.StatusCode = (int)HttpStatusCode.NotFound;
                        }
                    }

                    response.OutputStream.Close();
                }
                catch
                {
                    // Evita caídas al apagar el servidor
                }
            }
        }

        public void Stop() => listener?.Stop();
    }
}