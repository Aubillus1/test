using Newtonsoft.Json;
using System;

using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HsPvSerAPG.Entidad
{
    public class Pagos
    {
        [JsonProperty("cia")]
        public int cia { get; set; }

        [JsonProperty("codsuc")]
        public int codsuc { get; set; }

        [JsonProperty("fecha")]
        public string fecha { get; set; }

        [JsonProperty("ptipopago")]
        public int ptipopago { get; set; }

        [JsonProperty("impsol")]
        public decimal impsol { get; set; }

        [JsonProperty("impdol")]
        public decimal impdol { get; set; }

        [JsonProperty("vuesol")]
        public decimal vuesol { get; set; }

        [JsonProperty("vuedol")]
        public decimal vuedol { get; set; }

        [JsonProperty("codbantar")]
        public int codbantar { get; set; }

        [JsonProperty("nrocheope")]
        public string nrocheope { get; set; }

        [JsonProperty("feccheope")]
        public string feccheope { get; set; }

        [JsonProperty("fecdif")]
        public string fecdif { get; set; }

        [JsonProperty("tipcam")]
        public decimal tipcam { get; set; }

        [JsonProperty("cobobs")]
        public string cobobs { get; set; }

        [JsonProperty("correl")]
        public int correl { get; set; }

        [JsonProperty("documentos")]
        public List<Documentos> Documentos { get; set; } = new List<Documentos>();


    }

    public class Documentos
    {
        [JsonProperty("tipdoc")]
        public int tipdoc { get; set; }

        [JsonProperty("numdoc")]
        public string numdoc { get; set; }

        [JsonProperty("tipmon")]
        public decimal tipmon { get; set; }

        [JsonProperty("importe")]
        public decimal importe { get; set; }

    }


    public class PagoUnicoDoc
    {
        [JsonProperty("cia")]
        public int cia { get; set; }

        [JsonProperty("codsuc")]
        public int codsuc { get; set; }

        [JsonProperty("tipdoc")]
        public int tipdoc { get; set; }

        [JsonProperty("numdoc")]
        public string numdoc { get; set; } = string.Empty;

        [JsonProperty("codusr")]
        public int codusr { get; set; }


        [JsonProperty("detpagos")]
        public List<DetpagosUnicos> detpagos { get; set; } = new List<DetpagosUnicos>();
    }
    public class DetpagosUnicos
    {
        [JsonProperty("ptipopago")]
        public int ptipopago { get; set; }

        [JsonProperty("pimpsol")]
        public decimal Pimpsol { get; set; }

        [JsonProperty("pimpdol")]
        public decimal pimpdol { get; set; }

        [JsonProperty("pvuesol")]
        public decimal pvuesol { get; set; }

        [JsonProperty("pvuedol")]
        public decimal pvuedol { get; set; }

        [JsonProperty("pcodbantar")]
        public int pcodbantar { get; set; }

        [JsonProperty("pnrocheope")]
        public string pnrocheope { get; set; } = string.Empty;

        [JsonProperty("pfeccheope")]
        public string pfeccheope { get; set; }

        [JsonProperty("pfecdif")]
        public string pfecdif { get; set; }

        [JsonProperty("ptipcam")]
        public decimal ptipcam { get; set; }

        [JsonProperty("pcobobs")]
        public string pcobobs { get; set; } = string.Empty;


    }

    public class RespuestaCobranza
    {
        public bool success { get; set; }
        public string message { get; set; } = string.Empty;
        public List<int> correls { get; set; }
    }


}

