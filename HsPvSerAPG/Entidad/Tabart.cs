using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HsPvSerAPG.Entidad
{
    public class Tabart
    {
        public int cia { get; set; }
        public string codart {  get; set; } = string.Empty;
        public string codfab { get; set; } = string.Empty;
        public string descri { get; set; } = string.Empty;
        public string modelo { get; set; } = string.Empty;
        public int codmar { get; set; }
        public string desmar { get; set; } = string.Empty;

        public int tipmon { get; set; }

        public string formato { get; set; } = "";
       

        private Producto productoParaCantidad;
        private double precioOriginal;
        private int tipmonOriginal;

    }
    public class Tabmon
    {
        public int cod { get; set; }

        [JsonProperty("des")]
        public string des { get; set; } = string.Empty;

        [JsonProperty("desabr")]
        public string desabr { get; set; } = string.Empty;
    }
}
