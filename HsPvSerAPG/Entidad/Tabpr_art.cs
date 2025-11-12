using HsPvSerAPG.Utilis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HsPvSerAPG.Entidad
{
    public class Tabpr_art
    {
        public int cia { get; set; }
        public string codart { get; set; } = string.Empty;

        [JsonProperty("coduni")]
        public int coduni { get; set; }

        [JsonProperty("desuni")]
        public string desuni { get; set; } = string.Empty;

        [JsonProperty("desuniABR")]
        public string desuniABR { get; set; } = string.Empty;

        [JsonProperty("precio")]
        public decimal precio { get; set; }

        [JsonProperty("precio_mayor")]
        public decimal precio_mayor { get; set; }

        [JsonProperty("precio_min")]
        public decimal precio_min { get; set; }

        [JsonProperty("precio_pase")]
        public decimal precio_pase { get; set; }

        [JsonProperty("precio_pase1")]
        public decimal precio_pase1 { get; set; }

        [JsonProperty("tipmon")]
        public int tipmon { get; set; }
    }
}
