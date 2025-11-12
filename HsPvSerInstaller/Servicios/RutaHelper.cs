using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HsPvSerInstaller2.Servicios
{
    public class RutaHelper
    {
        public static string ObtenerRutaExe(string subCarpeta, string exeNombre)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string projectPath = Path.GetFullPath(Path.Combine(basePath, $@"..\{subCarpeta}"));
            return Path.Combine(projectPath, exeNombre);
        }
    }
}
