namespace ProjectRelations
{
    using Microsoft.Web.WebView2.WinForms;
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    /// <summary>
    /// Control WinForms que aloja WebView2 para renderizar Mermaid.
    /// </summary>
    public partial class RelationsToolWindow : Form
    {
        private WebView2 webView;
        private readonly UserSettings userSettings;


        /// <summary>
        /// Crea una nueva instancia del control de la ToolWindow.
        /// </summary>
        public RelationsToolWindow(string mmdFile, UserSettings userSettings)
        {
            this.userSettings = userSettings;
            InitializeComponent();
            _ = InitializeAsync(mmdFile);
        }

        private async Task InitializeAsync(string mmdFile)
        {
            webView = new WebView2 { Dock = DockStyle.Fill };

            Controls.Add(webView);

            try
            {
                string webView2UserDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, "WebView2");
                Environment.SetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER", webView2UserDataPath);
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
            try
            {
                var content = File.Exists(mmdFile) ? File.ReadAllText(mmdFile) : "graph TD\n\tEmpty";

                Invoke(() => RenderMermaid(content));
            }
            catch (Exception ex)
            {
                Invoke(() => MessageBox.Show($"Error: {ex.Message}"));
            }
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

        private void InitializeComponent()
        {
            SuspendLayout();
            Name = "RelationsToolWindow";
            Size = new System.Drawing.Size(800, 600);
            ResumeLayout(false);
        }
    }
}
