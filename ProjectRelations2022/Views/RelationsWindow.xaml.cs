using System.IO;
using System.Windows;
using ProjectRelations2022.DTO;

namespace ProjectRelations2022.Views
{
    /// <summary>
    /// Ventana WPF que muestra un diagrama Mermaid usando WebView2.
    /// </summary>
    public partial class RelationsWindow : Window
    {
        private readonly UserSettings userSettings;

        public RelationsWindow(string mmdFile, UserSettings userSettings)
        {
            this.userSettings = userSettings;
            InitializeComponent();
            this.Loaded += async (s, e) => await InitializeAsync(mmdFile);
        }

        private void RelationsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async Task InitializeAsync(string mmdFile)
        {
            try
            {
                var envPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, "WebView2");
                System.Environment.SetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER", envPath);
                await webView.EnsureCoreWebView2Async();
                Generate(mmdFile);
            }
            catch
            {
                // ignore initialization errors at design time
            }
        }

        public void Generate(string mmdFile)
        {
            var content = File.Exists(mmdFile) ? File.ReadAllText(mmdFile) : "graph TD\n\tEmpty";
            RenderMermaid(content);
        }

        private void RenderMermaid(string mmd)
        {
            var color = userSettings.HasBackgroundColor ? userSettings.BackgroundColor.ToLower() : "black";
            var encoded = System.Net.WebUtility.HtmlEncode(mmd);
            var html = "<!doctype html>" +
                       "<html>" +
                       "<head>" +
                       "  <meta charset=\"utf-8\">" +
                       """
                       <script type="module">
                         import mermaid from "https://cdn.jsdelivr.net/npm/mermaid@latest/dist/mermaid.esm.min.mjs";
                         import elkLayouts from "https://cdn.jsdelivr.net/npm/@mermaid-js/layout-elk@latest/dist/mermaid-layout-elk.esm.min.mjs";

                         // Registra el motor ELK con Mermaid
                         mermaid.registerLayoutLoaders(elkLayouts);

                         // Inicializa Mermaid
                         mermaid.initialize({
                            startOnLoad: true,
                            flowchart: { defaultRenderer: "elk" }
                            });
                       </script>
                       
                       """ +
                       "  <style>body { margin:10px; padding:0; background-color: " + color + "; }</style>" +
                       "</head>" +
                       "<body>" +
                       "<div class=\"mermaid\">" + encoded + "</div>" +
                       "</body>" +
                       "</html>";

            try
            {
                webView.CoreWebView2.NavigateToString(html);
            }
            catch
            {
                // ignore if not initialized
            }
        }
    }
}
