namespace ProjectRelations2022
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.VisualStudio.Extensibility;
    using ProjectRelations2022.Services;
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
                    id: "ProjectRelations2022.d36240c7-3cbc-4ceb-96aa-ce0aee85b8b9",
                    version: this.ExtensionAssemblyVersion,
                    publisherName: "Joan López",
                    displayName: "Project Relations 2022",
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
