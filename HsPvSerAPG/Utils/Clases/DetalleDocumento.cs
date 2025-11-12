using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HsPvSerAPG.Utils.Clases
{
    public class DetalleDocumento
    {
        public int Item { get; set; }
        public string CodArt { get; set; }
        public string Descripcion { get; set; }
        public string Udm { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Descuento { get; set; }
        public decimal Monto => (Cantidad * PrecioUnitario) - Descuento;
    }
}
