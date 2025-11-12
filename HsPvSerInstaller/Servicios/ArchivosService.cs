using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HsPvSerInstaller2.Servicios
{
    public class ArchivosService
    {
        public static bool EliminarArchivo(string ruta)
        {
            if (File.Exists(ruta))
            {
                File.Delete(ruta);
                return true;
            }
            return false;
        }
    }
}
