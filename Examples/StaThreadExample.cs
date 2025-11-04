using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

class StaWindowStarter
{
    public void StartWindowOnStaThread()
    {
        var staThread = new Thread(() =>
        {
            // Opcional: crear Application si no existe
            if (Application.Current == null)
            {
                new Application();
            }

            var window = new Window { Title = "STA Window" };
            window.Closed += (s, e) =>
            {
                // Termina el dispatcher cuando la ventana se cierre
                Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
            };
            window.Show();

            // Inicia el loop de mensajes del hilo STA
            Dispatcher.Run();
        });

        staThread.SetApartmentState(ApartmentState.STA); // MUST set before Start
        staThread.IsBackground = true;
        staThread.Start();
    }
}