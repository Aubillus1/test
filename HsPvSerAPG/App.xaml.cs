using HsPvSerAPG.Utilis;
using HsPvSerAPG.Utils.Clases;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

namespace HsPvSerAPG
{
    public partial class App : Application
    {
        [DllImport("kernel32", SetLastError = true)]
        static extern IntPtr LoadLibrary(string lpFileName);

        public App()
        {
            // ⚠️ CAPTURA TODAS LAS EXCEPCIONES DEL HILO PRINCIPAL (UI)
            this.DispatcherUnhandledException += (s, e) =>
            {
                MessageBox.Show(
                    "ERROR GLOBAL (UI THREAD):\n\n" + e.Exception.ToString(),
                    "Excepción no controlada",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                e.Handled = true; // evita que la app se cierre
            };

            // ⚠️ CAPTURA EXCEPCIONES EN HILOS NO UI (HttpClient, tareas, etc.)
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                Exception ex = e.ExceptionObject as Exception;

                MessageBox.Show(
                    "ERROR GLOBAL (NON-UI THREAD):\n\n" + ex?.ToString(),
                    "Excepción Fatal",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            };

            // ⚠️ CAPTURA EXCEPCIONES DE TAREAS ASÍNCRONAS NO ESPERADAS
            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                MessageBox.Show(
                    "ERROR GLOBAL (TASK):\n\n" + e.Exception.ToString(),
                    "Excepción en Tarea",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                e.SetObserved(); // evita que cierre tu programa
            };
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string theme = HsPvSerAPG.Properties.Settings.Default.Tema;

            if (string.IsNullOrEmpty(theme))
                theme = "TemaClaro";

            InitializeComponent();

            string servidor = ConfigServer.LeerServidor();
            Console.WriteLine($"Servidor cargado desde server.hs: {servidor}");
        }
    }
}
