using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opstreetIO
{
    internal class Provincie
    {
        public int Id { get; set; }
        public string Naam { get; set; }
        public string Taalcode { get; set; }
        public int GemeenteId { get; set; }

        public override string ToString()
        {
            return Naam;
        }

    }
}
