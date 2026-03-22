namespace ProjectRelations2026.Commands
{
    using Microsoft;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Internal.VisualStudio.Extensibility.Framework;
    using Microsoft.VisualStudio.Extensibility;
    using Microsoft.VisualStudio.Extensibility.Commands;
    using ProjectRelations2026.Services;
    using ProjectRelations2026.Views;
    using RelationsShared.DTO;
    using System.Diagnostics;

    /// <summary>
    /// ShowProjectRelationsCommand handler.
    /// </summary>
    [VisualStudioContribution]
    internal class ShowProjectRelationsCommand : Command
    {
        private readonly TraceSource logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowProjectRelationsCommand"/> class.
        /// </summary>
        /// <param name="traceSource">Trace source instance to utilize.</param>
        public ShowProjectRelationsCommand(TraceSource traceSource)
        {
            // This optional TraceSource can be used for logging in the command. You can use dependency injection to access
            // other services here as well.
            logger = Requires.NotNull(traceSource, nameof(traceSource));
        }

        /// <inheritdoc />
        public override CommandConfiguration CommandConfiguration => new("%ProjectRelations2026.ShowProjectRelationsCommand.DisplayName%")
        {
            // Use this object initializer to set optional parameters for the command. The required parameter,
            // displayName, is set above. DisplayName is localized and references an entry in .vsextension\string-resources.json.
            Icon = new(ImageMoniker.KnownValues.Relationship, IconSettings.IconAndText),
            Placements = [CommandPlacement.VsctParent(new Guid("{d309f791-903f-11d0-9efc-00a0c911004f}"), id: 0x014D, priority: 0x0000),
                          CommandPlacement.VsctParent(new Guid("{d309f791-903f-11d0-9efc-00a0c911004f}"), id: 0x0206, priority: 0x2000)],
            /// El comando está habilitado cuando:
            /// - La solución tiene múltiples proyectos
            /// - La solución está completamente cargada
            /// Si el usuario selecciona algo que no es un proyecto, el comando mostrará un error descriptivo
            EnabledWhen = ActivationConstraint.SolutionState(SolutionState.MultipleProject)
                & ActivationConstraint.SolutionState(SolutionState.FullyLoaded)
                & ActivationConstraint.ClientContext(ClientContextKey.Shell.ActiveSelectionFileName, @".+")
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
            try
            {
                var workspace = Extensibility.Workspaces();
                var projectInfo = await workspace.GetSelectedProjectDetailsAsync(context, cancellationToken);
                var relationsContextFactory = ExtensionContext.ServiceProvider.GetService<IRelationsContextFactory>();

                var result = await relationsContextFactory.CreateAsync(RelationType.UsedBy, projectInfo);
                var remoteControl = new RelationsUserControl(result);
                await this.Extensibility.Shell().ShowDialogAsync(remoteControl, result.Title, cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                logger.TraceInformation(ex.Message);
            }
            catch (Exception ex)
            {
                logger.TraceInformation($"Error ejecutando comando: {ex.Message}");
            }
        }
    }
}
