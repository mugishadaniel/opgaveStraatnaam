using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opstreetIO
{
    internal class Straat
    {
        public int Id { get; set; }
        public string Naam { get; set; }
        public int GemeenteId { get; set; }
        public int ProvincieId { get; set; }
        public string GemeenteNaam { get; set; }
        public string ProvincieNaam { get; set; }
    }
}
