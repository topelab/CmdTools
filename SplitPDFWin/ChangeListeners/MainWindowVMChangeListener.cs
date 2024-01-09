using SplitPDFWin.Models;
using SplitPDFWin.Resources;
using SplitPDFWin.ViewModels;
using System.IO;

namespace SplitPDFWin.ChangeListeners
{
    internal class MainWindowVMChangeListener : BaseVMChangeListener<MainWindowViewModel, INoModel>, IMainWindowVMChangeListener
    {
        protected override bool UpdateModelPropertyWithVMValue(string propertyName, MainWindowViewModel vm, INoModel model)
        {
            switch (propertyName)
            {
                case nameof(vm.PdfInput):
                    vm.PdfOutput = Path.GetDirectoryName(vm.PdfInput);
                    break;
                case nameof(vm.FileOverride):
                    vm.FileOverrideTipInfo = vm.FileOverride ? Strings.FileOverrideOn : Strings.FileOverrideOff;
                    break;
                default:
                    break;
            }
            return false;
        }
    }
}
