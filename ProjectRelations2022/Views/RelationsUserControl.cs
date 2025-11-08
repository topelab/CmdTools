using Microsoft.VisualStudio.Extensibility.UI;

namespace ProjectRelations2022.Views
{
    /// <summary>
    /// Ventana WPF que muestra un diagrama Mermaid usando WebView2.
    /// </summary>
    public class RelationsUserControl : RemoteUserControl
    {
        public RelationsUserControl(RelationsUserControlContext relationsUserControlContext) : base(relationsUserControlContext, null)
        {
        }
    }
}
