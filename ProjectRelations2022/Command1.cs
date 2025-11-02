namespace ProjectRelations2022
{
    using Microsoft;
    using Microsoft.VisualStudio.Extensibility;
    using Microsoft.VisualStudio.Extensibility.Commands;
    using Microsoft.VisualStudio.Extensibility.Shell;
    using Microsoft.VisualStudio.ProjectSystem.Query;
    using System.Diagnostics;

    /// <summary>
    /// Command1 handler.
    /// </summary>
    [VisualStudioContribution]
    internal class Command1 : Command
    {
        private readonly TraceSource logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Command1"/> class.
        /// </summary>
        /// <param name="traceSource">Trace source instance to utilize.</param>
        public Command1(TraceSource traceSource)
        {
            // This optional TraceSource can be used for logging in the command. You can use dependency injection to access
            // other services here as well.
            logger = Requires.NotNull(traceSource, nameof(traceSource));
        }

        /// <inheritdoc />
        public override CommandConfiguration CommandConfiguration => new("%ProjectRelations2022.Command1.DisplayName%")
        {
            // Use this object initializer to set optional parameters for the command. The required parameter,
            // displayName, is set above. DisplayName is localized and references an entry in .vsextension\string-resources.json.
            Icon = new(ImageMoniker.KnownValues.Extension, IconSettings.IconAndText),
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
            var projectPath = await context.GetSelectedPathAsync(cancellationToken);
            var workspace = this.Extensibility.Workspaces();

            IQueryResults<IProjectSnapshot> results = await workspace.QueryProjectsAsync(
                project => project.Where(p => p.Path == projectPath.LocalPath).With(p => new { p.Name, p.Guid, p.Path }),
                cancellationToken);

            var projectSnapshot = results.FirstOrDefault();

            var solutions = await workspace.QuerySolutionAsync(
                solution => solution.With(s => new { s.BaseName, s.Path, s.Projects }),
                cancellationToken);

            var solution = solutions.First();

            await Extensibility.Shell().ShowPromptAsync($"Hello:\n - Project {projectSnapshot.Name}\n - Solution: {solution.Path}", PromptOptions.OK, cancellationToken);
        }
    }
}
