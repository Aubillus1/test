using HsPvSerAPG.Utils.Clases;
using System;
using System.IO;

namespace HsPvSerAPG.Utils.Clases
{
    public static class ConfigServer
    {
        private static readonly string configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "server.hs");

        public static string LeerServidor()
        {
            try
            {
                if (!File.Exists(configFile))
                    throw new FileNotFoundException("No se encontró el archivo de configuración server.hs");

                string url = File.ReadAllText(configFile).Trim();

                if (string.IsNullOrEmpty(url))
                    throw new Exception("El archivo server.hs está vacío.");

                return url;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Error al leer la configuración del servidor:\n{ex.Message}",
                    "Configuración",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error
                );

                // Valor por defecto de respaldo
                return null;
            }
        }
    }
}