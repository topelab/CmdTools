using System;
using System.Windows.Input;

namespace SplitPDFWin.Factories
{
    public interface ICommandFactoryBase<C>
        where C : class
    {
        ICommand Create(C context = null, IObservable<bool> canExecuteObservable = null);

        IObservable<bool> CanExecuteObservable { get; }
    }

    public interface ICommandFactory<C> : ICommandFactoryBase<C>
        where C : class
    {
        void Execute();
    }

    public interface ICommandFactory : ICommandFactory<object>
    {
    }

    public interface ICommandFactory<C, T> : ICommandFactoryBase<C>
        where C : class
        where T : class
    {
        ICommand Create(C context = null, T canExecuteParam = null);

        bool CanExecute(T param);
        void Execute(T param);
    }

}
