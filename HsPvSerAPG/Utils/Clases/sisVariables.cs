using HsPvSerAPG.Entidad;
using HsPvSerAPG.Vista.Reenviar.Anticipo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HsPvSerAPG.Utilis
{

    public static class sisVariables
    {
        public static string CargarServidor()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "server.hs");

            if (!File.Exists(path))
                throw new FileNotFoundException("No se encontró el archivo de configuración server.hs en la carpeta raíz.");

            string contenido = File.ReadAllText(path).Trim();

            // http://192.168.18.36:8000/api
            return contenido;
        }
        public static string sisVersion =
        FileVersionInfo.GetVersionInfo(Path.GetFullPath("HsPvSerAPG.exe")).FileVersion;

        public static string GAPI = $"http://{CargarServidor().TrimEnd('/')}/api/";
        public static string sisName = "HsPvSerAPG";

        public static int GCodusr = 0;


        public static string GUsuario = "";
        public static int GCia = 0;
        public static int GCodSuc = 0;
        public static int GCodven = 0;
        public static string GSucursal = "";
        public static int GCodSucursal = 0;
        public static int GPeresp = 0;
        public static int GVerstock = 0;
        public static int Gnrodoc = 0;
        public static int GCodCaja = 0;

        public static int anticipo = 0;
        public static double salant = 0.0;
        public static int Gtipmon { get; set; } = 0;
        public static decimal Gtipcam { get; set; } = 0;

        public static string GNumLote = "";

        public static int stentregado = 0;

        //api para imagenes guardar
        public const String img = "https://www.amkdelivery.com/test_apirest/test_apg/api/subirImagenTabart";

        //api para mostrar imagen
        public const String GVimg = "https://www.amkdelivery.com/test_apirest/test_apg/img/tabart/";

        //api reniec

        public const String GAPIRENIEC = "http://www.amkdelivery.com/consulta-dni/api/v3";
        public static string GToken = "XR^N1KNnuUSgEnQ%LlaFxs@hnhjdohFr9o$ACLISWESQNPkw4$DKQekMNJEC";
    }
}
