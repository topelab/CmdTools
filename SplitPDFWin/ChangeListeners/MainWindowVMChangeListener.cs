using SplitPDFWin.Models;
using SplitPDFWin.ViewModels;
using System;
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
                default:
                    break;
            }
            return false;
        }
    }
}
