using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HsPvSerAPG.Entidad
{
    public class TabFacAnticipos
    {
        public int tipdoc { get; set; }
        public string numdoc { get; set; }
        public string fecha { get; set; }
        public string razsoc { get; set; }
        public int tipmon { get; set; }
        public string total { get; set; }
        public string salant { get; set; }

        public string NombreMoneda
        {
            get
            {
                return tipmon == 1 ? "SOLES"
                     : tipmon == 2 ? "DOLARES"
                     : "DESCONOCIDO";
            }
        }

    }
}
