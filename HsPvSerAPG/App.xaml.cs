using HsPvSerAPG.Utilis;
using HsPvSerAPG.Utils.Clases;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

namespace HsPvSerAPG
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {
        [DllImport("kernel32", SetLastError = true)]
        static extern IntPtr LoadLibrary(string lpFileName);
        protected override void OnStartup(StartupEventArgs e)
        {
            string url = "";
            base.OnStartup(e);

            string theme = HsPvSerAPG.Properties.Settings.Default.Tema;

            // Si no hay tema guardado, usar por defecto
            if (string.IsNullOrEmpty(theme))
                theme = "TemaClaro";

            InitializeComponent();

            string servidor = ConfigServer.LeerServidor();
            Console.WriteLine($"Servidor cargado desde server.hs: {servidor}");

            base.OnStartup(e);
        }
    }
}
