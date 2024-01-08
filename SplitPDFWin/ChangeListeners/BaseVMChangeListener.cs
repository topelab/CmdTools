using ReactiveUI;
using SplitPDFWin.Extensions;
using SplitPDFWin.ViewModels;
using System;
using System.Collections.Generic;

namespace SplitPDFWin.ChangeListeners
{
    internal abstract class BaseVMChangeListener<TViewModel, TModel>
        where TViewModel : BaseVM<TViewModel, TModel>
        where TModel : class
    {
        private readonly List<string> propertiesToExcludeListening =
        [
            nameof(BaseVM<TViewModel>.HasErrors),
            nameof(BaseVM<TViewModel>.ErrorMessages)
        ];


        public virtual void Start(TViewModel vm)
        {
            vm.Unsubscribe();
            PreSubscribe(vm);
            Subscribe(vm);
            PostSubscribe(vm);
        }

        protected abstract bool UpdateModelPropertyWithVMValue(string propertyName, TViewModel vm, TModel model);

        protected virtual void PreSubscribe(TViewModel vm) { }

        protected virtual void Subscribe(TViewModel vm)
        {
            vm.Changed.Subscribe(OnPropertyChanged).RegisterSubscription(vm);
        }

        protected virtual void PostSubscribe(TViewModel vm) { }

        private void OnPropertyChanged(IReactivePropertyChangedEventArgs<IReactiveObject> args)
        {
            if (!propertiesToExcludeListening.Contains(args.PropertyName!))
            {
                var vm = args.Sender as TViewModel;
                var model = vm.Model;
                vm.IsUpdated = UpdateModelPropertyWithVMValue(args.PropertyName!, vm, model);
            }
        }
    }
}