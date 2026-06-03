namespace AvaloniaProjectRelations.MainControl
{
    using CmdTools.Shared;
    using Topelab.Core.Avalonia.Base;

    internal class MainControlVMChangeListener(IMainControlVMInitializer mainControlVMInitializer) : BaseVMChangeListener<MainControlVM, ProjectOptions>(null), IMainControlVMChangeListener
    {
        private readonly IMainControlVMInitializer mainControlVMInitializer = mainControlVMInitializer;

        protected override bool UpdateModelPropertyWithVMValue(string propertyName, MainControlVM vm, ProjectOptions model)
        {
            var isUpdated = true;

            switch (propertyName)
            {
                case nameof(MainControlVM.ShowListOnly):
                    model.RenderType = vm.ShowListOnly ? RenderType.Text : RenderType.Mermaid;
                    break;
                case nameof(MainControlVM.IncludePackages):
                    model.WithPackages = vm.IncludePackages;
                    break;
                case nameof(MainControlVM.IsUsedBy):
                case nameof(MainControlVM.SelectedItem):
                    if (vm.Projects.Contains(vm.SelectedItem))
                    {
                        model.RootPath = vm.IsUsing ? vm.SolutionPath : vm.ProjectData.GetProjectPath(vm.SelectedItem);
                        model.PinnedElement = vm.IsUsing ? vm.SelectedItem : null;
                        model.SelectedElement = vm.SelectedItem;
                    } else
                    {
                        isUpdated = false;
                    }
                    break;
                case nameof(MainControlVM.IsUsing):
                default:
                    isUpdated = false;
                    break;
            }

            if (isUpdated)
            {
                mainControlVMInitializer.Initialize(vm);
            }

            return isUpdated;
        }
    }
}
