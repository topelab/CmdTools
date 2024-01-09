using SplitPDFWin.ChangeListeners;
using SplitPDFWin.Resources;
using SplitPDFWin.ViewModels;

namespace SplitPDFWin.Factories
{
    internal class MainWindowFactory(ISelectFileCommandFactory selectFileCommandFactory,
                                     IMainWindowVMChangeListener changeListener,
                                     IStartCommandFactory startCommandFactory,
                                     ISelectOutputPathCommandFactory selectOutputPathCommandFactory) : IMainWindowFactory
    {
        private readonly ISelectFileCommandFactory selectFileCommandFactory = selectFileCommandFactory ?? throw new System.ArgumentNullException(nameof(selectFileCommandFactory));
        private readonly IMainWindowVMChangeListener changeListener = changeListener ?? throw new System.ArgumentNullException(nameof(changeListener));
        private readonly IStartCommandFactory startCommandFactory = startCommandFactory ?? throw new System.ArgumentNullException(nameof(startCommandFactory));
        private readonly ISelectOutputPathCommandFactory selectOutputPathCommandFactory = selectOutputPathCommandFactory ?? throw new System.ArgumentNullException(nameof(selectOutputPathCommandFactory));

        public MainWindowViewModel Create()
        {
            MainWindowViewModel viewModel = new MainWindowViewModel();
            viewModel.SelectFileCommand = selectFileCommandFactory.Create(viewModel);
            viewModel.StartCommand = startCommandFactory.Create(viewModel);
            viewModel.SelectOutputPathCommand = selectOutputPathCommandFactory.Create(viewModel);
            viewModel.FileOverrideTipInfo = Strings.FileOverrideOff;

            changeListener.Start(viewModel);
            return viewModel;
        }
    }
}
