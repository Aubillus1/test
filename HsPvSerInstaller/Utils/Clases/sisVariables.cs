using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HsPvSerInstaller2.Utils.Clases
{
    public static class sisVariables
    {
        public static string sisVersionO = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public static string sisServerUrl()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "server.hs");

            if (!File.Exists(path))
                throw new FileNotFoundException("No se encontró el archivo de configuración server.hs en la carpeta raíz.");

            string contenido = File.ReadAllText(path).Trim();

            return contenido;
        }

        public static string GAPI => $"http://{sisServerUrl().TrimEnd('/')}/api/";

        public static string basePath = AppDomain.CurrentDomain.BaseDirectory;

        public static string destinoExe = Path.Combine(basePath, "HsPvSerWPF.exe");

    }
}
