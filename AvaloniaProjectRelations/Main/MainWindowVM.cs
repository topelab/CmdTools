using Topelab.Core.Avalonia.Base;

namespace AvaloniaProjectRelations.Main
{
    public class MainWindowVM : BaseVM<MainWindowVM, INoModel>
    {
        public BaseVM PrincipalVM { get; private set; }

        public MainWindowVM(BaseVM mainVM)
        {
            IsMain = true;
            PrincipalVM = mainVM;
            ContentViewModel = mainVM;
            Title = mainVM.Title;
        }
    }
}