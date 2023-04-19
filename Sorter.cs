using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opstreetIO
{
    class SorterComparer : IComparer<Straat>
    {
        public int Compare(Straat x, Straat y)
        {
            int result = x.ProvincieNaam.CompareTo(y.ProvincieNaam);

            if (result == 0)
            {
                result = x.GemeenteNaam.CompareTo(y.GemeenteNaam);

                if (result == 0)
                {
                    result = x.Naam.CompareTo(y.Naam);
                }
            }

            return result;
            
        }
    }
}
