using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HsPvSerAPG.Entidad
{
    public class TabBan
    {
        public int cod {  get; set; }
        public string des { get; set; } = string.Empty;
        public string nrocta { get; set; } = string.Empty;
        public string cci { get; set; } = string.Empty;
        public int tipmon { get; set; }
    }
}
