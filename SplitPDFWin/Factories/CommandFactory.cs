using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Windows.Input;

namespace SplitPDFWin.Factories
{
    public class CommandFactory<C> : ICommandFactory<C>
        where C : class
    {
        protected C context;

        public virtual ICommand Create(C context = null, IObservable<bool> canExecuteObservable = null)
        {
            this.context = context;
            return ReactiveCommand.Create(Execute, canExecuteObservable ?? CanExecuteObservable);
        }

        public virtual IObservable<bool> CanExecuteObservable { get; } = null;

        public virtual void Execute() { }
    }

    public class CommandFactory<C, T> : ICommandFactory<C, T>
        where C : class
        where T : class
    {
        protected C context;

        public virtual ICommand Create(C context = null, IObservable<bool> canExecuteObservable = null)
        {
            this.context = context;
            return ReactiveCommand.Create<T>(Execute, canExecuteObservable ?? CanExecuteObservable);
        }

        public virtual ICommand Create(C context = null, T canExecuteParam = null)
        {
            this.context = context;
            return ReactiveCommand.Create<T>(Execute, Observable.Return(CanExecute(canExecuteParam)));
        }

        public virtual IObservable<bool> CanExecuteObservable { get; } = null;

        public virtual bool CanExecute(T param) => true;
        public virtual void Execute(T param) { }
    }

    public class CommandFactory : CommandFactory<object>
    {
    }
}
