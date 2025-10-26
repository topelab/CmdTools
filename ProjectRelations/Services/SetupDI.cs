namespace ProjectRelations.Services
{
    using SimpleInjector;

    internal static class SetupDI
    {
        public static Container Container { get; private set; } = null!;

        public static void Initialize()
        {
            var container = new Container();

            // Registrar singletons usados por la extensión
            container.RegisterSingleton<IJsonSettings, JsonSettings>();
            container.RegisterSingleton<IUserSettingsFactory, UserSettingsFactory>();
            container.Register<IProjectRelationsOpener, ProjectRelationsOpener>();

            // Verificar configuración (intenta resolver servicios registrados)
            container.Verify();

            Container = container;
        }
    }
}
