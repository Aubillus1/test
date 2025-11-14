using HsPvSerInstaller2.Utils.Clases;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace HsPvSerInstaller2.Controlador
{
    public class VersionController
    {
        private readonly string url;
        private readonly string destino;
        public VersionController(string basePath, string destinoExe) 
        {
            this.url = basePath;
            this.destino = destinoExe;
        }
        public async Task DescargarArchivo()
        {
            if (File.Exists(destino))
            {
                string backup = destino + ".old";
                if (File.Exists(backup))
                    File.Delete(backup);

                File.Move(destino, backup);
            }

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

        public void EjecutandoSistema()
        {
            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string exePath = Path.GetFullPath(Path.Combine(basePath, @"..\HsPvSer\HsPvSerWPF.exe"));

                if (!File.Exists(exePath))
                {
                    MessageBox.Show($"No se encontró el archivo a ejecutar:\n{exePath}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Process.Start(new ProcessStartInfo
                {
                    FileName = exePath,
                    UseShellExecute = true,
                    WorkingDirectory = Path.GetDirectoryName(exePath)
                });

                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al intentar ejecutar el sistema: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
