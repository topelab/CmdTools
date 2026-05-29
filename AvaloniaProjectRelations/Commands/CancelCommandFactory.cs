using AvaloniaProjectRelations.Resources;
using ReactiveUI;
using Topelab.Core.Avalonia.Base;
using Topelab.Core.Avalonia.Command;
using Topelab.Core.Avalonia.Services;
using Topelab.Core.Domain.Extensions;

namespace AvaloniaProjectRelations.Commands
{
    /// <summary>
    /// Comando para cancelar
    /// </summary>
    /// <remarks>
    /// Se le preguntará al usuario si quiere cancelar los cambios
    /// </remarks>
    internal class CancelCommandFactory(IMessageService messageService) : CommandFactory<BaseVM>, ICancelCommandFactory
    {
        protected override IObservable<bool> CanExecuteObservable => context.WhenAnyValue(c => c.IsMain, x => !x);

        protected async override void Execute()
        {
            if (context != null && !await CanCancelChanges(context))
            {
                return;
            }
            context.Unsubscribe();
            App.MainVM.ContentViewModel = context.Parent ?? App.MainVM.PrincipalVM;
        }

        private async Task<bool> CanCancelChanges(BaseVM vm)
        {
            var cancelChanges = true;

            if (vm.SaveAndCloseCommand?.CanExecute(null) ?? false)
            {
                cancelChanges = await messageService.ShowQuestionBoxAsync(Strings.Warning, Strings.DataModifiedMessage);
            }

            return cancelChanges;
        }
    }
}
