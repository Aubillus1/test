using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HsPvSerAPG.Entidad
{
    public class PersonaReniec
    {
        public string document_number { get; set; }
        public string full_name { get; set; }
        public string first_name { get; set; }
        public string first_last_name { get; set; }
        public string second_last_name { get; set; }
        public bool success { get; set; }
        public PersonaReniec data { get; set; }
    }
}
