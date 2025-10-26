namespace ProjectRelations
{
    using ProjectRelations.Services;

    [Command(PackageIds.OpenUsedByProjectCommand)]
    internal sealed class OpenUsedByProjectCommand : BaseCommand<OpenUsedByProjectCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var dte = await VS.GetServiceAsync<EnvDTE.DTE, EnvDTE.DTE>();

            var selectedItems = dte.SelectedItems;
            if (selectedItems.Count == 0)
            {
                await VS.MessageBox.ShowWarningAsync("ProjectRelations", "No hay ningún elemento seleccionado");
                return;
            }

            var sel = selectedItems.Item(1);
            var project = sel.Project ?? sel.ProjectItem?.ContainingProject;

            if (project == null)
            {
                await VS.MessageBox.ShowWarningAsync("ProjectRelations", "El elemento seleccionado no pertenece a un proyecto");
                return;
            }

            var projectRelationsOpener = SetupDI.Container.GetInstance<IProjectRelationsOpener>();
            projectRelationsOpener.OpenUsedByProject(project.FullName, project.Name);
        }
    }
}
