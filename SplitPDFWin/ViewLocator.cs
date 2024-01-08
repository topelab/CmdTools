using Avalonia.Controls;
using Avalonia.Controls.Templates;
using SplitPDFWin.ViewModels;

namespace SplitPDFWin
{
    public class ViewLocator : IDataTemplate
    {
        public Control Build(object data)
        {
            var control = Resolve<Control>(data.GetType().Name);
            if (control != null)
            {
                return control;
            }
            else
            {
                return new TextBlock { Text = "Not Found: " + data.GetType().FullName };
            }
        }

        public bool Match(object data)
        {
            return data is BaseVM;
        }
    }
}
