using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HsPvSerAPG.Entidad
{
    public class ApiResponseSunat
    {
        public bool success { get; set; }
        public SunatData data { get; set; }
    }

    public class SunatData
    {
        public string ruc { get; set; }
        public string razsoc { get; set; }
        public string estado { get; set; }
        public string condicion { get; set; }
        public string ubigeo { get; set; }
        public string tipovia { get; set; }
        public string nomvia { get; set; }
        public string codzona { get; set; }
        public string tipzona { get; set; }
        public string numero { get; set; }
        public string interior { get; set; }
        public string lote { get; set; }
        public string departamento { get; set; }
        public string manzana { get; set; }
        public string kilometro { get; set; }
        public string direccion { get; set; }
    }

}
