using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HsPvSerAPG.Entidad
{
    class AnticipoRequest
    {
        public string cia { get; set; } = string.Empty;
        public string tipdoc { get; set; } = string.Empty;
        public string numdoc { get; set; } = string.Empty;
        public int correl { get; set; } 
        public int docref { get; set; }
        public string numref { get; set; } = string.Empty;
        public int codcaja { get; set; }
        public decimal tipcam { get; set; }
        public decimal impsol { get; set; }
        public decimal impdol { get; set; }
        public string usrins { get; set; } = string.Empty;
    }
}
