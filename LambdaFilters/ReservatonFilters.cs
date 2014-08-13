using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Agency.EntityFramework;

namespace LambdaFilters
{
    public class ReservatonFilters
    {
        public Filter<Reservation, Hotel> Hotels = new Filter<Reservation, Hotel>()
        {
            MainSetKey = "HotelID"
            , FilterSetKey = "HotelID"
            , FilterSetDisplayProperty = "HotelName"
            , FilterTitle = "Hotels"
        };

        public List<FilterSearchItem> SearchItems { get; set; }

        public ReservatonFilters()
        {
            FilterSearchItem search = new FilterSearchItem();
            search.SearchKey = "HotelID";
            search.SearchType = "List<int>";
            search.SearchData = "5466,5467";
            SearchItems = new List<FilterSearchItem>();
            SearchItems.Add(search);

            Class1 c = new Class1();
            c.Listtesting123(Hotels, SearchItems);
        }
    }
}
