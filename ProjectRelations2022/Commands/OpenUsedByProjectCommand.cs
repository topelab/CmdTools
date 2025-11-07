namespace ProjectRelations2022.Commands
{
    using Microsoft;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Internal.VisualStudio.Extensibility.Framework;
    using Microsoft.VisualStudio.Extensibility;
    using Microsoft.VisualStudio.Extensibility.Commands;
    using ProjectRelations2022.Services;
    using ProjectRelations2022.Views;
    using System.Diagnostics;

    /// <summary>
    /// OpenUsedByProjectCommand handler.
    /// </summary>
    [VisualStudioContribution]
    internal class OpenUsedByProjectCommand : Command
    {
        private readonly TraceSource logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenUsedByProjectCommand"/> class.
        /// </summary>
        /// <param name="traceSource">Trace source instance to utilize.</param>
        public OpenUsedByProjectCommand(TraceSource traceSource)
        {
            // This optional TraceSource can be used for logging in the command. You can use dependency injection to access
            // other services here as well.
            logger = Requires.NotNull(traceSource, nameof(traceSource));
        }

        /// <inheritdoc />
        public override CommandConfiguration CommandConfiguration => new("%ProjectRelations2022.OpenUsedByProjectCommand.DisplayName%")
        {
            // Use this object initializer to set optional parameters for the command. The required parameter,
            // displayName, is set above. DisplayName is localized and references an entry in .vsextension\string-resources.json.
            Icon = new(ImageMoniker.KnownValues.Relationship, IconSettings.IconAndText),
            Placements = [CommandPlacement.VsctParent(new Guid("{d309f791-903f-11d0-9efc-00a0c911004f}"), id: 0x0234, priority: 0)],
        };

        /// <inheritdoc />
        public override Task InitializeAsync(CancellationToken cancellationToken)
        {
            // Use InitializeAsync for any one-time setup or initialization.
            return base.InitializeAsync(cancellationToken);
        }

        /// <inheritdoc />
        public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
        {
            var workspace = Extensibility.Workspaces();
            var projectInfo = await workspace.GetSelectedProjectDetailsAsync(context, cancellationToken);
            var projectRelationsOpener = ExtensionContext.ServiceProvider.GetService<IProjectRelationsOpener>();

            var result = await projectRelationsOpener.OpenUsedByProjectAsync(projectInfo.Path, projectInfo.Name);
            var remoteControl = new RelationsWindow(result);
            await this.Extensibility.Shell().ShowDialogAsync(remoteControl, cancellationToken);
        }
    }
}
