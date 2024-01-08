using ReactiveUI;
using SplitPDFWin.Managers;

namespace SplitPDFWin.ViewModels
{
    public class BaseVM : ReactiveObject
    {
        public bool IsUpdated { get; set; }
    }

    internal class BaseVM<T> : BaseVM
        where T : BaseVM
    {
        public IErrorManager<T> ErrorManager { get; set; }

        public bool HasErrors => ErrorManager?.HasErrors ?? false;

        public string ErrorMessages => ErrorManager == null ? null : string.Join("\n", ErrorManager.GetErrors());
    }

    internal class BaseVM<TViewModel, TModel> : BaseVM<TViewModel>
        where TModel : class
        where TViewModel : BaseVM
    {
        protected TModel model;

        public BaseVM()
        {
        }

        public BaseVM(TModel model)
        {
            this.model = model;
        }

        public TModel Model => model;
    }
}