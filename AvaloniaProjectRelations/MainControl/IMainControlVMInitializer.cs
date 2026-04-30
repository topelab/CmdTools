namespace AvaloniaProjectRelations.MainControl
{
    internal interface IMainControlVMInitializer
    {
        void Initialize(MainControlVM vm, bool isFirstInitialization = false);
    }
}
