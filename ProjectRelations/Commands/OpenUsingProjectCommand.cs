namespace ProjectRelations
{
    using ProjectRelations.Services;

    [Command(PackageIds.OpenUsingProjectCommand)]
    internal sealed class OpenUsingProjectCommand : BaseCommand<OpenUsingProjectCommand>
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

            // Ruta completa del archivo .sln (incluye nombre de archivo)
            var solutionFullName = dte.Solution?.FullName;
            if (string.IsNullOrEmpty(solutionFullName))
            {
                await VS.MessageBox.ShowWarningAsync("ProjectRelations", "No hay solución abierta o no está guardada");
                return;
            }

            var sel = selectedItems.Item(1);
            EnvDTE.Project project = sel.Project ?? sel.ProjectItem?.ContainingProject;

            if (project == null)
            {
                await VS.MessageBox.ShowWarningAsync("ProjectRelations", "El elemento seleccionado no pertenece a un proyecto");
                return;
            }

            // Directorio de la solución
            var solutionDir = System.IO.Path.GetDirectoryName(solutionFullName);

            var projectRelationsOpener = SetupDI.Container.GetInstance<IProjectRelationsOpener>();
            projectRelationsOpener.OpenUsingProject(solutionDir, project.Name);
        }
    }
}
