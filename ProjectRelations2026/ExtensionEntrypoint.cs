namespace ProjectRelations2026
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.VisualStudio.Extensibility;
    using ProjectRelations2026.Services;
    using Topelab.Core.Resolver.Microsoft;

    /// <summary>
    /// Extension entrypoint for the VisualStudio.Extensibility extension.
    /// </summary>
    [VisualStudioContribution]
    internal class ExtensionEntrypoint : Extension
    {
        /// <inheritdoc/>
        public override ExtensionConfiguration ExtensionConfiguration => new()
        {
            Metadata = new(
                    id: "ProjectRelations2026.496f101e-be17-4727-9b84-c0381ed0e400",
                    version: this.ExtensionAssemblyVersion,
                    publisherName: "Joan López",
                    displayName: "Project Relations 2026",
                    description: "Shows a diagram of project relations"),
        };

        /// <inheritdoc />
        protected override void InitializeServices(IServiceCollection serviceCollection)
        {
            base.InitializeServices(serviceCollection);
            serviceCollection.AddResolver(SetupDI.GetResolveInfoCollection());
            Environment.SetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", "--allow-file-access-from-files");
        }
    }
}
