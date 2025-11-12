﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace Actualizar.Vista.ControlUsuario
{
    /// <summary>
    /// Lógica de interacción para Welcom.xaml
    /// </summary>
    public partial class Welcom : UserControl
    {
        public Welcom()
        {
            InitializeComponent();
        }
        public static string CargarServidor()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "server.hs");

            if (!File.Exists(path))
                throw new FileNotFoundException("No se encontró el archivo de configuración server.hs en la carpeta raíz.");

            return File.ReadAllText(path).Trim();
        }

        // URL de la API que devuelve directamente el .exe
        public static string apiUrl => $"https://{CargarServidor().TrimEnd('/')}/api/Actualizar";

        private async Task DescargarArchivo(string url, string destino)
        {
            // Si existe el exe anterior, lo eliminamos
            if (File.Exists(destino))
                File.Delete(destino);

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(destino, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await stream.CopyToAsync(fileStream);
                }
            }
        }

        private async Task EjecutarActualizacion()
        {
            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string destinoExe = Path.Combine(basePath, "HsPvSerAPG.exe");

                // Descargar directamente el nuevo exe
                await DescargarArchivo(apiUrl, destinoExe);

                MessageBox.Show("Actualización completada. La aplicación se reiniciará.",
                    "Actualización", MessageBoxButton.OK, MessageBoxImage.Information);

                // Lanzar el nuevo exe
                Process.Start(new ProcessStartInfo
                {
                    FileName = destinoExe,
                    UseShellExecute = true,
                    WorkingDirectory = basePath
                });

                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error durante el proceso de actualización: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(200);
            await EjecutarActualizacion();
        }
    }
}
