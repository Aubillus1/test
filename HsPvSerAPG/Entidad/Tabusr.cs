using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HsPvSerAPG.Entidad
{
    public class Tabusr
    {
        public int cod { get; set; }
        public string usuario { get; set; } = string.Empty;
        public string nomusr { get; set; } = string.Empty;
        public string apeusr { get; set; } = string.Empty;
        public string fecalt { get; set; }
        public string fecbaj { get; set; } 

        public int st { get; set; }
        public int ciaact { get; set; }
        public int sucact { get; set; }
        public int? nivel { get; set; }
        public int codgru { get; set; }
        public int codcar { get; set; }
        public int stconta { get; set; }
        public int stsunatperm { get; set; }
        public int codven { get; set; }
        public int peresp { get; set; }
        public int verstock { get; set; }
        public int stproforma { get; set; }
        public int stfactura { get; set; }
        public int stboleta { get; set; }
        public int stne { get; set; }
        public int stgr { get; set; }
    }

}
