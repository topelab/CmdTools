using System.IO;
using System.Windows;
using Microsoft.VisualStudio.Extensibility.UI;
using ProjectRelations2022.DTO;

namespace ProjectRelations2022.Views
{
    /// <summary>
    /// Ventana WPF que muestra un diagrama Mermaid usando WebView2.
    /// </summary>
    public class RelationsWindow : RemoteUserControl
    {

        private readonly UserSettings userSettings;


        public RelationsWindow(RelationsWindowsContext relationsWindowsContext) : base(relationsWindowsContext, null)
        {
            this.userSettings = relationsWindowsContext.UserSettings;
            var envPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, "WebView2");
            System.Environment.SetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER", envPath);
            Generate(relationsWindowsContext, relationsWindowsContext.MermaidFile);
        }

        private void RelationsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Generate(RelationsWindowsContext relationsWindowsContext, string mmdFile)
        {
            var content = File.Exists(mmdFile) ? File.ReadAllText(mmdFile) : "graph TD\n\tEmpty";
            var html = RenderMermaid(content);
            var fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, "result.html");
            File.WriteAllText(fileName, html);
            relationsWindowsContext.Url = new Uri(fileName).AbsoluteUri;
        }

        private string RenderMermaid(string mmd)
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

            return html;
        }
    }
}
