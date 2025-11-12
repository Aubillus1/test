using HsPvSerAPG.Entidad;
using HsPvSerAPG.Utilis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HsPvSerAPG.Controlador
{
    public class TabRCC
    {
        public int Cia { get; set; }               // Código de la compañía
        public int TipDoc { get; set; }            // Tipo de documento (97 = Ingreso, 98 = Egreso)
        public int NroSer { get; set; }            // Número de serie
        public string NumDoc { get; set; }         // Número de documento
        public string Fecha { get; set; }          // Fecha del movimiento
        public string RazSoc { get; set; }         // Razón social o nombre del proveedor/cliente
        public string TipMon { get; set; }         // Tipo de moneda
        public decimal Monto { get; set; }         // Monto del movimiento
        public string Obs { get; set; }            // Observaciones
        public string CodCaja { get; set; }        // Código de caja
        public int StAnticipo { get; set; }        // Indica si es anticipo (0 = no, 1 = sí)
        public string CodCli { get; set; }         // Código del cliente o responsable
        public int CodMotivo { get; set; }         // Código del motivo del movimiento
    }

    // Clases para mapear JSON
    public class TabRCCTipoDoc
    {
        public int cod { get; set; }
        public string des { get; set; }
    }

    public class TabRCCMotivo
    {
        public int cod { get; set; }
        public string des { get; set; }
    }
}
