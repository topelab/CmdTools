using ReactiveUI;
using SplitPDFWin.Models;
using System.Windows.Input;

namespace SplitPDFWin.ViewModels
{
    internal class MainWindowViewModel : BaseVM<MainWindowViewModel, INoModel>
    {
        private string pdfInput;

        public string PdfInput
        {
            get { return pdfInput; }
            set { this.RaiseAndSetIfChanged(ref pdfInput, value); }
        }

        private string pdfOutput;

        public string PdfOutput
        {
            get { return pdfOutput; }
            set { this.RaiseAndSetIfChanged(ref pdfOutput, value); }
        }

        private bool fileOverride;

        public bool FileOverride
        {
            get { return fileOverride; }
            set { this.RaiseAndSetIfChanged(ref fileOverride, value); }
        }

        private string fileOverrideTipInfo;

        public string FileOverrideTipInfo
        {
            get { return fileOverrideTipInfo; }
            set { this.RaiseAndSetIfChanged(ref fileOverrideTipInfo, value); }
        }

        public ICommand SelectFileCommand { get; set; }
        public ICommand SelectOutputPathCommand { get; set; }
        public ICommand StartCommand { get; set; }
    }
}
