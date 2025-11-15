using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Actualizar
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.Dispatcher.InvokeAsync(() =>
            {
                // Obtener la ventana creada automáticamente por WPF
                var win = Current.MainWindow;

                if (win != null)
                {
                    win.Width = 50;  

                    win.Height =60;  

                    win.ResizeMode = ResizeMode.NoResize;
                    win.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                    // (Opcional) Sin borde
                    win.WindowStyle = WindowStyle.None;
                }
            });
        }
    }
}

