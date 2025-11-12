using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HsPvSerAPG.Entidad
{
    public class TablaUMD
    {
        public int Codigo { get; set; }
        public string Desuni { get; set; } = string.Empty;
        public decimal precioSOLES { get; set; }
        public decimal precioDOLARES { get; set; }
        public decimal precio_mayorSOLES { get; set; }
        public decimal precio_mayorDOLARES { get; set; }
        public decimal precio_minSOLES { get; set; }
        public decimal precio_minDOLARES { get; set; }
        public decimal precio_paseSOLES { get; set; }
        public decimal precio_paseDOLARES { get; set; }
        public decimal precio_pase1SOLES { get; set; }
        public decimal precio_pase1DOLARES { get; set; }
        public Tabpr_art EntidadOriginal { get; set; }
    }
}
