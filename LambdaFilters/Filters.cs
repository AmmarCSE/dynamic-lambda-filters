using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Agency.EntityFramework;

namespace LambdaFilters
{
    public class Filters
    {
        public Filter<Allotment, Hotel> Hotels = new Filter<Allotment, Hotel>()
        {
            MainSetKey = "HotelID"
            , FilterSetKey = "HotelID"
            , FilterSetDisplayProperty = "HotelName"
            , FilterTitle = "Hotels"
        };

        public Filters()
        {
            Class1 c = new Class1();
            c.Listtesting123(Hotels);
        }
    }
}
